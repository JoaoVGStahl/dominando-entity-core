using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using src.Domain;
using src.Extensions;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<UsuarioFuncao> UsuarioFuncoes { get; set; }
        public DbSet<DepartamentoRelatorio> DepartamentoRelatorio { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source=DESKTOP-LD0IN04\\DELLSERVER;Initial Catalog=PontoSys-Tips;User Id=sa;Password=@jr120401;Pooling=True;Application Name=EFCore")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ? Tabela sem chave primaria via Fluent API
            // modelBuilder.Entity<UsuarioFuncao>().HasNoKey();

            modelBuilder.Entity<DepartamentoRelatorio>(e =>
            {
                e.HasNoKey();

                e.ToView("vw_departamento_relatorio");

                e.Property(p => p.Departamento).HasColumnName("Descricao");
            });

            var properties = modelBuilder.Model.GetEntityTypes().SelectMany(p => p.GetProperties()).Where(p => p.ClrType == typeof(string) && p.GetColumnType() == null);

            foreach (var property in properties)
            {
                property.SetIsUnicode(false);
                // property.SetColumnType("varchar(100)");
            }

            modelBuilder.ToSnakeCaseNames();

            /*
            CREATE TABLE [colaboradores] (
            [id] int NOT NULL IDENTITY,
            [nome] varchar(max) NULL,
            [departamento_id] int NOT NULL,
            CONSTRAINT [pk_colaboradores] PRIMARY KEY ([id]),
            CONSTRAINT [fk_colaboradores_departamentos_departamento_id] FOREIGN KEY ([departamento_id]) REFERENCES [departamentos] ([id]) ON DELETE CASCADE
            );
            GO
            */
        }
    }
}