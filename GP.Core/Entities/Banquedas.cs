using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Banquedas")]
    public class Banquedas
    {
        [Key]
        public long BanquedaID { get; set; }

        public long JugadaID { get; set; }

        public decimal Monto { get; set; }

        public int NumeroEjemplar { get; set; }

        public int TipoJugadaID { get; set; }

        public DateTime FechaBanqueada { get; set; }
        [StringLength(50)]
        public string Usuario { get; set; }
        [StringLength(50)]
        public string Agente { get; set; }

        public int IdAgente { get; set; }
        [StringLength(50)]
        public string Moneda { get; set; }
        public string CajaNro { get; set; }
        public string Status { get; set; }
        //[ForeignKey("FK_Banquedas_TipoJugadas")]
        public TipoJugadas TipoJugadas { get; set; }
        //[ForeignKey("FK_Banquedas_Jugadas")]
        public Jugadas Jugadas { get; set; }

        public List<Aceptaciones> Aceptaciones { get; set; }
    }
}
