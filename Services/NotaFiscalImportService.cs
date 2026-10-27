using System.Globalization;
using System.Text.RegularExpressions;
using Serralheria_Paredes.Application.NotasFiscais;
using Serralheria_Paredes.Services.Common;
using Serralheria_Paredes.Services.Interfaces;
using UglyToad.PdfPig;

namespace Serralheria_Paredes.Services;

public class NotaFiscalImportService : INotaFiscalImportService
{
    private const long TamanhoMaximo = 10 * 1024 * 1024;
    private static readonly string[] ExtensoesPdf = [".pdf"];
    private static readonly Regex NumeroRegex = new(@"N[ºo]?\.?\s*(?<numero>\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ChaveRegex = new(@"(?<!\d)(?:\d\s*){44}(?!\d)", RegexOptions.Compiled);
    private static readonly Regex CnpjRegex = new(@"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}", RegexOptions.Compiled);
    private static readonly Regex ValorNotaRegex = new(@"VALOR TOTAL DA NOTA\s+(?<valor>\d+[\d.,]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ItemRegex = new(@"(?<codigo>\d{8,14})\s+(?<descricao>.+?)\s+\d{8}\s+\d{3}\s+\d{4}\s+(?:KG|UN|MT|M|PC)-?\s+(?<quantidade>\d+[,.]\d+)\s+(?<unitario>\d+[,.]\d+)\s+(?<total>\d+[,.]\d+)", RegexOptions.Compiled);

    public async Task<OperationResult<NotaFiscalImportada>> AnalisarAsync(Stream arquivo, string nomeArquivo, CancellationToken cancellationToken = default)
    {
        if (arquivo == null || !arquivo.CanRead) return OperationResult<NotaFiscalImportada>.Fail("Arquivo fiscal inválido.");
        if (arquivo.Length > TamanhoMaximo) return OperationResult<NotaFiscalImportada>.Fail("O arquivo fiscal deve ter no máximo 10 MB.");

        var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
        if (!ExtensoesPdf.Contains(extensao))
            return OperationResult<NotaFiscalImportada>.Fail("Nesta etapa, apenas PDF textual é aceito. Imagens JPG/PNG exigem o provider OCR configurado.");

        arquivo.Position = 0;
        using var documento = PdfDocument.Open(arquivo);
        var texto = string.Join("\n", documento.GetPages().Select(p => p.Text));
        if (string.IsNullOrWhiteSpace(texto)) return OperationResult<NotaFiscalImportada>.Fail("Não foi possível extrair texto do PDF.");

        return OperationResult<NotaFiscalImportada>.Ok(AnalisarTexto(texto, nomeArquivo));
    }

    public static NotaFiscalImportada AnalisarTexto(string texto, string nomeArquivo)
    {
        var alertas = new List<string> { "Revise os dados extraídos antes de lançar a compra e atualizar o estoque." };
        var numeroMatch = NumeroRegex.Match(texto);
        var chave = ChaveRegex.Match(texto).Success ? Regex.Replace(ChaveRegex.Match(texto).Value, "\\s", "") : null;
        var cnpjs = CnpjRegex.Matches(texto).Select(m => m.Value).Distinct().ToList();
        var valor = ParseDecimal(ValorNotaRegex.Match(texto).Groups["valor"].Value);
        var itens = ItemRegex.Matches(texto).Select(m => new ItemNotaFiscalImportado(
            m.Groups["codigo"].Value,
            m.Groups["descricao"].Value.Trim(),
            ParseDecimal(m.Groups["quantidade"].Value) ?? 0,
            ParseDecimal(m.Groups["unitario"].Value) ?? 0,
            ParseDecimal(m.Groups["total"].Value) ?? 0)).ToList();

        if (chave == null) alertas.Add("Chave de acesso não identificada.");
        if (itens.Count == 0) alertas.Add("Nenhum item estruturado foi identificado.");
        if (valor == null) alertas.Add("Valor total não identificado.");

        var emitente = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .FirstOrDefault(l => l.Contains("LTDA", StringComparison.OrdinalIgnoreCase) || l.Contains("S/A", StringComparison.OrdinalIgnoreCase));

        return new NotaFiscalImportada(nomeArquivo, numeroMatch.Success ? numeroMatch.Groups["numero"].Value : null, null, chave, emitente, cnpjs.FirstOrDefault(), valor, itens, alertas, true);
    }

    private static decimal? ParseDecimal(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        return decimal.TryParse(valor.Replace(".", "").Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var resultado) ? resultado : null;
    }
}
