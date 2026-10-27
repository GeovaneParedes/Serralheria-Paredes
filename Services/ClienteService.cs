using Serralheria_Paredes.Common.Validation;
using Serralheria_Paredes.Common.Pagination;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Cadastros;

namespace Serralheria_Paredes.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(IClienteRepository clienteRepository, ILogger<ClienteService> logger)
    {
        _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PagedResult<ClienteViewModel>> ListarAsync(string? termo = null, int pagina = 1, int tamanho = Paginacao.TamanhoPadrao, CancellationToken cancellationToken = default)
    {
        (pagina, tamanho) = Paginacao.Normalizar(pagina, tamanho);
        var clientes = await _clienteRepository.ObterTodosComFiltroAsync(termo, pagina, tamanho, cancellationToken);
        return new PagedResult<ClienteViewModel>(clientes.Items.Select(MapearParaViewModel).ToList(), pagina, tamanho, clientes.Total);
    }

    public async Task<OperationResult<ClienteViewModel>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            return OperationResult<ClienteViewModel>.Fail("Cliente não encontrado.");
        }

        return OperationResult<ClienteViewModel>.Ok(MapearParaViewModel(cliente));
    }

    public async Task<OperationResult<ClienteFormViewModel>> ObterParaEdicaoAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            return OperationResult<ClienteFormViewModel>.Fail("Cliente não encontrado.");
        }

        var form = new ClienteFormViewModel
        {
            Id = cliente.Id,
            TipoPessoa = cliente.TipoPessoa,
            NomeOuRazaoSocial = cliente.NomeOuRazaoSocial,
            NomeFantasia = cliente.NomeFantasia,
            CpfOuCnpj = DocumentosUtils.Formatar(cliente.CpfOuCnpj),
            RgOuInscricaoEstadual = cliente.RgOuInscricaoEstadual,
            Telefone = cliente.Telefone,
            Celular = cliente.Celular,
            Email = cliente.Email,
            EnderecoCep = cliente.EnderecoCep,
            EnderecoLogradouro = cliente.EnderecoLogradouro,
            EnderecoNumero = cliente.EnderecoNumero,
            EnderecoComplemento = cliente.EnderecoComplemento,
            EnderecoBairro = cliente.EnderecoBairro,
            EnderecoCidade = cliente.EnderecoCidade,
            EnderecoEstado = cliente.EnderecoEstado,
            Observacoes = cliente.Observacoes,
            IsActive = cliente.IsActive
        };

        return OperationResult<ClienteFormViewModel>.Ok(form);
    }

    public async Task<OperationResult<ClienteViewModel>> CriarAsync(ClienteFormViewModel model, CancellationToken cancellationToken = default)
    {
        var validacao = ValidarDocumentoEUnicidade(model.TipoPessoa, model.CpfOuCnpj);
        if (!validacao.Valido)
        {
            return OperationResult<ClienteViewModel>.Fail(validacao.Mensagem!);
        }

        var numerosDocumento = DocumentosUtils.ApenasNumeros(model.CpfOuCnpj);
        if (await _clienteRepository.DocumentoJaCadastradoAsync(numerosDocumento, null, cancellationToken))
        {
            return OperationResult<ClienteViewModel>.Fail($"Já existe um cliente cadastrado com este {(model.TipoPessoa == TipoPessoa.Fisica ? "CPF" : "CNPJ")}.");
        }

        var cliente = new Cliente
        {
            TipoPessoa = model.TipoPessoa,
            NomeOuRazaoSocial = model.NomeOuRazaoSocial.Trim(),
            NomeFantasia = model.NomeFantasia?.Trim(),
            CpfOuCnpj = numerosDocumento,
            RgOuInscricaoEstadual = model.RgOuInscricaoEstadual?.Trim(),
            Telefone = model.Telefone.Trim(),
            Celular = model.Celular?.Trim(),
            Email = model.Email?.Trim().ToLower(),
            EnderecoCep = DocumentosUtils.ApenasNumeros(model.EnderecoCep),
            EnderecoLogradouro = model.EnderecoLogradouro?.Trim(),
            EnderecoNumero = model.EnderecoNumero?.Trim(),
            EnderecoComplemento = model.EnderecoComplemento?.Trim(),
            EnderecoBairro = model.EnderecoBairro?.Trim(),
            EnderecoCidade = model.EnderecoCidade?.Trim(),
            EnderecoEstado = model.EnderecoEstado?.Trim().ToUpper(),
            Observacoes = model.Observacoes?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _clienteRepository.AddAsync(cliente, cancellationToken);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente criado com sucesso: ID {Id} - {Nome}", cliente.Id, cliente.NomeOuRazaoSocial);
        return OperationResult<ClienteViewModel>.Ok(MapearParaViewModel(cliente), "Cliente cadastrado com sucesso.");
    }

    public async Task<OperationResult<ClienteViewModel>> AtualizarAsync(int id, ClienteFormViewModel model, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            return OperationResult<ClienteViewModel>.Fail("Cliente não encontrado.");
        }

        var validacao = ValidarDocumentoEUnicidade(model.TipoPessoa, model.CpfOuCnpj);
        if (!validacao.Valido)
        {
            return OperationResult<ClienteViewModel>.Fail(validacao.Mensagem!);
        }

        var numerosDocumento = DocumentosUtils.ApenasNumeros(model.CpfOuCnpj);
        if (await _clienteRepository.DocumentoJaCadastradoAsync(numerosDocumento, id, cancellationToken))
        {
            return OperationResult<ClienteViewModel>.Fail($"Já existe outro cliente cadastrado com este {(model.TipoPessoa == TipoPessoa.Fisica ? "CPF" : "CNPJ")}.");
        }

        cliente.TipoPessoa = model.TipoPessoa;
        cliente.NomeOuRazaoSocial = model.NomeOuRazaoSocial.Trim();
        cliente.NomeFantasia = model.NomeFantasia?.Trim();
        cliente.CpfOuCnpj = numerosDocumento;
        cliente.RgOuInscricaoEstadual = model.RgOuInscricaoEstadual?.Trim();
        cliente.Telefone = model.Telefone.Trim();
        cliente.Celular = model.Celular?.Trim();
        cliente.Email = model.Email?.Trim().ToLower();
        cliente.EnderecoCep = DocumentosUtils.ApenasNumeros(model.EnderecoCep);
        cliente.EnderecoLogradouro = model.EnderecoLogradouro?.Trim();
        cliente.EnderecoNumero = model.EnderecoNumero?.Trim();
        cliente.EnderecoComplemento = model.EnderecoComplemento?.Trim();
        cliente.EnderecoBairro = model.EnderecoBairro?.Trim();
        cliente.EnderecoCidade = model.EnderecoCidade?.Trim();
        cliente.EnderecoEstado = model.EnderecoEstado?.Trim().ToUpper();
        cliente.Observacoes = model.Observacoes?.Trim();
        cliente.UpdatedAt = DateTime.UtcNow;

        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cliente atualizado: ID {Id} - {Nome}", cliente.Id, cliente.NomeOuRazaoSocial);
        return OperationResult<ClienteViewModel>.Ok(MapearParaViewModel(cliente), "Dados do cliente atualizados com sucesso.");
    }

    public async Task<OperationResult> AlternarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
        {
            return OperationResult.Fail("Cliente não encontrado.");
        }

        cliente.IsActive = ativo;
        cliente.UpdatedAt = DateTime.UtcNow;

        _clienteRepository.Update(cliente);
        await _clienteRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Status do cliente ID {Id} alterado para {Ativo}", id, ativo);
        return OperationResult.Ok($"Cliente {(ativo ? "ativado" : "desativado")} com sucesso.");
    }

    private static (bool Valido, string? Mensagem) ValidarDocumentoEUnicidade(TipoPessoa tipo, string documento)
    {
        var numeros = DocumentosUtils.ApenasNumeros(documento);

        if (tipo == TipoPessoa.Fisica)
        {
            if (!DocumentosUtils.ValidarCpf(numeros))
                return (false, "O CPF informado é inválido pelo algoritmo verificador da Receita Federal.");
        }
        else
        {
            if (!DocumentosUtils.ValidarCnpj(numeros))
                return (false, "O CNPJ informado é inválido pelo algoritmo verificador da Receita Federal.");
        }

        return (true, null);
    }

    private static ClienteViewModel MapearParaViewModel(Cliente c) => new()
    {
        Id = c.Id,
        NomeOuRazaoSocial = c.NomeOuRazaoSocial,
        NomeFantasia = c.NomeFantasia,
        TipoPessoa = c.TipoPessoa,
        CpfOuCnpj = c.CpfOuCnpj,
        RgOuInscricaoEstadual = c.RgOuInscricaoEstadual,
        Telefone = c.Telefone,
        Celular = c.Celular,
        Email = c.Email,
        EnderecoCidade = c.EnderecoCidade,
        EnderecoEstado = c.EnderecoEstado,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    };
}
