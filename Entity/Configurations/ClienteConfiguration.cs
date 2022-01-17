using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            // ? Faz o EFCore entender que Endereco é um tipo complexo de Cliente
            builder.OwnsOne(x => x.Endereco, end =>
                {
                    end.Property(p => p.Bairro).HasColumnName("Bairro");

                    end.ToTable("Enderecos");
                });
        }
    }
}