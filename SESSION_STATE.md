# SESSION_STATE - Serralheria-Paredes

## 1. O Que Foi Feito
- **Fundação Arquitetural e Infraestrutura (Concluído & Merged em main):**
  - Configuração de .NET 10, Npgsql (PostgreSQL), Kestrel na porta 5000, Nginx reverse proxy com suporte a Cloudflare e systemd service.
  - Middlewares de segurança OWASP (`SecurityHeadersMiddleware`) e captura global de exceções (`GlobalExceptionHandlerMiddleware`).
  - Generic Repository, Unit of Work e Result Pattern (`OperationResult`).
- **Módulo 1: Autenticação e Perfis de Acesso (Concluído na branch `feature/modulo-autenticacao`):**
  - Criada branch dedicada `feature/modulo-autenticacao`.
  - Instalado o pacote `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (v10.0.14).
  - Modeladas as entidades de domínio `Usuario` (com `NomeCompleto`, `Cargo`, auditoria e `IsActive`) e `Perfil` (com `Descricao`).
  - Definidas constantes de papéis do sistema em `PerfisConstantes` (`Administrador`, `Serralheiro`, `Atendente`).
  - Atualizado `AppDbContext` herdando de `IdentityDbContext<Usuario, Perfil, int>` com mapeamento para convenções limpas do PostgreSQL (`usuarios`, `perfis`, `usuario_perfis`, etc.).
  - Criadas ViewModels dedicadas: `LoginViewModel`, `NovoUsuarioViewModel`, `UsuarioViewModel`.
  - Implementado `IAuthService` e `AuthService` com regras completas de login, logout, validação de bloqueio por tentativas (lockout), verificação de usuário ativo e invalidação de sessão ao desativar usuário (`UpdateSecurityStampAsync`).
  - Implementado `IdentityDataSeeder` para auto-provisionamento dos perfis e administrador padrão (`admin@serralheriaparedes.com.br`).
  - Desenvolvido `AuthController` seguindo padrão Thin Controller sem dependência direta do DbContext.
  - Criadas as telas Razor completas e responsivas:
    - `Views/Auth/Login.cshtml`: Tela de login com card moderno e tratamento de mensagens.
    - `Views/Auth/Usuarios.cshtml`: Gestão de colaboradores com badges por perfil e ativação/desativação.
    - `Views/Auth/NovoUsuario.cshtml`: Cadastro de funcionários com validação e atribuição de papel.
    - `Views/Auth/AcessoNegado.cshtml`: Página de bloqueio de permissão HTTP 403.
  - Atualizado `_Layout.cshtml` com cabeçalho de usuário logado e menu administrativo restrito.
  - Modernizado `Home/Index.cshtml` para servir como Dashboard da Serralheria com atalhos para os 4 módulos.
  - Gerada Migration inicial do EF Core: `Data/Migrations/20261027002236_CriarTabelasIdentity.cs`.
  - **Limpeza de Código Morto do Template MVC Padrão:** Removida a View `Views/Home/Privacy.cshtml`, a action `Privacy()` de `Controllers/HomeController.cs`, o estilo conflitante `Views/Shared/_Layout.cshtml.css`, e tags não utilizadas no `_Layout.cshtml`.
  - **Automação de Ambiente Dev com Cloudflare Quick Tunnel (`dev.sh`):** Criado script utilitário executável `dev.sh` na raiz do projeto, permitindo rodar localmente na porta 5000 com geração automática de URL pública HTTPS temporária (`https://*.trycloudflare.com`) e encerramento gracioso via `Ctrl+C`.

## 2. Decisões Tomadas
- **ASP.NET Core Identity com PostgreSQL:** Optou-se pela robustez do Identity com chave primária inteira (`int`), hashing PBKDF2 (SHA-256) nativo, políticas de lockout contra brute-force (5 tentativas = 15 minutos de bloqueio) e cookies seguros (`__Host-Serralheria.Auth`, `SameSite=Strict`, `HttpOnly=true`).
- **Invalidação Proativa de Sessão:** Ao desativar um colaborador no `AuthService`, o `SecurityStamp` do usuário é renovado imediatamente, invalidando qualquer cookie de sessão ativo no navegador do colaborador.
- **Isolamento de Seed:** O `IdentityDataSeeder` roda na inicialização do aplicativo dentro de um bloco resiliente com log informativo, permitindo inicialização contínua mesmo se o banco estiver em provisionamento.

## 3. O Que Falta / Próximos Passos
- Realizar o merge da branch `feature/modulo-autenticacao` após revisão do PR.
- Iniciar o **Módulo 2: Clientes e Fornecedores** na branch `feature/modulo-cadastros`:
  - Entidades `Cliente` e `Fornecedor` com suporte a Pessoa Física (CPF) e Pessoa Jurídica (CNPJ).
  - Validação algorítmica de CPF e CNPJ (dígitos verificadores).
  - Serviços de negócio (`IClienteService`, `IFornecedorService`).
  - Interfaces de CRUD ricas com busca e paginação.

## 4. Estado Atual da Aplicação
- **Build Status:** Compilação com êxito (0 erros, 0 avisos) via `dotnet build`.
- **Target Framework:** .NET 10 (`net10.0`).
- **Git Branch:** `feature/modulo-autenticacao`.
- **Repositório Remoto:** `git@github.com:GeovaneParedes/Serralheria-Paredes.git`.
