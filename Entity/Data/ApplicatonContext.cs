using System;
using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Entity.Data
{
    public class ApplicatonContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }

        public DbSet<Funcionario> Funcionarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=DESKTOP-LD0IN04\\DELLSERVER; Initial Catalog=Entity002; User Id=sa;Password=@jr120401;pooling=true;";
            optionsBuilder
            .UseSqlServer(strConnection)
            .EnableSensitiveDataLogging()
            //.UseLazyLoadingProxies()
            .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}