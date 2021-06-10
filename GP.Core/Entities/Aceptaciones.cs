using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Aceptaciones")]
    public class Aceptaciones
    {
        [Key]
        public long AceptacionID { get; set; }
        public long? JugadaID { get; set; }
        public long? BanqueadaID { get; set; }

        [StringLength(50)]
        public string Usuario { get; set; }

        public decimal Monto { get; set; }
        public string Status { get; set; }

        public DateTime Fecha { get; set; }

        public string CajaNro { get; set; }
        public bool Activo { get; set; }

        public virtual Banquedas Banquedas { get; set; }

        public virtual Jugadas Jugadas { get; set; }
    }
}
