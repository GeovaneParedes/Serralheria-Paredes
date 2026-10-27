using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Orcamentos;

namespace Serralheria_Paredes.Controllers;

[Authorize]
public class OrcamentosController : Controller
{
    private readonly IOrcamentoService _orcamentoService;
    private readonly IClienteService _clienteService;

    public OrcamentosController(IOrcamentoService orcamentoService, IClienteService clienteService)
    {
        _orcamentoService = orcamentoService;
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orcamentos = await _orcamentoService.ListarAsync(HttpContext.RequestAborted);
        return View(new OrcamentoIndexViewModel { Orcamentos = orcamentos });
    }

    [HttpGet]
    public async Task<IActionResult> Novo()
    {
        await CarregarClientesAsync();
        return View(new OrcamentoFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(OrcamentoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await CarregarClientesAsync();
            return View("Novo", model);
        }

        var result = await _orcamentoService.CriarAsync(model.ToCadastro(), HttpContext.RequestAborted);
        if (!result.Success)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            await CarregarClientesAsync();
            return View("Novo", model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Aprovar(int id)
    {
        var result = await _orcamentoService.AprovarAsync(id, HttpContext.RequestAborted);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success
            ? result.Message
            : string.Join("; ", result.Errors);
        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarClientesAsync()
    {
        ViewBag.Clientes = await _clienteService.ListarAsync(null, HttpContext.RequestAborted);
    }
}
