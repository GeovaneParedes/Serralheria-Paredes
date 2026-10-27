using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Data.Seeds;
using Serralheria_Paredes.Middlewares;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Repositories;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services;
using Serralheria_Paredes.Services.Interfaces;

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

// 2. Configuração do ASP.NET Core Identity com PostgreSQL
builder.Services.AddIdentity<Usuario, Perfil>(options =>
{
    // Requisitos de Senha Forte
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Políticas de Bloqueio Defensivo contra Brute-Force
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Usuário
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 3. Configuração de Cookies de Autenticação Seguros
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-Serralheria.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AcessoNegado";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// 4. Injeção de Dependência das Camadas Base (Repositories & Services)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IFornecedorRepository, FornecedorRepository>();
builder.Services.AddScoped<IInsumoRepository, InsumoRepository>();
builder.Services.AddScoped<IMovimentacaoEstoqueRepository, MovimentacaoEstoqueRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<IInsumoService, InsumoService>();

// 5. Segurança: Proteção CSRF / Antiforgery com Cookies Seguros
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "__Host-Serralheria.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// 6. Configuração do Proxy Reverso (Nginx + Cloudflare)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// 7. Configuração de MVC com Controllers Magros
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

// VI. Inicialização do Seeder de Perfis e Administrador Padrão
try
{
    await IdentityDataSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Aviso: Banco de dados não conectado durante a inicialização para rodar o Seeder. Ele será executado na primeira conexão.");
}

app.Run();

public partial class Program
{
}
