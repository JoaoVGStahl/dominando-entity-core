using Entity.domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entity.Configurations
{
    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            /* // ! TPH
            builder
                .ToTable("Pessoas")
                .HasDiscriminator<int>("TipoPessoas")
                .HasValue<Pessoa>(1)
                .HasValue<Instrutor>(2)
                .HasValue<Aluno>(50); */

            builder
                .ToTable("Pessoas");
        }
    }
    public class InstrutorConfiguration : IEntityTypeConfiguration<Instrutor>
    {
        public void Configure(EntityTypeBuilder<Instrutor> builder)
        {
            builder
                .ToTable("Instrutores");
        }
    }
    public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder
                .ToTable("Alunos");
        }
    }
}