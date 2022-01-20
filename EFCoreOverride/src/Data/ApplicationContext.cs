using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using src.Domain;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                //.LogTo(Console.WriteLine)
                .UseSqlServer("Data Source=DESKTOP-LD0IN04\\DELLSERVER;Initial Catalog=PontoSys-Override;User Id=sa;Password=@jr120401;Pooling=True;Application Name=EFCore")
                //.ReplaceService<IQuerySqlGeneratorFactory, MySqlServerQueryGeneratorFactory>()
                .EnableSensitiveDataLogging();
        }
    }
}