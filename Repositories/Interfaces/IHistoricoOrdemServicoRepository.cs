using Serralheria_Paredes.Models;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IHistoricoOrdemServicoRepository : IRepository<HistoricoOrdemServico>
{
    Task<IReadOnlyList<HistoricoOrdemServico>> ObterPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default);
}
