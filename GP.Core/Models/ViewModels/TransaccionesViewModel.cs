using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GP.Core.Models.ViewModels
{
    public class TransaccionesViewModel
    {
        //public long TransaccionID { get; set; }
        [StringLength(50)]
        public string Usuario { get; set; }

        public int IdAgente { get; set; }
        [StringLength(50)]
        public string Agente { get; set; }
        [StringLength(50)]
        public string Moneda { get; set; }
        [StringLength(50)]
        public string IP { get; set; }

        public Guid? Token { get; set; }

        public long CarreraID { get; set; }

        public long? JugadaID { get; set; }

        public long? BanqueadaID { get; set; }

        public long? NotificacionID { get; set; }

        public long? AceptacionID { get; set; }
        public decimal? MontoJugada { get; set; }

        public decimal? MontoBanqueada { get; set; }

        public decimal? MontoAceptacion { get; set; }
        public decimal? Balance { get; set; }
        [StringLength(50)]
        public string Accion { get; set; }
        [StringLength(250)]
        public string Descripcion { get; set; }
        [StringLength(250)]
        public string ErrorMensaje { get; set; }
        [StringLength(4000)]
        public string JsonRequest { get; set; }

        public DateTime Fecha { get; set; }
    }
}
