using Serralheria_Paredes.Application.Reports;

namespace Serralheria_Paredes.Services.Interfaces;

public interface IRelatorioService : IService
{
    Task<RelatorioViewModel> ObterAsync(DateTime? inicio = null, DateTime? fim = null, CancellationToken cancellationToken = default);
}
