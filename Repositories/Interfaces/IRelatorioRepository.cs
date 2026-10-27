using Serralheria_Paredes.Application.Reports;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IRelatorioRepository
{
    Task<RelatorioConsulta> ObterConsultaAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
}
