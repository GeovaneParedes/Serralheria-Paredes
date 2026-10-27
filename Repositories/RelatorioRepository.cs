using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;

namespace Serralheria_Paredes.Repositories;

public class RelatorioRepository : IRelatorioRepository
{
    private readonly AppDbContext _context;

    public RelatorioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RelatorioConsulta> ObterConsultaAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
    {
        var ordens = _context.OrdensServico.AsNoTracking();
        var movimentacoes = _context.MovimentacoesEstoque.AsNoTracking();

        var status = await ordens.GroupBy(o => o.Status)
            .Select(g => new StatusOrdemResumo(g.Key, g.Count()))
            .OrderBy(x => x.Status)
            .ToListAsync(cancellationToken);

        var consumo = await movimentacoes
            .Where(m => m.Tipo == TipoMovimentacaoEstoque.Saida && m.CreatedAt >= inicio && m.CreatedAt < fim)
            .GroupBy(m => m.Insumo.Descricao)
            .Select(g => new ConsumoInsumoResumo(g.Key, g.Sum(m => m.Quantidade)))
            .OrderByDescending(x => x.Quantidade)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new RelatorioConsulta(
            await _context.Clientes.CountAsync(c => c.IsActive, cancellationToken),
            await _context.Fornecedores.CountAsync(f => f.IsActive, cancellationToken),
            await _context.Insumos.CountAsync(i => i.IsActive && i.EstoqueAtual <= i.EstoqueMinimo, cancellationToken),
            await _context.Orcamentos.Where(o => o.Status == StatusOrcamento.Aprovado && o.CreatedAt >= inicio && o.CreatedAt < fim).SumAsync(o => (decimal?)o.ValorTotal, cancellationToken) ?? 0,
            await _context.Orcamentos.CountAsync(o => o.CreatedAt >= inicio && o.CreatedAt < fim, cancellationToken),
            status,
            consumo);
    }
}
