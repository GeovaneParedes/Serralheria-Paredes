using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serralheria_Paredes.Models.Constantes;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.Controllers;

[Authorize(Roles = PerfisConstantes.Administrador + "," + PerfisConstantes.Atendente)]
public class NotasFiscaisController : Controller
{
    private readonly INotaFiscalImportService _service;

    public NotasFiscaisController(INotaFiscalImportService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Importar() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Importar(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Selecione um PDF de nota fiscal.");
            return View();
        }

        await using var stream = arquivo.OpenReadStream();
        var result = await _service.AnalisarAsync(stream, arquivo.FileName, HttpContext.RequestAborted);
        if (!result.Success)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return View();
        }

        return View(result.Data);
    }
}
