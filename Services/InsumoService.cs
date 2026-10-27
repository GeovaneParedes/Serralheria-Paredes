using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.Services;

public class InsumoService : IInsumoService
{
    private readonly IInsumoRepository _repository;
    private readonly ILogger<InsumoService> _logger;

    public InsumoService(IInsumoRepository repository, ILogger<InsumoService> logger)
    {
        _repository = repository;
        _logger = logger;
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

    public async Task<OperationResult> AjustarEstoqueAsync(int id, decimal quantidade, CancellationToken cancellationToken = default)
    {
        var insumo = await _repository.GetByIdAsync(id, cancellationToken);
        if (insumo == null) return OperationResult.Fail("Insumo não encontrado.");
        if (insumo.EstoqueAtual + quantidade < 0) return OperationResult.Fail("O estoque não pode ficar negativo.");

        insumo.EstoqueAtual += quantidade;
        insumo.UpdatedAt = DateTime.UtcNow;
        _repository.Update(insumo);
        await _repository.SaveChangesAsync(cancellationToken);
        return OperationResult.Ok("Estoque ajustado com sucesso.");
    }

    private static InsumoResumo Mapear(Insumo insumo) => new(
        insumo.Id, insumo.Codigo, insumo.Descricao, insumo.Categoria, insumo.Unidade,
        insumo.EstoqueAtual, insumo.EstoqueMinimo, insumo.CustoUnitario,
        insumo.EstoqueAtual <= insumo.EstoqueMinimo, insumo.Fornecedor?.NomeFantasia ?? insumo.Fornecedor?.RazaoSocial);
}
