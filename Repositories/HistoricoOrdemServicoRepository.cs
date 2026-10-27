using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class HistoricoOrdemServicoRepository : Repository<HistoricoOrdemServico>, IHistoricoOrdemServicoRepository
{
    public HistoricoOrdemServicoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<HistoricoOrdemServico>> ObterPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().Where(h => h.OrdemServicoId == ordemServicoId).OrderByDescending(h => h.CreatedAt).ToListAsync(cancellationToken);
}
