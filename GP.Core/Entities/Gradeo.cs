using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Gradeo")]
    public class Gradeo
    {
        [Key]
        public long GradeoId { get; set; }
        public string Ticket { get; set; }
        public string Usuario { get; set; }
        public string Tipo { get; set; }
        public string Detalle { get; set; }
        public Decimal Monto { get; set; }
        public Decimal Comision { get; set; }
        public string Status { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
    }
}
