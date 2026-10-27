using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serralheria_Paredes.Models.Constantes;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Auth;

namespace Serralheria_Paredes.Controllers;

/// <summary>
/// Controller estritamente magro (Thin Controller) para o fluxo de autenticação e gestão de usuários.
/// Todas as regras de negócio residem no IAuthService.
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model, HttpContext.RequestAborted);
        if (!result.Success)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message ?? "Bem-vindo de volta!";

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        TempData["InfoMessage"] = "Sua sessão foi encerrada com segurança.";
        return RedirectToAction("Login", "Auth");
    }

    [HttpGet]
    [Authorize(Roles = PerfisConstantes.Administrador)]
    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await _authService.ListarUsuariosAsync(HttpContext.RequestAborted);
        return View(usuarios);
    }

    [HttpGet]
    [Authorize(Roles = PerfisConstantes.Administrador)]
    public IActionResult NovoUsuario()
    {
        ViewBag.Perfis = PerfisConstantes.Todos;
        return View(new NovoUsuarioViewModel());
    }

    [HttpPost]
    [Authorize(Roles = PerfisConstantes.Administrador)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NovoUsuario(NovoUsuarioViewModel model)
    {
        ViewBag.Perfis = PerfisConstantes.Todos;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegistrarUsuarioAsync(model, HttpContext.RequestAborted);
        if (!result.Success)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        TempData["SuccessMessage"] = result.Message ?? "Usuário criado com sucesso!";
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpPost]
    [Authorize(Roles = PerfisConstantes.Administrador)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlternarStatus(int id, bool ativo)
    {
        var result = await _authService.AlterarStatusUsuarioAsync(id, ativo, HttpContext.RequestAborted);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors);
        }
        else
        {
            TempData["SuccessMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Usuarios));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AcessoNegado()
    {
        return View();
    }
}
