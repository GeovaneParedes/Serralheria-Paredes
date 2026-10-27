using Serralheria_Paredes.Services.Common;

namespace Serralheria_Paredes.Services;

public sealed record ItemCalculoOrcamento(
    string Descricao,
    decimal Quantidade,
    decimal ComprimentoUnitarioMetros,
    decimal PercentualPerda,
    decimal CustoUnitario);

public sealed record ResultadoCalculoOrcamento(
    decimal SubtotalMateriais,
    decimal ValorMaoDeObra,
    decimal PercentualMargem,
    decimal ValorMargem,
    decimal ValorTotal,
    IReadOnlyList<decimal> SubtotaisDosItens);

public static class OrcamentoCalculator
{
    public static OperationResult<ResultadoCalculoOrcamento> Calcular(
        IEnumerable<ItemCalculoOrcamento> itens,
        decimal valorMaoDeObra,
        decimal percentualMargem)
    {
        if (valorMaoDeObra < 0 || percentualMargem < 0)
            return OperationResult<ResultadoCalculoOrcamento>.Fail("Mão de obra e margem não podem ser negativas.");

        var itensLista = itens.ToList();
        if (itensLista.Count == 0)
            return OperationResult<ResultadoCalculoOrcamento>.Fail("O orçamento deve possuir ao menos um item.");

        var subtotais = new List<decimal>(itensLista.Count);
        foreach (var item in itensLista)
        {
            if (string.IsNullOrWhiteSpace(item.Descricao) || item.Quantidade <= 0 ||
                item.ComprimentoUnitarioMetros < 0 || item.PercentualPerda < 0 || item.CustoUnitario < 0)
                return OperationResult<ResultadoCalculoOrcamento>.Fail("Todos os itens devem possuir descrição e valores válidos.");

            var metragem = item.Quantidade * Math.Max(item.ComprimentoUnitarioMetros, 1m);
            var fatorPerda = 1m + item.PercentualPerda / 100m;
            subtotais.Add(Math.Round(metragem * fatorPerda * item.CustoUnitario, 2));
        }

        var subtotalMateriais = subtotais.Sum();
        var baseMargem = subtotalMateriais + valorMaoDeObra;
        var valorMargem = Math.Round(baseMargem * percentualMargem / 100m, 2);
        var total = baseMargem + valorMargem;
        return OperationResult<ResultadoCalculoOrcamento>.Ok(new ResultadoCalculoOrcamento(
            subtotalMateriais,
            valorMaoDeObra,
            percentualMargem,
            valorMargem,
            Math.Round(total, 2),
            subtotais));
    }
}
