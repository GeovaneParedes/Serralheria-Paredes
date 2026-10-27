using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class MovimentacaoEstoqueRepository : Repository<MovimentacaoEstoque>, IMovimentacaoEstoqueRepository
{
    public MovimentacaoEstoqueRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<MovimentacaoEstoque>> ObterHistoricoAsync(int? insumoId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Include(m => m.Insumo).AsQueryable();
        if (insumoId.HasValue)
        {
            query = query.Where(m => m.InsumoId == insumoId.Value);
        }

        return await query.OrderByDescending(m => m.CreatedAt).ToListAsync(cancellationToken);
    }
}
