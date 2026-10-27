using Serralheria_Paredes.Common.Validation;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class DocumentosUtilsTests
{
    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    public void ValidarCpf_deve_aceitar_documento_valido(string cpf)
    {
        Assert.True(DocumentosUtils.ValidarCpf(cpf));
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("529.982.247-26")]
    public void ValidarCpf_deve_rejeitar_documento_invalido(string cpf)
    {
        Assert.False(DocumentosUtils.ValidarCpf(cpf));
    }

    [Fact]
    public void ValidarCnpj_deve_aceitar_documento_valido()
    {
        Assert.True(DocumentosUtils.ValidarCnpj("11.222.333/0001-81"));
    }

    [Fact]
    public void Formatar_deve_aplicar_mascara_conforme_o_tamanho()
    {
        Assert.Equal("529.982.247-25", DocumentosUtils.Formatar("52998224725"));
        Assert.Equal("11.222.333/0001-81", DocumentosUtils.Formatar("11222333000181"));
    }
}
