using System;
using System.IO;
using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Entity.Data
{
    public class ApplicatonContext : DbContext
    {
        private readonly StreamWriter _writer = new StreamWriter("EntityLog.txt", append: true);
        public DbSet<Departamento> Departamentos { get; set; }

        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=DESKTOP-LD0IN04\\DELLSERVER; Initial Catalog=PontoSys-02; User Id=sa;Password=@jr120401;pooling=true;";
            optionsBuilder
            //.UseSqlServer(strConnection, p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            //.UseSqlServer(strConnection)
                .UseSqlServer(
                    strConnection,
                        o => o
                            .MaxBatchSize(100) // ! MaxBatchSize é Recomendavel utilizar em redes instaveis, para evitar varias conexões com o banco
                            .CommandTimeout(30)
                            .EnableRetryOnFailure(5,TimeSpan.FromSeconds(10), null)) // ! Tentará 5 vezes durante com delay de 10 segundos
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
            /*
            .LogTo(Console.WriteLine,
            new[]{ CoreEventId.ContextInitialized,
            RelationalEventId.CommandExecuted},
            LogLevel.Information,
            DbContextLoggerOptions.LocalTime | DbContextLoggerOptions.SingleLine);
            */
            //.LogTo(_writer.WriteLine, LogLevel.Information);
            //.EnableDetailedErrors(); // ! Usado apenas em dubug, pois gera uma sobrecarga!
             // ! Usar apenas em debug!
        }


        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ! Filtro Global
            //modelBuilder.Entity<Departamento>().HasQueryFilter( p => !p.Excluido);
        }
    }
}