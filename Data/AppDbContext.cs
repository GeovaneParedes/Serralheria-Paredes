using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serralheria_Paredes.Models;
using Serralheria_Paredes.Models.Common;

namespace Serralheria_Paredes.Data;

public class AppDbContext : IdentityDbContext<Usuario, Perfil, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

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

        // Aplica automaticamente configurações de outras entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
