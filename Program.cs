using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Middlewares;
using Serralheria_Paredes.Repositories;
using Serralheria_Paredes.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados PostgreSQL (Npgsql)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada em appsettings.json.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    }));

// 2. Injeção de Dependência das Camadas Base (Repository & UnitOfWork)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 3. Segurança: Proteção CSRF / Antiforgery e Cookies Seguros
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__Host-Serralheria.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// 4. Configuração do Proxy Reverso (Nginx + Cloudflare)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Permite que o Nginx em localhost repasse os cabeçalhos reais de IP e protocolo
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// 5. Configuração de MVC com Controllers Magros
builder.Services.AddControllersWithViews();

var app = builder.Build();

// PIPELINE HTTP

// I. Proxy Reverso deve ser o PRIMEIRO a processar para restaurar IP e Scheme (HTTPS)
app.UseForwardedHeaders();

// II. Tratamento Global de Exceções e Logs Estruturados
app.UseGlobalExceptionHandler();

// III. Headers de Segurança Web (OWASP)
app.UseSecurityHeaders();

if (!app.Environment.IsDevelopment())
{
    // HSTS defensivo para tráfego seguro
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// IV. Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

// V. Roteamento de Arquivos Estáticos e Controladores
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
