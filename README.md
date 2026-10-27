# 🏭 Serralheria-Paredes

Sistema de gestão integrado e de alta performance para indústria e serviços de serralheria, desenvolvido com foco em arquitetura limpa, segurança perimetral e escalabilidade.

---

### 🚀 Badges & Stack Tecnológica

<p align="left">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10" />
  <img src="https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core MVC" />
  <img src="https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/PostgreSQL-16%2B-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Nginx-Reverse%20Proxy-009639?style=for-the-badge&logo=nginx&logoColor=white" alt="Nginx" />
  <img src="https://img.shields.io/badge/Cloudflare-SSL%20%2F%20DDoS-F38020?style=for-the-badge&logo=cloudflare&logoColor=white" alt="Cloudflare" />
  <img src="https://img.shields.io/badge/Ubuntu-Linux-E95420?style=for-the-badge&logo=ubuntu&logoColor=white" alt="Linux Ubuntu" />
  <img src="https://img.shields.io/badge/Build-Passing-brightgreen?style=for-the-badge" alt="Build Status" />
</p>

---

## 📌 Visão Geral do Sistema

O **Serralheria-Paredes** foi concebido para atender às demandas operacionais e financeiras de oficinas metalúrgicas e serralherias:
- **Gestão de Clientes e Fornecedores:** Controle cadastral completo (PF/PJ) com histórico de interações.
- **Catálogo de Insumos & Estoque:** Perfis de alumínio, tubos de aço, cantoneiras, chapas e ferragens com rastreio de barras inteiras e sobras de corte.
- **Orçamentos Inteligentes:** Cálculo automatizado de metragem linear, peso de materiais, aproveitamento de corte e margens de lucro.
- **Ordens de Serviço (OS):** Acompanhamento do fluxo de fabricação, corte, solda, pintura e instalação.

---

## 🏛️ Arquitetura de Software

O projeto segue estritamente a separação em camadas com **Thin Controllers**, **Result Pattern**, **Repository Genérico** e **Unit of Work**:

```
[ Cliente / Browser ]
         │ (HTTPS / TLS 1.3)
         ▼
[ Cloudflare (WAF, Proteção DDoS, SSL Full Strict) ]
         │ (CF-Connecting-IP, X-Forwarded-Proto)
         ▼
[ Nginx (Proxy Reverso - Rate Limit, Buffering, Gzip) ]
         │ (HTTP / 127.0.0.1:5000)
         ▼
[ Kestrel (.NET 10 - ASP.NET Core) ]
         ├── ForwardedHeaders (Restauração do IP real do cliente)
         ├── SecurityHeaders (OWASP: CSP, HSTS, X-Frame-Options)
         ├── GlobalExceptionHandler (Logs estruturados + RFC 7807)
         │
         ├── Controllers (Apenas delegam e devolvem Views/JSON)
         │       │
         │       ▼
         ├── Services (Regras de negócio, cálculos e contratos desacoplados)
         │       │
         │       ▼
         ├── Repositories & UnitOfWork (Abstração assíncrona EF Core)
         │       │
         │       ▼
         └── PostgreSQL (Npgsql com connection pooling e auditoria automática)
```

---

## 📁 Estrutura do Projeto

```text
Serralheria-Paredes/
├── Controllers/         # Apenas recebem requisições, chamam serviços e retornam Views/JSON
├── Data/                # DbContext, mapeamentos e convenções EF Core
├── Middlewares/         # Middlewares de segurança OWASP e tratamento global de erros
├── Models/              # Entidades do Domínio e Banco de Dados (PostgreSQL)
│   └── Common/          # Entidade Base com auditoria (CreatedAt, UpdatedAt, IsActive)
├── Repositories/        # Acesso ao banco de dados desacoplado via EF Core
│   └── Interfaces/      # Contratos IRepository<T> e IUnitOfWork
├── Services/            # Implementações de regra de negócio pura
│   ├── Common/          # OperationResult e Result Pattern para Thin Controllers
│   └── Interfaces/      # Contratos dos serviços via Injeção de Dependência
├── ViewModels/          # Objetos de transporte (DTOs) específicos para as Views Razor
├── Views/               # Telas Razor (.cshtml) organizadas por controlador
├── infra/               # Manifestos de infraestrutura e deployment
│   ├── nginx/           # Configuração de proxy reverso com suporte a Cloudflare
│   └── systemd/         # Definição do serviço Linux não-root
├── wwwroot/             # Ativos estáticos compilados (CSS, JS, bibliotecas)
└── Program.cs           # Bootstrap, DI, Forwarded Headers e Pipeline HTTP
```

---

## ⚙️ Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`10.0.401` ou superior)
- [PostgreSQL 16+](https://www.postgresql.org/)
- [Nginx](https://nginx.org/) (em ambiente de homologação/produção Linux)
- [Git](https://git-scm.com/)

---

## 🚀 Como Executar o Projeto Localmente

### 1. Clonar o Repositório
```bash
git clone git@github.com:GeovaneParedes/Serralheria-Paredes.git
cd Serralheria-Paredes
```

### 2. Configurar a String de Conexão
Edite o arquivo `appsettings.json` ou defina via variável de ambiente:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=serralheria_paredes_db;Username=postgres;Password=SEU_PASSWORD;Include Error Detail=true;"
}
```

### 3. Restaurar e Compilar
```bash
dotnet restore
dotnet build
```

### 4. Executar a Aplicação
```bash
dotnet run
```
Acesse no navegador: `http://localhost:5000`

---

## 🛡️ Segurança e Produção

- **Proxy Reverso Seguro:** Proteção de borda com Nginx repassando `X-Real-IP`, `X-Forwarded-For` e `CF-Connecting-IP`.
- **Cabeçalhos OWASP:** Injeção automática de `Content-Security-Policy`, `X-Frame-Options: DENY` e `X-Content-Type-Options: nosniff`.
- **Cookies Blindados:** Proteção CSRF com prefixo `__Host-` e política `SameSite=Strict`.
- **Hardenização Systemd:** Execução não-root (`www-data`) com isolamento de diretórios sensíveis (`ProtectSystem=full`, `PrivateTmp=true`).

---

## 📄 Licença

Este projeto é desenvolvido para a **Serralheria Paredes**. Todos os direitos reservados.
