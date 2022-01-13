using System.Collections.Generic;

namespace Entity.domain
{
    public class Departamento
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public bool Ativo { get; set; }

        public List<Funcionario> Funcionario { get; set; }
    }
}