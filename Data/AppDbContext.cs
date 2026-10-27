using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Common;

namespace Serralheria_Paredes.Data;

public class AppDbContext : IdentityDbContext<Usuario, Perfil, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Insumo> Insumos => Set<Insumo>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Orcamento> Orcamentos => Set<Orcamento>();
    public DbSet<ItemOrcamento> ItensOrcamento => Set<ItemOrcamento>();
    public DbSet<OrdemServico> OrdensServico => Set<OrdemServico>();
    public DbSet<HistoricoOrdemServico> HistoricosOrdensServico => Set<HistoricoOrdemServico>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity<int>
                     || e.Entity is BaseEntity<Guid>
                     || e.Entity is BaseEntity<long>
                     || e.Entity is Usuario);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Properties.Any(p => p.Metadata.Name == "CreatedAt"))
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento limpo das tabelas do Identity no PostgreSQL
        modelBuilder.Entity<Usuario>(b =>
        {
            b.ToTable("usuarios");
            b.Property(u => u.NomeCompleto).HasMaxLength(150).IsRequired();
            b.Property(u => u.Cargo).HasMaxLength(80);
        });

        modelBuilder.Entity<Perfil>(b =>
        {
            b.ToTable("perfis");
            b.Property(p => p.Descricao).HasMaxLength(250);
        });

        modelBuilder.Entity<IdentityUserRole<int>>(b => b.ToTable("usuario_perfis"));
        modelBuilder.Entity<IdentityUserClaim<int>>(b => b.ToTable("usuario_claims"));
        modelBuilder.Entity<IdentityUserLogin<int>>(b => b.ToTable("usuario_logins"));
        modelBuilder.Entity<IdentityUserToken<int>>(b => b.ToTable("usuario_tokens"));
        modelBuilder.Entity<IdentityRoleClaim<int>>(b => b.ToTable("perfil_claims"));

        // Mapeamento da entidade Cliente
        modelBuilder.Entity<Cliente>(b =>
        {
            b.ToTable("clientes");
            b.HasKey(c => c.Id);
            b.Property(c => c.NomeOuRazaoSocial).HasMaxLength(150).IsRequired();
            b.Property(c => c.NomeFantasia).HasMaxLength(150);
            b.Property(c => c.CpfOuCnpj).HasMaxLength(20).IsRequired();
            b.Property(c => c.RgOuInscricaoEstadual).HasMaxLength(25);
            b.Property(c => c.Telefone).HasMaxLength(20).IsRequired();
            b.Property(c => c.Celular).HasMaxLength(20);
            b.Property(c => c.Email).HasMaxLength(100);
            b.Property(c => c.EnderecoCep).HasMaxLength(10);
            b.Property(c => c.EnderecoLogradouro).HasMaxLength(150);
            b.Property(c => c.EnderecoNumero).HasMaxLength(20);
            b.Property(c => c.EnderecoComplemento).HasMaxLength(100);
            b.Property(c => c.EnderecoBairro).HasMaxLength(80);
            b.Property(c => c.EnderecoCidade).HasMaxLength(80);
            b.Property(c => c.EnderecoEstado).HasMaxLength(2);
            b.Property(c => c.Observacoes).HasMaxLength(500);

            b.HasIndex(c => c.CpfOuCnpj);
            b.HasIndex(c => c.NomeOuRazaoSocial);
        });

        // Mapeamento da entidade Fornecedor
        modelBuilder.Entity<Fornecedor>(b =>
        {
            b.ToTable("fornecedores");
            b.HasKey(f => f.Id);
            b.Property(f => f.RazaoSocial).HasMaxLength(150).IsRequired();
            b.Property(f => f.NomeFantasia).HasMaxLength(150);
            b.Property(f => f.CpfOuCnpj).HasMaxLength(20).IsRequired();
            b.Property(f => f.InscricaoEstadual).HasMaxLength(25);
            b.Property(f => f.Telefone).HasMaxLength(20).IsRequired();
            b.Property(f => f.Email).HasMaxLength(100);
            b.Property(f => f.ContatoResponsavel).HasMaxLength(80);
            b.Property(f => f.EnderecoCep).HasMaxLength(10);
            b.Property(f => f.EnderecoLogradouro).HasMaxLength(150);
            b.Property(f => f.EnderecoNumero).HasMaxLength(20);
            b.Property(f => f.EnderecoComplemento).HasMaxLength(100);
            b.Property(f => f.EnderecoBairro).HasMaxLength(80);
            b.Property(f => f.EnderecoCidade).HasMaxLength(80);
            b.Property(f => f.EnderecoEstado).HasMaxLength(2);
            b.Property(f => f.Observacoes).HasMaxLength(500);

            b.HasIndex(f => f.CpfOuCnpj);
            b.HasIndex(f => f.RazaoSocial);
        });

        modelBuilder.Entity<Insumo>(b =>
        {
            b.ToTable("insumos");
            b.Property<uint>("xmin").IsRowVersion();
            b.HasKey(i => i.Id);
            b.Property(i => i.Codigo).HasMaxLength(40).IsRequired();
            b.Property(i => i.Descricao).HasMaxLength(150).IsRequired();
            b.Property(i => i.ComprimentoBarraMetros).HasPrecision(10, 3);
            b.Property(i => i.EstoqueAtual).HasPrecision(12, 3);
            b.Property(i => i.EstoqueMinimo).HasPrecision(12, 3);
            b.Property(i => i.CustoUnitario).HasPrecision(12, 2);
            b.HasIndex(i => i.Codigo).IsUnique();
            b.HasIndex(i => i.Descricao);
            b.HasOne(i => i.Fornecedor)
                .WithMany()
                .HasForeignKey(i => i.FornecedorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<MovimentacaoEstoque>(b =>
        {
            b.ToTable("movimentacoes_estoque");
            b.HasKey(m => m.Id);
            b.Property(m => m.Quantidade).HasPrecision(12, 3);
            b.Property(m => m.SaldoAnterior).HasPrecision(12, 3);
            b.Property(m => m.SaldoPosterior).HasPrecision(12, 3);
            b.Property(m => m.Motivo).HasMaxLength(250).IsRequired();
            b.HasIndex(m => new { m.InsumoId, m.CreatedAt });
            b.HasOne(m => m.Insumo)
                .WithMany()
                .HasForeignKey(m => m.InsumoId)
                .OnDelete(DeleteBehavior.Restrict);
                b.HasOne(m => m.OrdemServico)
                    .WithMany(o => o.MovimentacoesEstoque)
                    .HasForeignKey(m => m.OrdemServicoId)
                    .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Orcamento>(b =>
        {
            b.ToTable("orcamentos");
            b.HasKey(o => o.Id);
            b.Property(o => o.Numero).HasMaxLength(30).IsRequired();
            b.Property(o => o.Titulo).HasMaxLength(150).IsRequired();
            b.Property(o => o.Descricao).HasMaxLength(1000);
            b.Property(o => o.SubtotalMateriais).HasPrecision(12, 2);
            b.Property(o => o.ValorMaoDeObra).HasPrecision(12, 2);
            b.Property(o => o.PercentualMargem).HasPrecision(5, 2);
            b.Property(o => o.ValorTotal).HasPrecision(12, 2);
            b.HasIndex(o => o.Numero).IsUnique();
            b.HasOne(o => o.Cliente)
                .WithMany()
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ItemOrcamento>(b =>
        {
            b.ToTable("itens_orcamento");
            b.HasKey(i => i.Id);
            b.Property(i => i.Descricao).HasMaxLength(200).IsRequired();
            b.Property(i => i.Quantidade).HasPrecision(12, 3);
            b.Property(i => i.ComprimentoUnitarioMetros).HasPrecision(10, 3);
            b.Property(i => i.PercentualPerda).HasPrecision(5, 2);
            b.Property(i => i.CustoUnitario).HasPrecision(12, 2);
            b.Property(i => i.Subtotal).HasPrecision(12, 2);
            b.HasOne(i => i.Orcamento)
                .WithMany(o => o.Itens)
                .HasForeignKey(i => i.OrcamentoId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(i => i.Insumo)
                .WithMany()
                .HasForeignKey(i => i.InsumoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OrdemServico>(b =>
        {
            b.ToTable("ordens_servico");
            b.HasKey(o => o.Id);
            b.Property(o => o.Numero).HasMaxLength(30).IsRequired();
            b.Property(o => o.ObservacoesProducao).HasMaxLength(1000);
            b.HasIndex(o => o.Numero).IsUnique();
            b.HasIndex(o => o.OrcamentoId).IsUnique();
            b.HasOne(o => o.Orcamento)
                .WithOne(o => o.OrdemServico)
                .HasForeignKey<OrdemServico>(o => o.OrcamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HistoricoOrdemServico>(b =>
        {
            b.ToTable("historico_ordens_servico");
            b.HasKey(h => h.Id);
            b.Property(h => h.Observacao).HasMaxLength(500);
            b.HasIndex(h => new { h.OrdemServicoId, h.CreatedAt });
            b.HasOne(h => h.OrdemServico)
                .WithMany(o => o.Historico)
                .HasForeignKey(h => h.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}
