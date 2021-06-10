using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("TipoJugadas")]
    public class TipoJugadas
    {
        [Key]
        public int TipoJugadaID { get; set; }
        [StringLength(5)]
        public string Codigo { get; set; }
        [StringLength(50)]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        
        public List<Jugadas> Jugadas { get; set; }
        
        public List<Banquedas> Banquedas { get; set; }
    }
}
