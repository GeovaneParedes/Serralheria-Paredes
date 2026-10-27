using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class RelatorioServiceTests
{
    [Fact]
    public async Task ObterAsync_deve_usar_janela_padrao_de_trinta_dias()
    {
        var repository = new FakeRelatorioRepository();
        var service = new RelatorioService(repository);

        var result = await service.ObterAsync(new DateTime(2026, 10, 27), new DateTime(2026, 10, 28));

        Assert.Equal(new DateTime(2026, 10, 27), result.Inicio);
        Assert.Equal(new DateTime(2026, 10, 28), result.Fim);
        Assert.Equal(12, result.ClientesAtivos);
        Assert.Equal(1500m, result.ValorOrcamentosAprovados);
        Assert.Equal(1, repository.Chamadas);
    }

    [Fact]
    public async Task ObterAsync_deve_corrigir_periodo_invertido()
    {
        var service = new RelatorioService(new FakeRelatorioRepository());

        var result = await service.ObterAsync(new DateTime(2026, 10, 28), new DateTime(2026, 10, 27));

        Assert.Equal(new DateTime(2026, 9, 27), result.Inicio);
        Assert.Equal(new DateTime(2026, 10, 27), result.Fim);
    }

    private sealed class FakeRelatorioRepository : IRelatorioRepository
    {
        public int Chamadas { get; private set; }

        public Task<RelatorioConsulta> ObterConsultaAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
        {
            Chamadas++;
            return Task.FromResult(new RelatorioConsulta(
                12,
                4,
                2,
                1500m,
                6,
                [new StatusOrdemResumo(StatusOrdemServico.EmCorte, 2)],
                [new ConsumoInsumoResumo("Perfil", 8)]));
        }
    }
}
