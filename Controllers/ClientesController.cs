using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Cadastros;

namespace Serralheria_Paredes.Controllers;

[Authorize]
public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? termo)
    {
        ViewBag.TermoBusca = termo;
        var clientes = await _clienteService.ListarAsync(termo, HttpContext.RequestAborted);
        return View(clientes);
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var result = await _clienteService.ObterPorIdAsync(id, HttpContext.RequestAborted);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors);
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Novo()
    {
        return View("Form", new ClienteFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(ClienteFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        var result = await _clienteService.CriarAsync(model, HttpContext.RequestAborted);
        if (!result.Success)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View("Form", model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var result = await _clienteService.ObterParaEdicaoAsync(id, HttpContext.RequestAborted);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors);
            return RedirectToAction(nameof(Index));
        }

        return View("Form", result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Atualizar(int id, ClienteFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        var result = await _clienteService.AtualizarAsync(id, model, HttpContext.RequestAborted);
        if (!result.Success)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View("Form", model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlternarStatus(int id, bool ativo)
    {
        var result = await _clienteService.AlternarStatusAsync(id, ativo, HttpContext.RequestAborted);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors);
        }
        else
        {
            TempData["SuccessMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
