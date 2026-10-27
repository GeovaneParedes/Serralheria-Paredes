using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services;
using Serralheria_Paredes.Services.Interfaces;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class InsumoServiceTests
{
    [Fact]
    public async Task MovimentarAsync_deve_registrar_entrada_com_saldos()
    {
        var insumo = new Insumo { Id = 1, Codigo = "PER-001", Descricao = "Perfil", EstoqueAtual = 5 };
        var repository = new FakeInsumoRepository(insumo);
        var movimentacoes = new FakeMovimentacaoRepository();
        var service = new InsumoService(repository, movimentacoes, NullLogger<InsumoService>.Instance);

        var result = await service.MovimentarAsync(1, TipoMovimentacaoEstoque.Entrada, 3, "Compra");

        Assert.True(result.Success);
        Assert.Equal(8, insumo.EstoqueAtual);
        var movimentacao = Assert.Single(movimentacoes.Items);
        Assert.Equal(5, movimentacao.SaldoAnterior);
        Assert.Equal(8, movimentacao.SaldoPosterior);
    }

    [Fact]
    public async Task MovimentarAsync_deve_rejeitar_saida_maior_que_o_saldo()
    {
        var insumo = new Insumo { Id = 1, Codigo = "PER-001", Descricao = "Perfil", EstoqueAtual = 2 };
        var movimentacoes = new FakeMovimentacaoRepository();
        var service = new InsumoService(
            new FakeInsumoRepository(insumo),
            movimentacoes,
            NullLogger<InsumoService>.Instance);

        var result = await service.MovimentarAsync(1, TipoMovimentacaoEstoque.Saida, 3, "Consumo na obra");

        Assert.False(result.Success);
        Assert.Equal(2, insumo.EstoqueAtual);
        Assert.Empty(movimentacoes.Items);
    }

    [Fact]
    public async Task ConsumirParaOrdemAsync_deve_vincular_a_baixa_a_os_em_producao()
    {
        var insumo = new Insumo { Id = 1, Codigo = "PER-001", Descricao = "Perfil", EstoqueAtual = 10 };
        var ordem = new OrdemServico { Id = 8, Numero = "OS-8", Status = StatusOrdemServico.EmCorte };
        var movimentacoes = new FakeMovimentacaoRepository();
        var service = new InsumoService(
            new FakeInsumoRepository(insumo),
            movimentacoes,
            NullLogger<InsumoService>.Instance,
            new FakeOrdemRepository(ordem));

        var result = await service.ConsumirParaOrdemAsync(8, 1, 3);

        Assert.True(result.Success);
        Assert.Equal(7, insumo.EstoqueAtual);
        Assert.Equal(8, Assert.Single(movimentacoes.Items).OrdemServicoId);
    }

    private sealed class FakeInsumoRepository : IInsumoRepository
    {
        private readonly List<Insumo> _items;

        public FakeInsumoRepository(params Insumo[] items) => _items = items.ToList();

        public Task<Insumo?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_items.FirstOrDefault(item => item.Id == Convert.ToInt32(id)));

        public Task<IReadOnlyList<Insumo>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Insumo>>(_items);

        public Task<IReadOnlyList<Insumo>> FindAsync(Expression<Func<Insumo, bool>> predicate, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Insumo>>(_items.AsQueryable().Where(predicate).ToList());

        public Task<bool> ExistsAsync(Expression<Func<Insumo, bool>> predicate, CancellationToken cancellationToken = default) =>
            Task.FromResult(_items.AsQueryable().Any(predicate));

        public Task AddAsync(Insumo entity, CancellationToken cancellationToken = default)
        {
            _items.Add(entity);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<Insumo> entities, CancellationToken cancellationToken = default)
        {
            _items.AddRange(entities);
            return Task.CompletedTask;
        }

        public void Update(Insumo entity)
        {
        }

        public void Remove(Insumo entity) => _items.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

        public Task<IReadOnlyList<Insumo>> ObterTodosComFiltroAsync(string? termo, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Insumo>>(_items);

        public Task<bool> CodigoJaCadastradoAsync(string codigo, int? ignorarId = null, CancellationToken cancellationToken = default) =>
            Task.FromResult(_items.Any(item => item.Codigo == codigo && item.Id != ignorarId));
    }

    private sealed class FakeMovimentacaoRepository : IMovimentacaoEstoqueRepository
    {
        public List<MovimentacaoEstoque> Items { get; } = new();

        public Task<MovimentacaoEstoque?> GetByIdAsync(object id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(item => item.Id == Convert.ToInt32(id)));

        public Task<IReadOnlyList<MovimentacaoEstoque>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MovimentacaoEstoque>>(Items);

        public Task<IReadOnlyList<MovimentacaoEstoque>> FindAsync(Expression<Func<MovimentacaoEstoque, bool>> predicate, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MovimentacaoEstoque>>(Items.AsQueryable().Where(predicate).ToList());

        public Task<bool> ExistsAsync(Expression<Func<MovimentacaoEstoque, bool>> predicate, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.AsQueryable().Any(predicate));

        public Task AddAsync(MovimentacaoEstoque entity, CancellationToken cancellationToken = default)
        {
            entity.Id = Items.Count + 1;
            Items.Add(entity);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<MovimentacaoEstoque> entities, CancellationToken cancellationToken = default)
        {
            Items.AddRange(entities);
            return Task.CompletedTask;
        }

        public void Update(MovimentacaoEstoque entity)
        {
        }

        public void Remove(MovimentacaoEstoque entity) => Items.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);

        public Task<IReadOnlyList<MovimentacaoEstoque>> ObterHistoricoAsync(int? insumoId = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MovimentacaoEstoque>>(Items);
    }

    private sealed class FakeOrdemRepository : IRepository<OrdemServico>
    {
        private readonly OrdemServico _ordem;

        public FakeOrdemRepository(OrdemServico ordem) => _ordem = ordem;
        public Task<OrdemServico?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => Task.FromResult<OrdemServico?>(_ordem.Id == Convert.ToInt32(id) ? _ordem : null);
        public Task<IReadOnlyList<OrdemServico>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<OrdemServico>>([_ordem]);
        public Task<IReadOnlyList<OrdemServico>> FindAsync(Expression<Func<OrdemServico, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<OrdemServico>>([_ordem]);
        public Task<bool> ExistsAsync(Expression<Func<OrdemServico, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task AddAsync(OrdemServico entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task AddRangeAsync(IEnumerable<OrdemServico> entities, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Update(OrdemServico entity) { }
        public void Remove(OrdemServico entity) { }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
