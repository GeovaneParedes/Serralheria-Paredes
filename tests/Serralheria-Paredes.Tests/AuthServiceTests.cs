using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Serralheria_Paredes.Data;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Constantes;
using Serralheria_Paredes.Services;
using Serralheria_Paredes.ViewModels.Auth;
using Xunit;

namespace Serralheria_Paredes.Tests;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<UserManager<Usuario>> _userManagerMock;
    private readonly Mock<SignInManager<Usuario>> _signInManagerMock;
    private readonly Mock<RoleManager<Perfil>> _roleManagerMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        var userStore = new Mock<IUserStore<Usuario>>();
        _userManagerMock = new Mock<UserManager<Usuario>>(
            userStore.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<Usuario>>(),
            Array.Empty<IUserValidator<Usuario>>(),
            Array.Empty<IPasswordValidator<Usuario>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<Usuario>>>());

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
        _signInManagerMock = new Mock<SignInManager<Usuario>>(
            _userManagerMock.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<ILogger<SignInManager<Usuario>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<Usuario>>());

        var roleStore = new Mock<IRoleStore<Perfil>>();
        _roleManagerMock = new Mock<RoleManager<Perfil>>(
            roleStore.Object,
            Array.Empty<IRoleValidator<Perfil>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<ILogger<RoleManager<Perfil>>>());

        _loggerMock = new Mock<ILogger<AuthService>>();

        _sut = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _roleManagerMock.Object,
            _context,
            _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task LoginAsync_ComCredenciaisValidasEUsuarioAtivo_DeveRetornarSucesso()
    {
        // Arrange
        var model = new LoginViewModel { Email = "admin@serralheria.com", Senha = "Password123!", LembrarMe = true };
        var usuario = new Usuario
        {
            Id = 1,
            Email = model.Email,
            NomeCompleto = "Administrador",
            Cargo = "Gestor",
            IsActive = true
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(usuario);
        _signInManagerMock.Setup(m => m.PasswordSignInAsync(usuario, model.Senha, model.LembrarMe, true))
            .ReturnsAsync(SignInResult.Success);
        _userManagerMock.Setup(m => m.GetRolesAsync(usuario)).ReturnsAsync(new List<string> { PerfisConstantes.Administrador });

        // Act
        var result = await _sut.LoginAsync(model);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Administrador", result.Data!.NomeCompleto);
        Assert.Equal(PerfisConstantes.Administrador, result.Data.Perfil);
    }

    [Fact]
    public async Task LoginAsync_ComUsuarioInexistente_DeveRetornarFalhaGenerica()
    {
        // Arrange
        var model = new LoginViewModel { Email = "desconhecido@serralheria.com", Senha = "Password123!" };
        _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync((Usuario?)null);

        // Act
        var result = await _sut.LoginAsync(model);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("E-mail ou senha inválidos.", result.Errors);
        _signInManagerMock.Verify(m => m.PasswordSignInAsync(It.IsAny<Usuario>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ComUsuarioInativo_DeveBloquearLoginSemChamarSignInManager()
    {
        // Arrange
        var model = new LoginViewModel { Email = "inativo@serralheria.com", Senha = "Password123!" };
        var usuario = new Usuario
        {
            Id = 2,
            Email = model.Email,
            NomeCompleto = "Usuário Desativado",
            IsActive = false
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(usuario);

        // Act
        var result = await _sut.LoginAsync(model);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Este usuário está inativo.", result.Errors.First());
        _signInManagerMock.Verify(m => m.PasswordSignInAsync(It.IsAny<Usuario>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_QuandoBloqueadoPorLockout_DeveRetornarMensagemDeLockout()
    {
        // Arrange
        var model = new LoginViewModel { Email = "alvo@serralheria.com", Senha = "Password123!" };
        var usuario = new Usuario { Id = 3, Email = model.Email, IsActive = true };

        _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync(usuario);
        _signInManagerMock.Setup(m => m.PasswordSignInAsync(usuario, model.Senha, It.IsAny<bool>(), true))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act
        var result = await _sut.LoginAsync(model);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Conta temporariamente bloqueada", result.Errors.First());
    }

    [Fact]
    public async Task RegistrarUsuarioAsync_ComEmailDuplicado_DeveRetornarFalha()
    {
        // Arrange
        var model = new NovoUsuarioViewModel
        {
            Email = "duplicado@serralheria.com",
            NomeCompleto = "Novo",
            Perfil = PerfisConstantes.Serralheiro,
            Senha = "Password123!"
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email))
            .ReturnsAsync(new Usuario { Id = 99, Email = model.Email });

        // Act
        var result = await _sut.RegistrarUsuarioAsync(model);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Já existe um usuário cadastrado com este e-mail.", result.Errors);
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<Usuario>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarUsuarioAsync_ComPerfilInvalido_DeveRetornarFalha()
    {
        // Arrange
        var model = new NovoUsuarioViewModel
        {
            Email = "novousuario@serralheria.com",
            NomeCompleto = "Novo",
            Perfil = "PerfilInexistenteHacker",
            Senha = "Password123!"
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(model.Email)).ReturnsAsync((Usuario?)null);

        // Act
        var result = await _sut.RegistrarUsuarioAsync(model);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("é inválido", result.Errors.First());
        _userManagerMock.Verify(m => m.CreateAsync(It.IsAny<Usuario>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AlterarStatusUsuarioAsync_QuandoAdminTentaDesativarASiMesmo_DeveBloquearOperacao()
    {
        // Arrange
        const int adminId = 10;
        const int executanteId = 10;

        // Act
        var result = await _sut.AlterarStatusUsuarioAsync(adminId, ativo: false, executanteId: executanteId);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("não pode desativar seu próprio usuário administrador", result.Errors.First());
        _userManagerMock.Verify(m => m.FindByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task AlterarStatusUsuarioAsync_AoDesativarOutroUsuario_DeveAtualizarSecurityStamp()
    {
        // Arrange
        const int usuarioAlvoId = 20;
        const int executanteId = 10;
        var usuario = new Usuario { Id = usuarioAlvoId, Email = "alvo@serralheria.com", IsActive = true };

        _userManagerMock.Setup(m => m.FindByIdAsync(usuarioAlvoId.ToString())).ReturnsAsync(usuario);
        _userManagerMock.Setup(m => m.UpdateSecurityStampAsync(usuario)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.UpdateAsync(usuario)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.AlterarStatusUsuarioAsync(usuarioAlvoId, ativo: false, executanteId: executanteId);

        // Assert
        Assert.True(result.Success);
        Assert.False(usuario.IsActive);
        _userManagerMock.Verify(m => m.UpdateSecurityStampAsync(usuario), Times.Once);
        _userManagerMock.Verify(m => m.UpdateAsync(usuario), Times.Once);
    }

    [Fact]
    public async Task AlterarStatusUsuarioAsync_AoAtivarUsuario_NaoDeveAtualizarSecurityStamp()
    {
        // Arrange
        const int usuarioAlvoId = 20;
        const int executanteId = 10;
        var usuario = new Usuario { Id = usuarioAlvoId, Email = "alvo@serralheria.com", IsActive = false };

        _userManagerMock.Setup(m => m.FindByIdAsync(usuarioAlvoId.ToString())).ReturnsAsync(usuario);
        _userManagerMock.Setup(m => m.UpdateAsync(usuario)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.AlterarStatusUsuarioAsync(usuarioAlvoId, ativo: true, executanteId: executanteId);

        // Assert
        Assert.True(result.Success);
        Assert.True(usuario.IsActive);
        _userManagerMock.Verify(m => m.UpdateSecurityStampAsync(It.IsAny<Usuario>()), Times.Never);
        _userManagerMock.Verify(m => m.UpdateAsync(usuario), Times.Once);
    }
}
