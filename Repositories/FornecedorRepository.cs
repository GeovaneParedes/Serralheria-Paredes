using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Common.Pagination;
using Serralheria_Paredes.Common.Validation;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class FornecedorRepository : Repository<Fornecedor>, IFornecedorRepository
{
    public FornecedorRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Fornecedor>> ObterTodosComFiltroAsync(string? termo, int pagina, int tamanho, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var termoLimpo = termo.Trim();
            var termoNumeros = DocumentosUtils.ApenasNumeros(termoLimpo);

            query = query.Where(f =>
                EF.Functions.ILike(f.RazaoSocial, $"%{termoLimpo}%") ||
                (f.NomeFantasia != null && EF.Functions.ILike(f.NomeFantasia, $"%{termoLimpo}%")) ||
                (f.ContatoResponsavel != null && EF.Functions.ILike(f.ContatoResponsavel, $"%{termoLimpo}%")) ||
                (f.Email != null && EF.Functions.ILike(f.Email, $"%{termoLimpo}%")) ||
                f.CpfOuCnpj.Contains(termoNumeros) ||
                f.Telefone.Contains(termoLimpo));
        }

        var total = await query.CountAsync(cancellationToken);
        var itens = await query.OrderBy(f => f.RazaoSocial).Skip((pagina - 1) * tamanho).Take(tamanho).ToListAsync(cancellationToken);
        return new PagedResult<Fornecedor>(itens, pagina, tamanho, total);
    }

    public async Task<bool> DocumentoJaCadastradoAsync(string cpfOuCnpj, int? ignorarId = null, CancellationToken cancellationToken = default)
    {
        var numeros = DocumentosUtils.ApenasNumeros(cpfOuCnpj);
        var query = _dbSet.Where(f => f.CpfOuCnpj == numeros);

        if (ignorarId.HasValue)
        {
            query = query.Where(f => f.Id != ignorarId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
