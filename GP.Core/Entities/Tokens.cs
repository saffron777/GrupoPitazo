using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Tokens")]
    public class Tokens
    {
        [Key]
        public long TokenID { get; set; }

        public Guid Token { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; }

    }
}
