using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entity.domain
{
    [Table("TabelaAtributos")]
    [Index(nameof(Descricao),nameof(Id),IsUnique = true)]
    [Comment("Meu Comentário Data Annotations")]
    public class Atributo
    {
        // ? Identity => EFCore deixa de gerar valor e passa a responsabilidade para o banco
        // ? None => EFcore impede que o banco de dados gere autoincremeto do lado dele
        // ? Computed => Não irá incluir esse campo nos insert ou updates, porem irá ler as informações
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("MinhaDescricao", TypeName = "VARCHAR(100)")]
        [Comment("Meu Comentário Data Annotations - Campo descricao")]
        public string Descricao { get; set; }

        //[Required]
        [MaxLength(255)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string Observacao { get; set; }
    }

    [Keyless]
    public class RelatorioFinanceiro{
        public string Descricao { get; set; }
        public decimal Total { get; set; }
        public DateTime Data { get; set; }
    }

    public class Aeroporto{
        public int Id { get; set; }

        public string Nome { get; set; }

        [NotMapped]
        public string PropriedadeTeste { get; set; }

        [InverseProperty("AeroportoPartida")]
        public ICollection<Voo> VoosDePartida { get; set; }
        [InverseProperty("AeroportoChegada")]
        public ICollection<Voo> VoosDeChegada { get; set; }
    }
    [NotMapped]
    public class Voo{
        public int Id { get; set; }

        public string Descricao { get; set; }

        public Aeroporto AeroportoPartida { get; set; }
        public Aeroporto AeroportoChegada { get; set; }
    }
}