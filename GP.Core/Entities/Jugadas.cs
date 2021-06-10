using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Jugadas")]
    public class Jugadas
    {
        [Key]
        public long JugadaID { get; set; }

        public long CarreraID { get; set; }

        public int NumeroEjemplar { get; set; }

        public int? NumeroCarrera { get; set; }
        public int TipoJugadaID { get; set; }

        public decimal Monto { get; set; }

        public DateTime FechaJugada { get; set; }
        [StringLength(50)]
        public string Usuario { get; set; }
        [StringLength(50)]
        public string Agente { get; set; }

        public int IdAgente { get; set; }
        [StringLength(50)]
        public string Moneda { get; set; }
        public string CajaNro { get; set; }
        public string Status { get; set; }
        //[ForeignKey("FK_Jugadas_Carreras")]
        public Carreras Carreras { get; set; }
        
        public List<Banquedas> Banquedas { get; set; }
        //[ForeignKey("FK_Jugadas_TipoJugadas")]
        public TipoJugadas TipoJugadas { get; set; }

        public List<Aceptaciones> Aceptaciones { get; set; }
    }
}
