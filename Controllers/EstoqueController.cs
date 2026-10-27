using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serralheria_Paredes.Models.Constantes;
using System.Security.Claims;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Estoque;

namespace Serralheria_Paredes.Controllers;

[Authorize(Roles = PerfisConstantes.Administrador + "," + PerfisConstantes.Serralheiro)]
public class EstoqueController : Controller
{
    private readonly IInsumoService _insumoService;

    public EstoqueController(IInsumoService insumoService)
    {
        _insumoService = insumoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? termo)
    {
        var insumos = await _insumoService.ListarAsync(termo, HttpContext.RequestAborted);
        return View(new EstoqueIndexViewModel { Insumos = insumos, Termo = termo });
    }

    [HttpGet]
    public async Task<IActionResult> Movimentar(int id)
    {
        var insumo = (await _insumoService.ListarAsync(null, HttpContext.RequestAborted))
            .FirstOrDefault(i => i.Id == id);
        if (insumo == null)
        {
            TempData["ErrorMessage"] = "Insumo não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        return View(new MovimentacaoEstoqueFormViewModel
        {
            InsumoId = insumo.Id,
            InsumoDescricao = $"{insumo.Codigo} - {insumo.Descricao}",
            SaldoAtual = insumo.EstoqueAtual,
            Unidade = insumo.Unidade
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Movimentar(MovimentacaoEstoqueFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _insumoService.MovimentarAsync(
            model.InsumoId,
            model.Tipo,
            model.Quantidade,
            model.Motivo,
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) ? usuarioId : null,
            cancellationToken: HttpContext.RequestAborted);
        if (!result.Success)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Historico(int? insumoId)
    {
        var historico = await _insumoService.HistoricoAsync(insumoId, HttpContext.RequestAborted);
        return View(historico);
    }
}
