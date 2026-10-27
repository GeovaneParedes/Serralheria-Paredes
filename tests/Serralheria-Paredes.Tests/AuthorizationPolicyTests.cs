using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Serralheria_Paredes.Controllers;
using Serralheria_Paredes.Models.Constantes;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class AuthorizationPolicyTests
{
    [Fact]
    public void Estoque_deve_ser_restrito_a_administrador_ou_serralheiro()
    {
        var attribute = Assert.Single(typeof(EstoqueController).GetCustomAttributes<AuthorizeAttribute>());

        Assert.Equal($"{PerfisConstantes.Administrador},{PerfisConstantes.Serralheiro}", attribute.Roles);
    }

    [Fact]
    public void Cadastros_devem_ser_restritos_a_administrador_ou_atendente()
    {
        var attribute = Assert.Single(typeof(ClientesController).GetCustomAttributes<AuthorizeAttribute>());
        var fornecedor = Assert.Single(typeof(FornecedoresController).GetCustomAttributes<AuthorizeAttribute>());

        Assert.Equal($"{PerfisConstantes.Administrador},{PerfisConstantes.Atendente}", attribute.Roles);
        Assert.Equal(attribute.Roles, fornecedor.Roles);
    }

    [Fact]
    public void Consumo_e_producao_devem_exigir_administrador_ou_serralheiro()
    {
        var method = typeof(OrcamentosController).GetMethod(nameof(OrcamentosController.Consumir));
        var attribute = Assert.Single(method!.GetCustomAttributes<AuthorizeAttribute>());

        Assert.Equal($"{PerfisConstantes.Administrador},{PerfisConstantes.Serralheiro}", attribute.Roles);
    }
}
