using System.Net;
using System.Text.Json;

namespace Serralheria_Paredes.Middlewares;

/// <summary>
/// Middleware global para captura e log estruturado de exceções não tratadas.
/// Evita vazamento de stacktrace para o cliente e registra telemetria detalhada.
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exceção não tratada capturada globalmente. Request: {Method} {Path} | TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("A resposta já havia sido iniciada, não foi possível modificar o status/body.");
            return;
        }

        // Se for requisição AJAX/API (JSON), retorna JSON RFC 7807 (ProblemDetails)
        var isApiOrAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
            || (context.Request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase));

        if (isApiOrAjax)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problemDetails = new
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Ocorreu um erro interno no servidor.",
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = context.Request.Path.Value,
                TraceId = context.TraceIdentifier,
                Detail = _env.IsDevelopment() ? exception.Message : "Por favor, entre em contato com o suporte com o TraceId fornecido."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
        else
        {
            // Requisição MVC tradicional via browser -> Redireciona para rota de erro amigável
            context.Response.Redirect($"/Home/Error?traceId={context.TraceIdentifier}");
        }
    }
}

public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
