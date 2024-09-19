using FrontPetConecta.Models;
using Microsoft.EntityFrameworkCore;

namespace FrontPetConecta.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<FuncionarioModel> Funcionarios { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Datasource=tds.db;Cache=Shared");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClienteModel>().ToTable("Clientes");
            modelBuilder.Entity<FuncionarioModel>().ToTable("Funcionarios");
            modelBuilder.Entity<ProdutoModel>().ToTable("Produtos");
            modelBuilder.Entity<ProdutoModel>()
       .HasKey(p => p.ProdutoId);


        }
    }


}
