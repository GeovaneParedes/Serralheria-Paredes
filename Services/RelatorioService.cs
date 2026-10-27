using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Application.Reports;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IRelatorioRepository _repository;

    public RelatorioService(IRelatorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<RelatorioViewModel> ObterAsync(DateTime? inicio = null, DateTime? fim = null, CancellationToken cancellationToken = default)
    {
        var dataFim = (fim ?? DateTime.UtcNow.Date).Date;
        var dataInicio = (inicio ?? dataFim.AddDays(-29)).Date;
        if (dataInicio >= dataFim)
        {
            dataInicio = dataFim.AddDays(-30);
        }

        var consulta = await _repository.ObterConsultaAsync(dataInicio, dataFim.AddDays(1), cancellationToken);
        return new RelatorioViewModel(
            dataInicio,
            dataFim,
            consulta.ClientesAtivos,
            consulta.FornecedoresAtivos,
            consulta.InsumosAbaixoDoMinimo,
            consulta.ValorOrcamentosAprovados,
            consulta.OrcamentosNoPeriodo,
            consulta.OrdensPorStatus,
            consulta.ConsumoPorInsumo);
    }
}
