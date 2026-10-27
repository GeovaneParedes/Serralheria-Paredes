namespace Serralheria_Paredes.Services.Interfaces;

/// <summary>
/// Interface marcadora base para a camada de serviços da Serralheria-Paredes.
/// Todas as regras de negócio de módulos específicos (Clientes, Orçamentos, Ordens de Serviço, Estoque)
/// devem ser expostas através de interfaces filhas registradas via Injeção de Dependência.
/// </summary>
public interface IService
{
}
