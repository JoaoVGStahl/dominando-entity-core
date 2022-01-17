using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations
{
    public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
    {
        public void Configure(EntityTypeBuilder<Documento> builder)
        {
            // ? Faz com que o EFCore entenda a CPF como campo no banco de dados, porem numa consulta o campo que será populado é o _cpf
            builder
                .Property("_cpf")
                .HasColumnName("CPF")
                .HasMaxLength(11);
                //.HasField("_cpf");
        }
    }
}