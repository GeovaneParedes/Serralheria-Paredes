using Serralheria_Paredes.Models.Enums;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IRelatorioRepository
{
    Task<RelatorioConsulta> ObterConsultaAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
}

public sealed record RelatorioConsulta(
    int ClientesAtivos,
    int FornecedoresAtivos,
    int InsumosAbaixoDoMinimo,
    decimal ValorOrcamentosAprovados,
    int OrcamentosNoPeriodo,
    IReadOnlyList<StatusOrdemResumo> OrdensPorStatus,
    IReadOnlyList<ConsumoInsumoResumo> ConsumoPorInsumo);

public sealed record StatusOrdemResumo(StatusOrdemServico Status, int Quantidade);
public sealed record ConsumoInsumoResumo(string Insumo, decimal Quantidade);
