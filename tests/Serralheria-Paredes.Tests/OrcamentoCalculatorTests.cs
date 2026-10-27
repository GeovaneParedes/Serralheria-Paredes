using Serralheria_Paredes.Services;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class OrcamentoCalculatorTests
{
    [Fact]
    public void Calcular_deve_aplicar_perda_margem_e_mao_de_obra()
    {
        var result = OrcamentoCalculator.Calcular(
            [new ItemCalculoOrcamento("Perfil", 10, 2, 10, 5)],
            100,
            20);

        Assert.True(result.Success);
        Assert.Equal(110, result.Data!.SubtotalMateriais);
        Assert.Equal(100, result.Data.ValorMaoDeObra);
        Assert.Equal(42, result.Data.ValorMargem);
        Assert.Equal(252, result.Data.ValorTotal);
    }

    [Fact]
    public void Calcular_deve_rejeitar_item_sem_quantidade()
    {
        var result = OrcamentoCalculator.Calcular(
            [new ItemCalculoOrcamento("Perfil", 0, 2, 0, 5)],
            0,
            0);

        Assert.False(result.Success);
        Assert.Contains("valores válidos", result.Errors.Single());
    }
}
