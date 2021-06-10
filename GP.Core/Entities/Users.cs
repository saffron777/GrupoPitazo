using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public int UserID { get; set; }
        [StringLength(50)]
        public string UserName { get; set; }
        [StringLength(50)]
        public string Password { get; set; }
        [StringLength(50)]
        public string Nombre { get; set; }
        [StringLength(50)]
        public string Apellido { get; set; }

        public DateTime FechaCreacion { get; set; }

        public long TokenID { get; set; }

        public bool Activo { get; set; }

    }
}
