using System.Text.RegularExpressions;

namespace Serralheria_Paredes.Common.Validation;

/// <summary>
/// Utilitário corporativo para validação algorítmica e formatação de CPF e CNPJ brasileiros.
/// Implementa a verificação oficial dos dígitos verificadores (módulo 11).
/// </summary>
public static class DocumentosUtils
{
    private static readonly Regex ApenasDigitosRegex = new(@"[^\d]", RegexOptions.Compiled);

    public static string ApenasNumeros(string? documento)
    {
        if (string.IsNullOrWhiteSpace(documento)) return string.Empty;
        return ApenasDigitosRegex.Replace(documento, "");
    }

    public static bool ValidarCpf(string? cpf)
    {
        var numeros = ApenasNumeros(cpf);
        if (numeros.Length != 11) return false;

        // Elimina padrões repetitivos conhecidos (ex: 111.111.111-11)
        if (new string(numeros[0], 11) == numeros) return false;

        var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = numeros.Substring(0, 9);
        var soma = 0;

        for (var i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (var i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return numeros.EndsWith(digito);
    }

    public static bool ValidarCnpj(string? cnpj)
    {
        var numeros = ApenasNumeros(cnpj);
        if (numeros.Length != 14) return false;

        // Elimina padrões repetitivos conhecidos
        if (new string(numeros[0], 14) == numeros) return false;

        var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = numeros.Substring(0, 12);
        var soma = 0;

        for (var i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCnpj += digito;
        soma = 0;

        for (var i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();

        return numeros.EndsWith(digito);
    }

    public static bool ValidarDocumento(string? documento, bool ehPessoaFisica)
    {
        return ehPessoaFisica ? ValidarCpf(documento) : ValidarCnpj(documento);
    }

    public static string Formatar(string? documento)
    {
        var numeros = ApenasNumeros(documento);
        if (numeros.Length == 11)
        {
            return Convert.ToUInt64(numeros).ToString(@"000\.000\.000\-00");
        }
        if (numeros.Length == 14)
        {
            return Convert.ToUInt64(numeros).ToString(@"00\.000\.000\/0000\-00");
        }
        return documento ?? string.Empty;
    }
}
