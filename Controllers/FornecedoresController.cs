using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serralheria_Paredes.Models.Constantes;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Cadastros;

namespace Serralheria_Paredes.Controllers;

[Authorize(Roles = PerfisConstantes.Administrador + "," + PerfisConstantes.Atendente)]
public class FornecedoresController : Controller
{
    private readonly IFornecedorService _fornecedorService;

    public FornecedoresController(IFornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService ?? throw new ArgumentNullException(nameof(fornecedorService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? termo, int pagina = 1, int tamanho = 20)
    {
        ViewBag.TermoBusca = termo;
        var fornecedores = await _fornecedorService.ListarAsync(termo, pagina, tamanho, HttpContext.RequestAborted);
        ViewBag.Paginacao = fornecedores;
        return View(fornecedores.Items);
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var result = await _fornecedorService.ObterPorIdAsync(id, HttpContext.RequestAborted);
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
        return View("Form", new FornecedorFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(FornecedorFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        var result = await _fornecedorService.CriarAsync(model, HttpContext.RequestAborted);
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
        var result = await _fornecedorService.ObterParaEdicaoAsync(id, HttpContext.RequestAborted);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = string.Join("; ", result.Errors);
            return RedirectToAction(nameof(Index));
        }

        return View("Form", result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Atualizar(int id, FornecedorFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", model);
        }

        var result = await _fornecedorService.AtualizarAsync(id, model, HttpContext.RequestAborted);
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
        var result = await _fornecedorService.AlternarStatusAsync(id, ativo, HttpContext.RequestAborted);
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
