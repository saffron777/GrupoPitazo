using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GP.Core.Models.ViewModels
{
    public class AceptacionesViewModels
    {
        public long AceptacionID { get; set; }
        public long? JugadaID { get; set; }
        public long? BanqueadaID { get; set; }
        public string HipodromoId { get; set; }
        public long CarreraId { get; set; }
        public string CarreraNum { get; set; }
        public string EjemplarNum { get; set; }
        public string TipoJugada { get; set; }
        public int IdAgente { get; set; }
        public string Agente { get; set; }
        public string Moneda { get; set; }
        [StringLength(50)]
        public string Usuario { get; set; }
        public string UsuarioApuesta { get; set; }
        public decimal Monto { get; set; }

        public decimal MontoJugada { get; set; }

        public string Status { get; set; }

        public DateTime Fecha { get; set; }

        public string CajaNro { get; set; }
        public bool Activo { get; set; }

    }
}
