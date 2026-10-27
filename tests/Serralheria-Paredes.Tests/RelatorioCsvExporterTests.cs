using Serralheria_Paredes.Application.Reports;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class RelatorioCsvExporterTests
{
    [Fact]
    public void Exportar_deve_gerar_csv_utf8_com_consumo_e_indicadores()
    {
        var relatorio = new RelatorioViewModel(
            new DateTime(2026, 10, 1),
            new DateTime(2026, 10, 31),
            2,
            1,
            3,
            1250.50m,
            4,
            [],
            [new ConsumoInsumoResumo("Perfil;25\"", 2.5m)]);

        var csv = System.Text.Encoding.UTF8.GetString(RelatorioCsvExporter.Exportar(relatorio));

        Assert.Contains("Clientes ativos;2", csv);
        Assert.Contains("Perfil;25", csv);
        Assert.Contains("2.500", csv);
    }
}
