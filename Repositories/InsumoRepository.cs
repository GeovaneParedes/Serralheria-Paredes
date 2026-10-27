using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Common.Pagination;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class InsumoRepository : Repository<Insumo>, IInsumoRepository
{
    public InsumoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Insumo>> ObterTodosComFiltroAsync(string? termo, int pagina, int tamanho, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Include(i => i.Fornecedor).AsQueryable();
        if (!string.IsNullOrWhiteSpace(termo))
        {
            var busca = termo.Trim();
            query = query.Where(i => EF.Functions.ILike(i.Codigo, $"%{busca}%") ||
                                     EF.Functions.ILike(i.Descricao, $"%{busca}%"));
        }

        var total = await query.CountAsync(cancellationToken);
        var itens = await query.OrderBy(i => i.Descricao).Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);
        return new PagedResult<Insumo>(itens, pagina, tamanho, total);
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
