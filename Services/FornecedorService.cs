using Serralheria_Paredes.Common.Validation;
using Serralheria_Paredes.Common.Pagination;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Enums;
using Serralheria_Paredes.Repositories.Interfaces;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Services.Interfaces;
using Serralheria_Paredes.ViewModels.Cadastros;

namespace Serralheria_Paredes.Services;

public class FornecedorService : IFornecedorService
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly ILogger<FornecedorService> _logger;

    public FornecedorService(IFornecedorRepository fornecedorRepository, ILogger<FornecedorService> logger)
    {
        _fornecedorRepository = fornecedorRepository ?? throw new ArgumentNullException(nameof(fornecedorRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PagedResult<FornecedorViewModel>> ListarAsync(string? termo = null, int pagina = 1, int tamanho = Paginacao.TamanhoPadrao, CancellationToken cancellationToken = default)
    {
        (pagina, tamanho) = Paginacao.Normalizar(pagina, tamanho);
        var fornecedores = await _fornecedorRepository.ObterTodosComFiltroAsync(termo, pagina, tamanho, cancellationToken);
        return new PagedResult<FornecedorViewModel>(fornecedores.Items.Select(MapearParaViewModel).ToList(), pagina, tamanho, fornecedores.Total);
    }

    public async Task<OperationResult<FornecedorViewModel>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var fornecedor = await _fornecedorRepository.GetByIdAsync(id, cancellationToken);
        if (fornecedor == null)
        {
            return OperationResult<FornecedorViewModel>.Fail("Fornecedor não encontrado.");
        }

        return OperationResult<FornecedorViewModel>.Ok(MapearParaViewModel(fornecedor));
    }

    public async Task<OperationResult<FornecedorFormViewModel>> ObterParaEdicaoAsync(int id, CancellationToken cancellationToken = default)
    {
        var fornecedor = await _fornecedorRepository.GetByIdAsync(id, cancellationToken);
        if (fornecedor == null)
        {
            return OperationResult<FornecedorFormViewModel>.Fail("Fornecedor não encontrado.");
        }

        var form = new FornecedorFormViewModel
        {
            Id = fornecedor.Id,
            TipoPessoa = fornecedor.TipoPessoa,
            RazaoSocial = fornecedor.RazaoSocial,
            NomeFantasia = fornecedor.NomeFantasia,
            CpfOuCnpj = DocumentosUtils.Formatar(fornecedor.CpfOuCnpj),
            InscricaoEstadual = fornecedor.InscricaoEstadual,
            Telefone = fornecedor.Telefone,
            Email = fornecedor.Email,
            ContatoResponsavel = fornecedor.ContatoResponsavel,
            EnderecoCep = fornecedor.EnderecoCep,
            EnderecoLogradouro = fornecedor.EnderecoLogradouro,
            EnderecoNumero = fornecedor.EnderecoNumero,
            EnderecoComplemento = fornecedor.EnderecoComplemento,
            EnderecoBairro = fornecedor.EnderecoBairro,
            EnderecoCidade = fornecedor.EnderecoCidade,
            EnderecoEstado = fornecedor.EnderecoEstado,
            Observacoes = fornecedor.Observacoes,
            IsActive = fornecedor.IsActive
        };

        return OperationResult<FornecedorFormViewModel>.Ok(form);
    }

    public async Task<OperationResult<FornecedorViewModel>> CriarAsync(FornecedorFormViewModel model, CancellationToken cancellationToken = default)
    {
        var validacao = ValidarDocumentoEUnicidade(model.TipoPessoa, model.CpfOuCnpj);
        if (!validacao.Valido)
        {
            return OperationResult<FornecedorViewModel>.Fail(validacao.Mensagem!);
        }

        var numerosDocumento = DocumentosUtils.ApenasNumeros(model.CpfOuCnpj);
        if (await _fornecedorRepository.DocumentoJaCadastradoAsync(numerosDocumento, null, cancellationToken))
        {
            return OperationResult<FornecedorViewModel>.Fail($"Já existe um fornecedor cadastrado com este {(model.TipoPessoa == TipoPessoa.Fisica ? "CPF" : "CNPJ")}.");
        }

        var fornecedor = new Fornecedor
        {
            TipoPessoa = model.TipoPessoa,
            RazaoSocial = model.RazaoSocial.Trim(),
            NomeFantasia = model.NomeFantasia?.Trim(),
            CpfOuCnpj = numerosDocumento,
            InscricaoEstadual = model.InscricaoEstadual?.Trim(),
            Telefone = model.Telefone.Trim(),
            Email = model.Email?.Trim().ToLower(),
            ContatoResponsavel = model.ContatoResponsavel?.Trim(),
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

        await _fornecedorRepository.AddAsync(fornecedor, cancellationToken);
        await _fornecedorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fornecedor cadastrado: ID {Id} - {RazaoSocial}", fornecedor.Id, fornecedor.RazaoSocial);
        return OperationResult<FornecedorViewModel>.Ok(MapearParaViewModel(fornecedor), "Fornecedor cadastrado com sucesso.");
    }

    public async Task<OperationResult<FornecedorViewModel>> AtualizarAsync(int id, FornecedorFormViewModel model, CancellationToken cancellationToken = default)
    {
        var fornecedor = await _fornecedorRepository.GetByIdAsync(id, cancellationToken);
        if (fornecedor == null)
        {
            return OperationResult<FornecedorViewModel>.Fail("Fornecedor não encontrado.");
        }

        var validacao = ValidarDocumentoEUnicidade(model.TipoPessoa, model.CpfOuCnpj);
        if (!validacao.Valido)
        {
            return OperationResult<FornecedorViewModel>.Fail(validacao.Mensagem!);
        }

        var numerosDocumento = DocumentosUtils.ApenasNumeros(model.CpfOuCnpj);
        if (await _fornecedorRepository.DocumentoJaCadastradoAsync(numerosDocumento, id, cancellationToken))
        {
            return OperationResult<FornecedorViewModel>.Fail($"Já existe outro fornecedor cadastrado com este {(model.TipoPessoa == TipoPessoa.Fisica ? "CPF" : "CNPJ")}.");
        }

        fornecedor.TipoPessoa = model.TipoPessoa;
        fornecedor.RazaoSocial = model.RazaoSocial.Trim();
        fornecedor.NomeFantasia = model.NomeFantasia?.Trim();
        fornecedor.CpfOuCnpj = numerosDocumento;
        fornecedor.InscricaoEstadual = model.InscricaoEstadual?.Trim();
        fornecedor.Telefone = model.Telefone.Trim();
        fornecedor.Email = model.Email?.Trim().ToLower();
        fornecedor.ContatoResponsavel = model.ContatoResponsavel?.Trim();
        fornecedor.EnderecoCep = DocumentosUtils.ApenasNumeros(model.EnderecoCep);
        fornecedor.EnderecoLogradouro = model.EnderecoLogradouro?.Trim();
        fornecedor.EnderecoNumero = model.EnderecoNumero?.Trim();
        fornecedor.EnderecoComplemento = model.EnderecoComplemento?.Trim();
        fornecedor.EnderecoBairro = model.EnderecoBairro?.Trim();
        fornecedor.EnderecoCidade = model.EnderecoCidade?.Trim();
        fornecedor.EnderecoEstado = model.EnderecoEstado?.Trim().ToUpper();
        fornecedor.Observacoes = model.Observacoes?.Trim();
        fornecedor.UpdatedAt = DateTime.UtcNow;

        _fornecedorRepository.Update(fornecedor);
        await _fornecedorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fornecedor atualizado: ID {Id} - {RazaoSocial}", fornecedor.Id, fornecedor.RazaoSocial);
        return OperationResult<FornecedorViewModel>.Ok(MapearParaViewModel(fornecedor), "Dados do fornecedor atualizados com sucesso.");
    }

    public async Task<OperationResult> AlternarStatusAsync(int id, bool ativo, CancellationToken cancellationToken = default)
    {
        var fornecedor = await _fornecedorRepository.GetByIdAsync(id, cancellationToken);
        if (fornecedor == null)
        {
            return OperationResult.Fail("Fornecedor não encontrado.");
        }

        fornecedor.IsActive = ativo;
        fornecedor.UpdatedAt = DateTime.UtcNow;

        _fornecedorRepository.Update(fornecedor);
        await _fornecedorRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Status do fornecedor ID {Id} alterado para {Ativo}", id, ativo);
        return OperationResult.Ok($"Fornecedor {(ativo ? "ativado" : "desativado")} com sucesso.");
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

    private static FornecedorViewModel MapearParaViewModel(Fornecedor f) => new()
    {
        Id = f.Id,
        RazaoSocial = f.RazaoSocial,
        NomeFantasia = f.NomeFantasia,
        TipoPessoa = f.TipoPessoa,
        CpfOuCnpj = f.CpfOuCnpj,
        InscricaoEstadual = f.InscricaoEstadual,
        Telefone = f.Telefone,
        Email = f.Email,
        ContatoResponsavel = f.ContatoResponsavel,
        EnderecoCidade = f.EnderecoCidade,
        EnderecoEstado = f.EnderecoEstado,
        IsActive = f.IsActive,
        CreatedAt = f.CreatedAt
    };
}
