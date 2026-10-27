using Microsoft.AspNetCore.Identity;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Constantes;

namespace Serralheria_Paredes.Data.Seeds;

/// <summary>
/// Seeder inicial para criação dos perfis padrão (Roles) e do usuário Administrador master.
/// </summary>
public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Perfil>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentityDataSeeder");

        // 1. Criar Perfis Padrão se não existirem
        foreach (var roleName in PerfisConstantes.Todos)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new Perfil(roleName, $"Perfil de {roleName} da Serralheria-Paredes");
                await roleManager.CreateAsync(role);
                logger.LogInformation("Perfil criado com sucesso: {Role}", roleName);
            }
        }

        // 2. Criar Usuário Administrador padrão se nenhum usuário existir
        var adminEmail = configuration["AdminSeed:Email"];
        var defaultPassword = configuration["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(defaultPassword))
        {
            logger.LogWarning("Administrador inicial não criado: AdminSeed:Email e AdminSeed:Password não configurados.");
            return;
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                NomeCompleto = "Administrador do Sistema",
                Cargo = "Gerente Geral",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, defaultPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, PerfisConstantes.Administrador);
                logger.LogInformation("Usuário Administrador padrão criado com sucesso: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Falha ao criar o Administrador padrão: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
