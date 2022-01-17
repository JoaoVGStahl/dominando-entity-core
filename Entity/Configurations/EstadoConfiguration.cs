using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations
{
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            
            builder.HasOne( p => p.Governador).WithOne( p => p.Estado).HasForeignKey<Governador>(p => p.EstadoReference); 

            builder
                .HasMany( p => p.Cidades)
                .WithOne(p => p.Estado)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // ? Faz com que não seja mais necessário utilizar o Include() de forma explicita
            builder.Navigation(p => p.Governador).AutoInclude();
        }
    }
}