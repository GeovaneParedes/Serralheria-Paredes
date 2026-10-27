using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.Services;

public class InsumoService : IInsumoService
{
    private readonly IInsumoRepository _repository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
    private readonly IRepository<OrdemServico>? _ordemRepository;
    private readonly ILogger<InsumoService> _logger;

    public InsumoService(
        IInsumoRepository repository,
        IMovimentacaoEstoqueRepository movimentacaoRepository,
        ILogger<InsumoService> logger,
        IRepository<OrdemServico>? ordemRepository = null)
    {
        _repository = repository;
        _movimentacaoRepository = movimentacaoRepository;
        _logger = logger;
        _ordemRepository = ordemRepository;
    }

    public async Task<IReadOnlyList<InsumoResumo>> ListarAsync(string? termo = null, CancellationToken cancellationToken = default)
    {
        var insumos = await _repository.ObterTodosComFiltroAsync(termo, cancellationToken);
        return insumos.Select(Mapear).ToList();
    }

    public async Task<OperationResult<InsumoResumo>> CriarAsync(InsumoCadastro cadastro, CancellationToken cancellationToken = default)
    {
        var codigo = cadastro.Codigo.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(cadastro.Descricao))
            return OperationResult<InsumoResumo>.Fail("Código e descrição são obrigatórios.");
        if (cadastro.EstoqueMinimo < 0 || cadastro.CustoUnitario < 0 || cadastro.ComprimentoBarraMetros < 0)
            return OperationResult<InsumoResumo>.Fail("Valores de estoque, custo e comprimento não podem ser negativos.");
        if (await _repository.CodigoJaCadastradoAsync(codigo, null, cancellationToken))
            return OperationResult<InsumoResumo>.Fail("Já existe um insumo com este código.");

        var insumo = new Insumo
        {
            Codigo = codigo,
            Descricao = cadastro.Descricao.Trim(),
            Categoria = cadastro.Categoria,
            Unidade = cadastro.Unidade,
            ComprimentoBarraMetros = cadastro.ComprimentoBarraMetros,
            EstoqueMinimo = cadastro.EstoqueMinimo,
            CustoUnitario = cadastro.CustoUnitario,
            FornecedorId = cadastro.FornecedorId
        };
        await _repository.AddAsync(insumo, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Insumo criado: {Codigo}", insumo.Codigo);
        return OperationResult<InsumoResumo>.Ok(Mapear(insumo), "Insumo cadastrado com sucesso.");
    }

    public async Task<IReadOnlyList<MovimentacaoResumo>> HistoricoAsync(int? insumoId = null, CancellationToken cancellationToken = default)
    {
        var movimentacoes = await _movimentacaoRepository.ObterHistoricoAsync(insumoId, cancellationToken);
        return movimentacoes.Select(m => new MovimentacaoResumo(
            m.Id,
            m.InsumoId,
            m.Insumo.Descricao,
            m.Tipo,
            m.Quantidade,
            m.SaldoAnterior,
            m.SaldoPosterior,
            m.Motivo,
            m.CreatedAt)).ToList();
    }

    public async Task<OperationResult> MovimentarAsync(
        int id,
        TipoMovimentacaoEstoque tipo,
        decimal quantidade,
        string motivo,
        int? usuarioId = null,
        CancellationToken cancellationToken = default)
    {
        return await RegistrarMovimentacaoAsync(id, tipo, quantidade, motivo, usuarioId, null, cancellationToken);
    }

    public async Task<OperationResult> ConsumirParaOrdemAsync(int ordemServicoId, int insumoId, decimal quantidade, int? usuarioId = null, CancellationToken cancellationToken = default)
    {
        if (_ordemRepository == null) return OperationResult.Fail("Repositório de ordem de serviço não configurado.");
        var ordem = await _ordemRepository.GetByIdAsync(ordemServicoId, cancellationToken);
        if (ordem == null) return OperationResult.Fail("Ordem de serviço não encontrada.");
        if (ordem.Status is StatusOrdemServico.AguardandoProducao or StatusOrdemServico.Concluida or StatusOrdemServico.Cancelada)
            return OperationResult.Fail("A OS precisa estar em uma etapa de produção para consumir material.");

        return await RegistrarMovimentacaoAsync(insumoId, TipoMovimentacaoEstoque.Saida, quantidade,
            $"Consumo na OS {ordem.Numero}", usuarioId, ordemServicoId, cancellationToken);
    }

    private async Task<OperationResult> RegistrarMovimentacaoAsync(
        int id,
        TipoMovimentacaoEstoque tipo,
        decimal quantidade,
        string motivo,
        int? usuarioId,
        int? ordemServicoId,
        CancellationToken cancellationToken)
    {
        if (quantidade <= 0) return OperationResult.Fail("A quantidade deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(motivo)) return OperationResult.Fail("O motivo da movimentação é obrigatório.");
        if (!Enum.IsDefined(tipo)) return OperationResult.Fail("Tipo de movimentação inválido.");

        var insumo = await _repository.GetByIdAsync(id, cancellationToken);
        if (insumo == null) return OperationResult.Fail("Insumo não encontrado.");

        var saldoAnterior = insumo.EstoqueAtual;
        var saldoPosterior = tipo == TipoMovimentacaoEstoque.Saida
            ? saldoAnterior - quantidade
            : saldoAnterior + quantidade;
        if (saldoPosterior < 0) return OperationResult.Fail("O estoque não pode ficar negativo.");

        insumo.EstoqueAtual = saldoPosterior;
        insumo.UpdatedAt = DateTime.UtcNow;
        var movimentacao = new MovimentacaoEstoque
        {
            InsumoId = insumo.Id,
            Tipo = tipo,
            Quantidade = quantidade,
            SaldoAnterior = saldoAnterior,
            SaldoPosterior = saldoPosterior,
            Motivo = motivo.Trim(),
            UsuarioId = usuarioId,
            OrdemServicoId = ordemServicoId
        };

        _repository.Update(insumo);
        await _movimentacaoRepository.AddAsync(movimentacao, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Movimentação {Tipo} registrada para o insumo {InsumoId}: {Quantidade}", tipo, id, quantidade);
        return OperationResult.Ok("Movimentação registrada com sucesso.");
    }

    private static InsumoResumo Mapear(Insumo insumo) => new(
        insumo.Id, insumo.Codigo, insumo.Descricao, insumo.Categoria, insumo.Unidade,
        insumo.EstoqueAtual, insumo.EstoqueMinimo, insumo.CustoUnitario,
        insumo.EstoqueAtual <= insumo.EstoqueMinimo, insumo.Fornecedor?.NomeFantasia ?? insumo.Fornecedor?.RazaoSocial);
}
