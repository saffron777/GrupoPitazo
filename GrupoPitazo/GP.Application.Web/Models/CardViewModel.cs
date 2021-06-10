using GP.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GP.Application.Web.Models
{
    public class CardViewModel
    {
        public string id { get; set; }
        public string UserId { get; set; }
        public string HipodromoId { get; set; }
        public string HipodromoName { get; set; }
        public long CarreraId { get; set; }
        public long JugadaId { get; set; }
        public long BanquedaId { get; set; }
        public string CarreraNum { get; set; }
        public string EjemplarNum { get; set; }
        public string EjemplarName { get; set; }
        public string TipoJugada { get; set; }
        public decimal Monto { get; set; }
        public decimal AceptacionesMonto { get; set; }
        public int IdAgente { get; set; }
        public string Agente { get; set; }
        public string Moneda { get; set; }
        public string CajaNro { get; set; }
        public string Status { get; set; }
        public string UsuarioApuesta { get; set; }
        public List<AceptacionesViewModels> Aceptaciones { get; set; }


    }
}
