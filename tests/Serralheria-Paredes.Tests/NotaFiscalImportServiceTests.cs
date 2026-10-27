using Serralheria_Paredes.Services;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class NotaFiscalImportServiceTests
{
    [Fact]
    public void AnalisarTexto_deve_extra_ir_dados_do_danfe_textual()
    {
        const string texto = """
        IMESUL METALURGICA LTDA
        Nº. 0488821 SÉRIE 3
        CHAVE DE ACESSO 5026 1003 7468 6400 0145 5500 3000 4888 2113 6234 5451
        CNPJ / CPF 03.746.864/0001-45
        VALOR TOTAL DA NOTA 244,88
        01014060079120 TUBO QUAD 20X20 #18 73089010 000 5101 KG 21,8935 9,50 207,99
        01094060476158 PERFIL CHATO 3/16 X 5/8 73089010 000 5101 KG 3,3539 10,99913533 36,89
        """;

        var result = NotaFiscalImportService.AnalisarTexto(texto, "NF10130488821.pdf");

        Assert.Equal("0488821", result.Numero);
        Assert.Equal("03746864000145", result.CnpjEmitente?.Replace(".", "").Replace("/", "").Replace("-", ""));
        Assert.Equal("50261003746864000145550030004888211362345451", result.ChaveAcesso);
        Assert.Equal(244.88m, result.ValorTotal);
        Assert.Equal(2, result.Itens.Count);
        Assert.True(result.RevisaoNecessaria);
    }
}
