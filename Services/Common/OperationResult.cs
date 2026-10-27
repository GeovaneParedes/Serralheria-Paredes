namespace Serralheria_Paredes.Services.Common;

/// <summary>
/// Estrutura de retorno padronizada para a Camada de Serviços.
/// Permite que os Controllers permaneçam extremamente magros (Thin Controllers),
/// apenas inspecionando o sucesso e populando o ModelState ou redirecionando.
/// </summary>
public class OperationResult
{
    public bool Success { get; protected set; }
    public string? Message { get; protected set; }
    public List<string> Errors { get; } = new();

    public static OperationResult Ok(string? message = null) => new() { Success = true, Message = message };
    public static OperationResult Fail(string error) => new() { Success = false, Errors = { error } };
    public static OperationResult Fail(IEnumerable<string> errors)
    {
        var result = new OperationResult { Success = false };
        result.Errors.AddRange(errors);
        return result;
    }
}

public class OperationResult<T> : OperationResult
{
    public T? Data { get; private set; }

    public static OperationResult<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public new static OperationResult<T> Fail(string error) =>
        new() { Success = false, Errors = { error } };

    public new static OperationResult<T> Fail(IEnumerable<string> errors)
    {
        var result = new OperationResult<T> { Success = false };
        result.Errors.AddRange(errors);
        return result;
    }
}
