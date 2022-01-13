using System;
using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Entity.Data
{
    public class ApplicatonContextCidade : DbContext
    {
        public DbSet<Cidade> Cidades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=DESKTOP-LD0IN04\\DELLSERVER; Initial Catalog=Entity002; User Id=sa;Password=@jr120401;pooling=true";
            optionsBuilder
            .UseSqlServer(strConnection)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}