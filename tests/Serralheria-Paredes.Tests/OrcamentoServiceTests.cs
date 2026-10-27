using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services;
using Serralheria_Paredes.Services.Interfaces;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class OrcamentoServiceTests
{
    [Fact]
    public async Task CriarAsync_deve_calcular_e_persistir_multiplos_itens()
    {
        var repository = new FakeOrcamentoRepository();
        var service = new OrcamentoService(repository, NullLogger<OrcamentoService>.Instance);
        var cadastro = new OrcamentoCadastro(
            7,
            "Portão de correr",
            null,
            DateTime.Today.AddDays(10),
            100,
            20,
            [
                new("Perfil", UnidadeMedida.Metro, 10, 2, 10, 5),
                new("Chapa", UnidadeMedida.Chapa, 2, 1, 0, 30)
            ]);

        var result = await service.CriarAsync(cadastro);

        Assert.True(result.Success);
        var orcamento = Assert.Single(repository.Items);
        Assert.Equal(2, orcamento.Itens.Count);
        Assert.Equal(324, orcamento.ValorTotal);
    }

    [Fact]
    public async Task AprovarAsync_deve_criar_ordem_de_servico_e_alterar_status()
    {
        var orcamento = new Orcamento
        {
            Id = 4,
            Numero = "ORC-20261027-01",
            ClienteId = 7,
            Titulo = "Portão",
            Status = StatusOrcamento.Rascunho,
            Itens = [new ItemOrcamento { Descricao = "Perfil", Quantidade = 1, CustoUnitario = 10 }]
        };
        var repository = new FakeOrcamentoRepository(orcamento);
        var service = new OrcamentoService(repository, NullLogger<OrcamentoService>.Instance);

        var result = await service.AprovarAsync(4);

        Assert.True(result.Success);
        Assert.Equal(StatusOrcamento.Aprovado, orcamento.Status);
        Assert.Equal("OS-20261027-01", orcamento.OrdemServico!.Numero);
        Assert.Equal(StatusOrdemServico.AguardandoProducao, orcamento.OrdemServico.Status);
    }

    private sealed class FakeOrcamentoRepository : IOrcamentoRepository
    {
        public List<Orcamento> Items { get; } = new();

        public FakeOrcamentoRepository(params Orcamento[] items) => Items.AddRange(items);
        public Task<Orcamento?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == Convert.ToInt32(id)));
        public Task<IReadOnlyList<Orcamento>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Orcamento>>(Items);
        public Task<IReadOnlyList<Orcamento>> FindAsync(Expression<Func<Orcamento, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Orcamento>>(Items.AsQueryable().Where(predicate).ToList());
        public Task<bool> ExistsAsync(Expression<Func<Orcamento, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult(Items.AsQueryable().Any(predicate));
        public Task AddAsync(Orcamento entity, CancellationToken cancellationToken = default) { entity.Id = Items.Count + 1; Items.Add(entity); return Task.CompletedTask; }
        public Task AddRangeAsync(IEnumerable<Orcamento> entities, CancellationToken cancellationToken = default) { Items.AddRange(entities); return Task.CompletedTask; }
        public void Update(Orcamento entity) { }
        public void Remove(Orcamento entity) => Items.Remove(entity);
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
        public Task<IReadOnlyList<Orcamento>> ObterTodosAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Orcamento>>(Items);
        public Task<Orcamento?> ObterComItensAsync(int id, CancellationToken cancellationToken = default) => Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
        public Task<bool> NumeroJaCadastradoAsync(string numero, CancellationToken cancellationToken = default) => Task.FromResult(Items.Any(x => x.Numero == numero));
    }
}
