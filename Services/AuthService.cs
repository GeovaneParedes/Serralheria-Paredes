using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Constantes;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Auth;

namespace Serralheria_Paredes.Services;

/// <summary>
/// Implementação das regras de negócio de autenticação e gestão de usuários.
/// Desacopla 100% os Controllers do ASP.NET Core Identity e provê consultas otimizadas sem tracking.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly RoleManager<Perfil> _roleManager;
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        RoleManager<Perfil> roleManager,
        AppDbContext context,
        ILogger<AuthService> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OperationResult<UsuarioViewModel>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tentativa de login para o e-mail: {Email}", model.Email);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            _logger.LogWarning("Tentativa de login falhou: e-mail {Email} não encontrado.", model.Email);
            return OperationResult<UsuarioViewModel>.Fail("E-mail ou senha inválidos.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Tentativa de login bloqueada: usuário {Email} está desativado.", model.Email);
            return OperationResult<UsuarioViewModel>.Fail("Este usuário está inativo. Contate o administrador.");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Senha,
            isPersistent: model.LembrarMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _logger.LogWarning("Conta temporariamente bloqueada por excesso de tentativas: {Email}", model.Email);
            return OperationResult<UsuarioViewModel>.Fail("Conta temporariamente bloqueada por tentativas incorretas. Tente novamente em alguns minutos.");
        }

        if (!result.Succeeded)
        {
            _logger.LogWarning("Senha incorreta para o e-mail {Email}", model.Email);
            return OperationResult<UsuarioViewModel>.Fail("E-mail ou senha inválidos.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var perfilPrincipal = roles.FirstOrDefault() ?? "Sem Perfil";

        var viewModel = new UsuarioViewModel
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            Email = user.Email ?? string.Empty,
            Cargo = user.Cargo,
            Perfil = perfilPrincipal,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        _logger.LogInformation("Login bem-sucedido: Usuário {Email} com perfil {Perfil}", user.Email, perfilPrincipal);
        return OperationResult<UsuarioViewModel>.Ok(viewModel, "Login realizado com sucesso.");
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Sessão de usuário encerrada via SignOutAsync.");
    }

    public async Task<OperationResult<UsuarioViewModel>> RegistrarUsuarioAsync(NovoUsuarioViewModel model, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Solicitação de cadastro de novo usuário: {Email} com perfil {Perfil}", model.Email, model.Perfil);

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return OperationResult<UsuarioViewModel>.Fail("Já existe um usuário cadastrado com este e-mail.");
        }

        if (!PerfisConstantes.Todos.Contains(model.Perfil))
        {
            return OperationResult<UsuarioViewModel>.Fail($"Perfil de acesso '{model.Perfil}' é inválido.");
        }

        var user = new Usuario
        {
            UserName = model.Email,
            Email = model.Email,
            NomeCompleto = model.NomeCompleto,
            Cargo = model.Cargo,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        // Garante atomicidade: se a atribuição do perfil falhar, o usuário não é persistido isolado
        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(cancellationToken)
            : null;

        try
        {
            var createResult = await _userManager.CreateAsync(user, model.Senha);
            if (!createResult.Succeeded)
            {
                if (transaction != null) await transaction.RollbackAsync(cancellationToken);
                var errors = createResult.Errors.Select(e => e.Description);
                _logger.LogWarning("Falha ao criar usuário {Email}: {Errors}", model.Email, string.Join(", ", errors));
                return OperationResult<UsuarioViewModel>.Fail(errors);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.Perfil);
            if (!addRoleResult.Succeeded)
            {
                if (transaction != null) await transaction.RollbackAsync(cancellationToken);
                var errors = addRoleResult.Errors.Select(e => e.Description);
                _logger.LogError("Usuário {Email} criado, mas falhou ao atribuir perfil {Perfil}: {Errors}", model.Email, model.Perfil, string.Join(", ", errors));
                return OperationResult<UsuarioViewModel>.Fail(errors);
            }

            if (transaction != null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            if (transaction != null) await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erro inesperado ao registrar usuário {Email}", model.Email);
            throw;
        }

        var viewModel = new UsuarioViewModel
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            Email = user.Email,
            Cargo = user.Cargo,
            Perfil = model.Perfil,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        _logger.LogInformation("Novo usuário cadastrado com sucesso: {Email} (ID: {Id})", user.Email, user.Id);
        return OperationResult<UsuarioViewModel>.Ok(viewModel, "Usuário cadastrado com sucesso.");
    }

    public async Task<OperationResult<UsuarioViewModel>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var viewModel = await (from u in _context.Users.AsNoTracking()
                               where u.Id == id
                               join ur in _context.UserRoles.AsNoTracking() on u.Id equals ur.UserId into userRoles
                               from ur in userRoles.DefaultIfEmpty()
                               join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id into roles
                               from r in roles.DefaultIfEmpty()
                               select new UsuarioViewModel
                               {
                                   Id = u.Id,
                                   NomeCompleto = u.NomeCompleto,
                                   Email = u.Email ?? string.Empty,
                                   Cargo = u.Cargo,
                                   Perfil = r != null ? r.Name! : "Sem Perfil",
                                   IsActive = u.IsActive,
                                   CreatedAt = u.CreatedAt
                               }).FirstOrDefaultAsync(cancellationToken);

        if (viewModel == null)
        {
            return OperationResult<UsuarioViewModel>.Fail("Usuário não encontrado.");
        }

        return OperationResult<UsuarioViewModel>.Ok(viewModel);
    }

    public async Task<IReadOnlyList<UsuarioViewModel>> ListarUsuariosAsync(CancellationToken cancellationToken = default)
    {
        // Projeção única em SQL evitando o problema de N+1 consultas
        return await (from u in _context.Users.AsNoTracking()
                      join ur in _context.UserRoles.AsNoTracking() on u.Id equals ur.UserId into userRoles
                      from ur in userRoles.DefaultIfEmpty()
                      join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id into roles
                      from r in roles.DefaultIfEmpty()
                      orderby u.CreatedAt descending
                      select new UsuarioViewModel
                      {
                          Id = u.Id,
                          NomeCompleto = u.NomeCompleto,
                          Email = u.Email ?? string.Empty,
                          Cargo = u.Cargo,
                          Perfil = r != null ? r.Name! : "Sem Perfil",
                          IsActive = u.IsActive,
                          CreatedAt = u.CreatedAt
                      }).ToListAsync(cancellationToken);
    }

    public async Task<OperationResult> AlterarStatusUsuarioAsync(int id, bool ativo, int? executanteId = null, CancellationToken cancellationToken = default)
    {
        // Regra defensiva de segurança: impede auto-desativação do administrador logado
        if (executanteId.HasValue && executanteId.Value == id && !ativo)
        {
            _logger.LogWarning("Tentativa de auto-desativação bloqueada para o usuário ID {Id}", id);
            return OperationResult.Fail("Operação não permitida: você não pode desativar seu próprio usuário administrador.");
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return OperationResult.Fail("Usuário não encontrado.");
        }

        user.IsActive = ativo;
        user.UpdatedAt = DateTime.UtcNow;

        // Ao desativar, atualiza o SecurityStamp para invalidar cookies de sessão ativos imediatamente
        if (!ativo)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return OperationResult.Fail(result.Errors.Select(e => e.Description));
        }

        _logger.LogInformation("Status do usuário {Email} alterado para {Status}.", user.Email, ativo ? "Ativo" : "Inativo");
        return OperationResult.Ok($"Usuário {(ativo ? "ativado" : "desativado")} com sucesso.");
    }
}
