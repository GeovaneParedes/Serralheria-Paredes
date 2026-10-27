using Serralheria_Paredes.Models.Enums;
using System.Globalization;
using System.Text;

namespace Serralheria_Paredes.Application.Reports;

public sealed record RelatorioConsulta(
    int ClientesAtivos,
    int FornecedoresAtivos,
    int InsumosAbaixoDoMinimo,
    decimal ValorOrcamentosAprovados,
    int OrcamentosNoPeriodo,
    IReadOnlyList<StatusOrdemResumo> OrdensPorStatus,
    IReadOnlyList<ConsumoInsumoResumo> ConsumoPorInsumo);

public sealed record StatusOrdemResumo(StatusOrdemServico Status, int Quantidade);
public sealed record ConsumoInsumoResumo(string Insumo, decimal Quantidade);

public sealed record RelatorioViewModel(
    DateTime Inicio,
    DateTime Fim,
    int ClientesAtivos,
    int FornecedoresAtivos,
    int InsumosAbaixoDoMinimo,
    decimal ValorOrcamentosAprovados,
    int OrcamentosNoPeriodo,
    IReadOnlyList<StatusOrdemResumo> OrdensPorStatus,
    IReadOnlyList<ConsumoInsumoResumo> ConsumoPorInsumo);

public static class RelatorioCsvExporter
{
    public static byte[] Exportar(RelatorioViewModel relatorio)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Indicador;Valor");
        csv.AppendLine($"Periodo;{relatorio.Inicio:dd/MM/yyyy} a {relatorio.Fim:dd/MM/yyyy}");
        csv.AppendLine($"Clientes ativos;{relatorio.ClientesAtivos}");
        csv.AppendLine($"Fornecedores ativos;{relatorio.FornecedoresAtivos}");
        csv.AppendLine($"Insumos abaixo do minimo;{relatorio.InsumosAbaixoDoMinimo}");
        csv.AppendLine($"Orcamentos aprovados;{relatorio.ValorOrcamentosAprovados.ToString("F2", CultureInfo.InvariantCulture)}");
        csv.AppendLine();
        csv.AppendLine("Consumo por insumo;Quantidade");
        foreach (var item in relatorio.ConsumoPorInsumo)
        {
            csv.AppendLine($"{Escapar(item.Insumo)};{item.Quantidade.ToString("F3", CultureInfo.InvariantCulture)}");
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
    }

    private static string Escapar(string valor) => $"\"{valor.Replace("\"", "\"\"")}\"";
}
