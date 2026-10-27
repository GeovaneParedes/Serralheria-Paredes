namespace Serralheria_Paredes.Application.NotasFiscais;

public sealed record NotaFiscalImportada(
    string Arquivo,
    string? Numero,
    string? Serie,
    string? ChaveAcesso,
    string? Emitente,
    string? CnpjEmitente,
    decimal? ValorTotal,
    IReadOnlyList<ItemNotaFiscalImportado> Itens,
    IReadOnlyList<string> Alertas,
    bool RevisaoNecessaria);

public sealed record ItemNotaFiscalImportado(
    string Codigo,
    string Descricao,
    decimal Quantidade,
    decimal ValorUnitario,
    decimal ValorTotal);
