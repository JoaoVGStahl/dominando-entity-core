using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EF.Core;

#nullable disable

namespace EF.Core.Context
{
    public partial class PontoSys03Context : DbContext
    {
        public PontoSys03Context()
        {
        }

        public PontoSys03Context(DbContextOptions<PontoSys03Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Pessoa> Pessoas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-LD0IN04\\DELLSERVER; Initial Catalog=PontoSys-03; User Id=sa;Password=@jr120401;pooling=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.Property(e => e.Nome).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
