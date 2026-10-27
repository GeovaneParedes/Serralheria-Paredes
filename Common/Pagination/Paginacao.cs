namespace Serralheria_Paredes.Common.Pagination;

public static class Paginacao
{
    public const int TamanhoPadrao = 20;
    public const int TamanhoMaximo = 100;

    public static (int Pagina, int Tamanho) Normalizar(int pagina, int tamanho)
    {
        return (Math.Max(1, pagina), Math.Clamp(tamanho, 1, TamanhoMaximo));
    }
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Pagina, int Tamanho, int Total)
{
    public int TotalPaginas => (int)Math.Ceiling(Total / (double)Tamanho);
}
