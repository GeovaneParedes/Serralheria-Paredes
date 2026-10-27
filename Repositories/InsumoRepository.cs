using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class InsumoRepository : Repository<Insumo>, IInsumoRepository
{
    public InsumoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Insumo>> ObterTodosComFiltroAsync(string? termo, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Include(i => i.Fornecedor).AsQueryable();
        if (!string.IsNullOrWhiteSpace(termo))
        {
            var busca = termo.Trim();
            query = query.Where(i => EF.Functions.ILike(i.Codigo, $"%{busca}%") ||
                                     EF.Functions.ILike(i.Descricao, $"%{busca}%"));
        }

        return await query.OrderBy(i => i.Descricao).ToListAsync(cancellationToken);
    }

    public Task<bool> CodigoJaCadastradoAsync(string codigo, int? ignorarId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(i => i.Codigo == codigo);
        if (ignorarId.HasValue)
        {
            query = query.Where(i => i.Id != ignorarId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }
}
