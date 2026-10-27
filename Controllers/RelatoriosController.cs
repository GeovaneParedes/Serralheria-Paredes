using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serralheria_Paredes.Application.Reports;
using Serralheria_Paredes.Models.Constantes;
using Serralheria_Paredes.Services.Interfaces;

namespace Serralheria_Paredes.Controllers;

[Authorize(Roles = PerfisConstantes.Administrador + "," + PerfisConstantes.Atendente + "," + PerfisConstantes.Serralheiro)]
public class RelatoriosController : Controller
{
    private readonly IRelatorioService _service;

    public RelatoriosController(IRelatorioService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? inicio, DateTime? fim)
    {
        var relatorio = await _service.ObterAsync(inicio, fim, HttpContext.RequestAborted);
        return View(relatorio);
    }

    [HttpGet]
    public async Task<IActionResult> Csv(DateTime? inicio, DateTime? fim)
    {
        var relatorio = await _service.ObterAsync(inicio, fim, HttpContext.RequestAborted);
        return File(RelatorioCsvExporter.Exportar(relatorio), "text/csv", $"relatorio-{relatorio.Inicio:yyyyMMdd}-{relatorio.Fim:yyyyMMdd}.csv");
    }
}
