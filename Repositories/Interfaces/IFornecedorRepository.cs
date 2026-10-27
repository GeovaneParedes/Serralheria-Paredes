using Serralheria_Paredes.Models;

namespace Serralheria_Paredes.Repositories.Interfaces;

public interface IFornecedorRepository : IRepository<Fornecedor>
{
    Task<IReadOnlyList<Fornecedor>> ObterTodosComFiltroAsync(string? termo, CancellationToken cancellationToken = default);
    Task<bool> DocumentoJaCadastradoAsync(string cpfOuCnpj, int? ignorarId = null, CancellationToken cancellationToken = default);
}
