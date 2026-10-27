using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.Services;

public class OrcamentoService : IOrcamentoService
{
    private readonly IOrcamentoRepository _repository;
    private readonly ILogger<OrcamentoService> _logger;

    public OrcamentoService(IOrcamentoRepository repository, ILogger<OrcamentoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<OrcamentoResumo>> ListarAsync(CancellationToken cancellationToken = default) =>
        (await _repository.ObterTodosAsync(cancellationToken)).Select(Mapear).ToList();

    public async Task<OperationResult<OrcamentoResumo>> CriarAsync(OrcamentoCadastro cadastro, CancellationToken cancellationToken = default)
    {
        if (cadastro.ClienteId <= 0 || string.IsNullOrWhiteSpace(cadastro.Titulo))
            return OperationResult<OrcamentoResumo>.Fail("Cliente e título são obrigatórios.");
        if (cadastro.ValidoAte.Date < DateTime.UtcNow.Date)
            return OperationResult<OrcamentoResumo>.Fail("A validade não pode estar no passado.");

        var calculo = OrcamentoCalculator.Calcular(
            cadastro.Itens.Select(i => new ItemCalculoOrcamento(i.Descricao, i.Quantidade, i.ComprimentoUnitarioMetros, i.PercentualPerda, i.CustoUnitario)),
            cadastro.ValorMaoDeObra,
            cadastro.PercentualMargem);
        if (!calculo.Success) return OperationResult<OrcamentoResumo>.Fail(calculo.Errors);

        var numero = $"ORC-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        while (await _repository.NumeroJaCadastradoAsync(numero, cancellationToken))
            numero = $"ORC-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Random.Shared.Next(10, 99)}";

        var resultado = calculo.Data!;
        var orcamento = new Orcamento
        {
            Numero = numero,
            ClienteId = cadastro.ClienteId,
            Titulo = cadastro.Titulo.Trim(),
            Descricao = cadastro.Descricao?.Trim(),
            ValidoAte = cadastro.ValidoAte,
            SubtotalMateriais = resultado.SubtotalMateriais,
            ValorMaoDeObra = resultado.ValorMaoDeObra,
            PercentualMargem = resultado.PercentualMargem,
            ValorTotal = resultado.ValorTotal,
            Itens = cadastro.Itens.Select((item, index) => new ItemOrcamento
            {
                InsumoId = item.InsumoId,
                Descricao = item.Descricao.Trim(),
                Unidade = item.Unidade,
                Quantidade = item.Quantidade,
                ComprimentoUnitarioMetros = item.ComprimentoUnitarioMetros,
                PercentualPerda = item.PercentualPerda,
                CustoUnitario = item.CustoUnitario,
                Subtotal = resultado.SubtotaisDosItens[index]
            }).ToList()
        };

        await _repository.AddAsync(orcamento, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Orçamento criado: {Numero}", numero);
        return OperationResult<OrcamentoResumo>.Ok(Mapear(orcamento), "Orçamento criado com sucesso.");
    }

    public async Task<OperationResult<OrcamentoResumo>> AprovarAsync(int id, CancellationToken cancellationToken = default)
    {
        var orcamento = await _repository.ObterComItensAsync(id, cancellationToken);
        if (orcamento == null) return OperationResult<OrcamentoResumo>.Fail("Orçamento não encontrado.");
        if (orcamento.Status == StatusOrcamento.Recusado || orcamento.Status == StatusOrcamento.Expirado)
            return OperationResult<OrcamentoResumo>.Fail("Este orçamento não pode ser aprovado.");
        if (orcamento.OrdemServico != null)
            return OperationResult<OrcamentoResumo>.Fail("Este orçamento já possui uma ordem de serviço.");

        orcamento.Status = StatusOrcamento.Aprovado;
        orcamento.OrdemServico = new OrdemServico
        {
            Numero = $"OS-{orcamento.Numero[4..]}",
            OrcamentoId = orcamento.Id,
            Status = StatusOrdemServico.AguardandoProducao
        };
        _repository.Update(orcamento);
        await _repository.SaveChangesAsync(cancellationToken);
        return OperationResult<OrcamentoResumo>.Ok(Mapear(orcamento), "Orçamento aprovado e ordem de serviço criada.");
    }

    private static OrcamentoResumo Mapear(Orcamento orcamento) => new(
        orcamento.Id,
        orcamento.Numero,
        orcamento.Cliente?.NomeOuRazaoSocial ?? "Cliente não carregado",
        orcamento.Titulo,
        orcamento.Status,
        orcamento.ValorTotal,
        orcamento.ValidoAte,
        orcamento.OrdemServico != null);
}
