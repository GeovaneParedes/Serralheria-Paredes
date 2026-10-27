using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class OrcamentoRepository : Repository<Orcamento>, IOrcamentoRepository
{
    public OrcamentoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Orcamento>> ObterTodosAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().Include(o => o.Cliente).OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);

    public async Task<Orcamento?> ObterComItensAsync(int id, CancellationToken cancellationToken = default) =>
        await _dbSet.Include(o => o.Cliente)
            .Include(o => o.Itens)
            .ThenInclude(i => i.Insumo)
            .Include(o => o.OrdemServico)
                .ThenInclude(o => o!.Historico)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<bool> NumeroJaCadastradoAsync(string numero, CancellationToken cancellationToken = default) =>
        _dbSet.AnyAsync(o => o.Numero == numero, cancellationToken);
}
