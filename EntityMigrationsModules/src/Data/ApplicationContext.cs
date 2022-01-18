using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using src.Domain;

namespace src.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data Source=DESKTOP-LD0IN04\\DELLSERVER; Initial Catalog=PontoSys-03; User Id=sa;Password=@jr120401;pooling=true;";
            optionsBuilder.
                UseSqlServer(strConnection)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>(conf =>{
                conf.HasKey( p => p.Id);
                conf.Property( p => p.Nome).HasMaxLength(50).IsUnicode(false);
            }); 
        }
    }
}