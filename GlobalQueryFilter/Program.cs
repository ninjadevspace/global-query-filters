using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GlobalQueryFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            using (var db = new Contexto())
            {
                var usuarios = db.Usuario.ToList();

                Console.WriteLine("======== Usuários com Query filter aplicado ========");

                foreach (var usuario in usuarios)
                {
                    Console.WriteLine($"Nome: {usuario.Nome} Deletado: {usuario.IsDeleted}");
                }

                usuarios = db.Usuario.IgnoreQueryFilters().ToList();

                Console.WriteLine("======== Usuários com Query filter ignorado ========");

                foreach (var usuario in usuarios)
                {
                    Console.WriteLine($"Nome: {usuario.Nome} Deletado: {usuario.IsDeleted}");
                }

            }
        }

        private static void SetupDatabase()
        {
            using (var db = new Contexto())
            {
                if (db.Database.EnsureCreated())
                {
                    db.Usuario.Add(new Usuario { Nome = "Usuario 1", IsDeleted = false });
                    db.Usuario.Add(new Usuario { Nome = "Usuario 2", IsDeleted = true });
                    db.Usuario.Add(new Usuario { Nome = "Usuario 3", IsDeleted = false });
                    db.Usuario.Add(new Usuario { Nome = "Usuario 4", IsDeleted = true });

                    db.SaveChanges();
                }
            }
        }
    }

    #region Entidade

    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool IsDeleted { get; set; }
    }

    #endregion

    #region Contexto EntityFramework

    public class Contexto : DbContext
    {
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(
                    @"Server=.;Database=Demo.QueryFilters;Trusted_Connection=True;ConnectRetryCount=0;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurando o Global Query Filter
            modelBuilder.Entity<Usuario>().HasQueryFilter(p => !p.IsDeleted);
        }
    }

    #endregion

}
