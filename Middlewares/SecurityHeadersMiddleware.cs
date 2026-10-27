namespace Serralheria_Paredes.Middlewares;

/// <summary>
/// Middleware de endurecimento de segurança HTTP seguindo as diretrizes OWASP.
/// Injeta cabeçalhos defensivos contra Clickjacking, MIME sniffing e XSS.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            if (!headers.ContainsKey("X-Content-Type-Options"))
                headers.Append("X-Content-Type-Options", "nosniff");

            if (!headers.ContainsKey("X-Frame-Options"))
                headers.Append("X-Frame-Options", "DENY");

            if (!headers.ContainsKey("Referrer-Policy"))
                headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            if (!headers.ContainsKey("Permissions-Policy"))
                headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

            // Content-Security-Policy defensiva padrão MVC
            if (!headers.ContainsKey("Content-Security-Policy"))
            {
                headers.Append("Content-Security-Policy",
                    "default-src 'self'; " +
                    "img-src 'self' data:; " +
                    "script-src 'self' 'unsafe-inline'; " +
                    "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                    "font-src 'self' https://fonts.gstatic.com; " +
                    "frame-ancestors 'none'; " +
                    "form-action 'self';");
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
