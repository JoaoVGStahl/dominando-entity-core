using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.domain
{
    public class Livro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        [Column(TypeName = ("VARCHAR(50)"))]
        public string Autor { get; set; }
    }
}