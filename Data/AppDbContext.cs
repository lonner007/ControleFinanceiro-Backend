using Microsoft.EntityFrameworkCore;
using ControleFinanceiro_Backend.Models;
namespace ControleFinanceiro_Backend.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Conta> Contas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Meta> Metas { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Orcamento> Orcamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Usuario>().HasIndex(u => u.dsEmail).IsUnique();
        modelBuilder.Entity<Transacao>().HasOne(t => t.Conta).WithMany().HasForeignKey(t => t.cdConta).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Transacao>().HasOne(t => t.ContaDestino).WithMany().HasForeignKey(t => t.cdContaDestino).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
        modelBuilder.Entity<Transacao>().HasOne(t => t.Categoria).WithMany().HasForeignKey(t => t.cdCategoria).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
        modelBuilder.Entity<Orcamento>().HasOne(o => o.Categoria).WithMany().HasForeignKey(o => o.cdCategoria).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Orcamento>().HasIndex(o => new { o.cdUsuario, o.cdCategoria, o.nrMes, o.nrAno }).IsUnique();
    }
}
