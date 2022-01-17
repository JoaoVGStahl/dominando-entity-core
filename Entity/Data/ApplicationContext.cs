using System;
using System.IO;
using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Entity.Data
{
    public class ApplicationContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("EntityLog.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }

        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=DESKTOP-LD0IN04\\DELLSERVER; Initial Catalog=PontoSys-02; User Id=sa;Password=@jr120401;pooling=true;";
            optionsBuilder
                .UseSqlServer(strConnection)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
            /*
            .UseSqlServer(strConnection, p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .UseSqlServer(strConnection)
            
                .UseSqlServer(
                    strConnection,
                        o => o
                            .MaxBatchSize(100) // ! MaxBatchSize é Recomendavel utilizar em redes instaveis, para evitar varias conexões com o banco
                            .CommandTimeout(30)
                            .EnableRetryOnFailure(5,TimeSpan.FromSeconds(10), null)) // ! Tentará 5 vezes durante com delay de 10 segundos
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
          
            .LogTo(Console.WriteLine,
            new[]{ CoreEventId.ContextInitialized,
            RelationalEventId.CommandExecuted},
            LogLevel.Information,
            DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine);
            
            .LogTo(_writer.WriteLine, LogLevel.Information);
            .EnableDetailedErrors(); // ! Usado apenas em dubug, pois gera uma sobrecarga!
             */
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ! Filtro Global
            // * modelBuilder.Entity<Departamento>().HasQueryFilter( p => !p.Excluido);

            // ? SQL_Latin1_General => é o designador de agrupamento => Regras basicas de agrupamento
            // ? CP1 => Windows 1252 
            // ? CI => não ira diferenciar maiscula de minuscula / CS é o inverso ( Joao => joao)
            // ? AI => irá ignorar acentuação / CS => irá validar acentuação ( João => Joao)

            /*
             ! Configuração a nivel global!
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

             ! Configuração Propriedade!
            modelBuilder.Entity<Departamento>().Property(p => p.Descricao).UseCollation("SQL_Latin1_General_CP1_CS_AS");
            

             ? Por padrão é criado em int porem suporta apenas long, decimal, byte => limitação do banco de dados
             ? basicamento é um Identity, porem é mais flexivel
            modelBuilder
                .HasSequence<int>("MinhaSequencia", "sequencias")
                .StartsAt(1)
                .IncrementsBy(2)
                .HasMin(1)
                .HasMax(10)
                .IsCyclic();

            modelBuilder.Entity<Departamento>().Property(p => p.Id).HasDefaultValueSql("NEXT VALUE FOR sequencias.MinhaSequencia");
            */

            // ? Criação de Indices e Indices Compostos
            modelBuilder
                .Entity<Departamento>()
                .HasIndex(p => new {p.Descricao, p.Ativo})
                .HasDatabaseName("idx_meu_indice_composto")
                .HasFilter("Descricao IS NOT NULL")
                // ? Definir Fator de Preenchimento usado para ajustar o armazentamento e o desempennho dos indices / Irá deixar 20% da folha para beneficio proprio
                .HasFillFactor(80) 
                .IsUnique(); // ! Evita que o indice seja duplicado
        }

        // ? Flush StreamWriter
        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();

        }


    }
}