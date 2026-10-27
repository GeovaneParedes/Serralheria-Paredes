# SESSION_STATE - Serralheria-Paredes

## 1. O Que Foi Feito
- Inicializada a branch de feature `feature/arquitetura-base-infraestrutura`.
- Configurado arquivo `.gitignore` específico para ecossistema .NET / Linux.
- Instalados os pacotes NuGet fundamentais:
  - `Npgsql.EntityFrameworkCore.PostgreSQL` (v10.0.3)
  - `Microsoft.EntityFrameworkCore.Design` (v10.0.14)
- Estruturada a arquitetura em camadas MVC com separação estrita de responsabilidades:
  - `Models/Common/BaseEntity.cs`: Entidade base com timestamps de auditoria (`CreatedAt`, `UpdatedAt`) e `IsActive`.
  - `Data/AppDbContext.cs`: DbContext configurado com interceptor automático de auditoria e carregamento de configurações do assembly.
  - `Repositories/Interfaces/IRepository.cs` e `Repositories/Repository.cs`: Padrão Generic Repository assíncrono.
  - `Repositories/Interfaces/IUnitOfWork.cs` e `Repositories/UnitOfWork.cs`: Controle transacional e gerenciamento de repositórios.
  - `Services/Common/OperationResult.cs`: Objeto de transferência de resultado desacoplado para manter Thin Controllers.
  - `Services/Interfaces/IService.cs`: Interface base para injeção de dependência dos módulos de negócio.
  - `ViewModels/ErrorViewModel.cs`: Isolamento de ViewModels fora da camada de Models (Domínio).
  - `Middlewares/SecurityHeadersMiddleware.cs`: Injeção de cabeçalhos de segurança OWASP (CSP, HSTS, X-Frame-Options, X-Content-Type-Options).
  - `Middlewares/GlobalExceptionHandlerMiddleware.cs`: Tratamento global com suporte a RFC 7807 (ProblemDetails para requisições AJAX/JSON) e redirecionamento MVC.
- Reconfigurado `Program.cs` com suporte a proxy reverso Nginx (`ForwardedHeadersOptions`), Kestrel, DbContext e DI.
- Atualizado `appsettings.json` com endpoint Kestrel (`http://localhost:5000`) e string de conexão do PostgreSQL.
- Criado manifesto de configuração do Nginx em `infra/nginx/serralheria-paredes.conf` com suporte a Cloudflare (Full Strict SSL, `CF-Connecting-IP`, preservação de IP real, rate limiting e gzip).
- Criado arquivo de serviço do systemd em `infra/systemd/serralheria-paredes.service` com hardening e execução não-root.
- Modernizado e documentado o `README.md` com badges da stack (.NET 10, C# 13, EF Core, PostgreSQL, Nginx, Cloudflare, Ubuntu), fluxo arquitetural e instruções completas de execução local e produção.

## 2. Decisões Tomadas
- **Camada de Repositórios Genérica + UnitOfWork:** Desacopla o EF Core da camada de negócio e permite transações ACID consistentes em operações compostas.
- **Thin Controllers com OperationResult:** Os Controllers delegam 100% da lógica para os Services e apenas tomam decisões de apresentação (View ou Redirect) inspecionando `OperationResult`.
- **Proxy Reverso Nginx + Kestrel:** Kestrel escuta internamente em `127.0.0.1:5000` sem exposição direta à internet. O Nginx atua como proxy reverso gerenciando buffer, rate limit e terminação TLS com certificados Origin da Cloudflare.
- **Tratamento de Headers no ASP.NET Core:** Utilizado `ForwardedHeadersOptions` com `KnownIPNetworks.Clear()` e `KnownProxies.Clear()` para suportar dinamicamente cabeçalhos vindos do Nginx/Cloudflare em ambiente Linux.

## 3. O Que Falta / Próximos Passos
- Implementar o módulo de Autenticação/Autorização (ASP.NET Core Identity ou Cookies seguros com Hash PBKDF2/Argon2 para colaboradores e perfis de acesso: Administrador, Serralheiro, Atendente).
- Iniciar a modelagem dos módulos centrais de negócio:
  - Módulo 1: Gestão de Clientes e Fornecedores.
  - Módulo 2: Catálogo de Perfis, Tubos, Chapas e Acessórios (Estoque/Insumos).
  - Módulo 3: Orçamentos com cálculo automatizado de corte, metragem e margem.
  - Módulo 4: Ordens de Serviço (OS) e fluxo de produção da serralheria.
- Executar migrations iniciais do EF Core no PostgreSQL (`dotnet ef migrations add InitialCreate`).

## 4. Estado Atual da Aplicação
- **Build Status:** Compilação com êxito (0 erros, 0 avisos) via `dotnet build`.
- **Target Framework:** .NET 10 (`net10.0`).
- **Git Branch:** `feature/arquitetura-base-infraestrutura` (sincronizada e publicada em `origin`).
- **Repositório Remoto:** `git@github.com:GeovaneParedes/Serralheria-Paredes.git`
