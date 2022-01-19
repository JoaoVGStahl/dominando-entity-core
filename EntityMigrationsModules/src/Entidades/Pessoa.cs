using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EF.Core
{
    public partial class Pessoa
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Nome { get; set; }
    }
}
