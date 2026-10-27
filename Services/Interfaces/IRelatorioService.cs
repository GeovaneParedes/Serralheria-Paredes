using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IRelatorioService : IService
{
    Task<RelatorioViewModel> ObterAsync(DateTime? inicio = null, DateTime? fim = null, CancellationToken cancellationToken = default);
}

public sealed record RelatorioViewModel(
    DateTime Inicio,
    DateTime Fim,
    int ClientesAtivos,
    int FornecedoresAtivos,
    int InsumosAbaixoDoMinimo,
    decimal ValorOrcamentosAprovados,
    int OrcamentosNoPeriodo,
    IReadOnlyList<StatusOrdemResumo> OrdensPorStatus,
    IReadOnlyList<ConsumoInsumoResumo> ConsumoPorInsumo);
