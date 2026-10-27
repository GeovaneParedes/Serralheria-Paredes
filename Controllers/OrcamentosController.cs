using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Orcamentos;

namespace Serralheria_Paredes.Controllers;

[Authorize]
public class OrcamentosController : Controller
{
    private readonly IOrcamentoService _orcamentoService;
    private readonly IClienteService _clienteService;
    private readonly IInsumoService _insumoService;

    public OrcamentosController(IOrcamentoService orcamentoService, IClienteService clienteService, IInsumoService insumoService)
    {
        _orcamentoService = orcamentoService;
        _clienteService = clienteService;
        _insumoService = insumoService;
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

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var result = await _orcamentoService.ObterDetalhesAsync(id, HttpContext.RequestAborted);
        if (!result.Success) return NotFound();
        return View(result.Data);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarStatus(int id, StatusOrdemServico status)
    {
        var result = await _orcamentoService.AtualizarStatusOrdemAsync(id, status, HttpContext.RequestAborted);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? result.Message : string.Join("; ", result.Errors);
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Consumir(int id, int insumoId, decimal quantidade)
    {
        var usuarioId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsed) ? parsed : (int?)null;
        var result = await _insumoService.ConsumirParaOrdemAsync(id, insumoId, quantidade, usuarioId, HttpContext.RequestAborted);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? result.Message : string.Join("; ", result.Errors);
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    private async Task CarregarClientesAsync()
    {
        ViewBag.Clientes = await _clienteService.ListarAsync(null, HttpContext.RequestAborted);
    }
}
