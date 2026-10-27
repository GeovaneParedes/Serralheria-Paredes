using Serralheria_Paredes.Application.NotasFiscais;
using Serralheria_Paredes.Services.Common;

namespace Serralheria_Paredes.Services.Interfaces;

public interface INotaFiscalImportService : IService
{
    Task<OperationResult<NotaFiscalImportada>> AnalisarAsync(Stream arquivo, string nomeArquivo, CancellationToken cancellationToken = default);
}
