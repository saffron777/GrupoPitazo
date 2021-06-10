using GP.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GP.Application.Web.Models
{
    public class MensajeViewModel
    {
        public string TicketNro { get; set; }
        public long CarreraId { get; set; }
        public long JugadaId { get; set; }
        public DateTime Fecha { get; set; }

        public string Status { get; set; }
        public string HipodromoId { get; set; }
        public decimal Monto { get; set; }
        public string TipoJugada { get; set; }

        public List<AceptacionesViewModels> Aceptaciones { get; set; }
    }
}
