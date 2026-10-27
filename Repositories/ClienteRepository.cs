using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Common.Pagination;
using Serralheria_Paredes.Common.Validation;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Cliente>> ObterTodosComFiltroAsync(string? termo, int pagina, int tamanho, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var termoLimpo = termo.Trim();
            var termoNumeros = DocumentosUtils.ApenasNumeros(termoLimpo);

            query = query.Where(c =>
                EF.Functions.ILike(c.NomeOuRazaoSocial, $"%{termoLimpo}%") ||
                (c.NomeFantasia != null && EF.Functions.ILike(c.NomeFantasia, $"%{termoLimpo}%")) ||
                (c.Email != null && EF.Functions.ILike(c.Email, $"%{termoLimpo}%")) ||
                c.CpfOuCnpj.Contains(termoNumeros) ||
                c.Telefone.Contains(termoLimpo));
        }

        var total = await query.CountAsync(cancellationToken);
        var itens = await query.OrderBy(c => c.NomeOuRazaoSocial).Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);
        return new PagedResult<Cliente>(itens, pagina, tamanho, total);
    }

    public async Task<bool> DocumentoJaCadastradoAsync(string cpfOuCnpj, int? ignorarId = null, CancellationToken cancellationToken = default)
    {
        var numeros = DocumentosUtils.ApenasNumeros(cpfOuCnpj);
        var query = _dbSet.Where(c => c.CpfOuCnpj == numeros);

        if (ignorarId.HasValue)
        {
            query = query.Where(c => c.Id != ignorarId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
