using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models.ViewModels
{
    public class BanqueadaViewModel
    {
        public long BanqueadaID { get; set; }
        public long JugadaID { get; set; }        
        public int NumeroEjemplar { get; set; }
        public int TipoJugada { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaBanqueada { get; set; }
        public string Usuario { get; set; }
        public string Agente { get; set; }
        public int IdAgente { get; set; }
        public string Moneda { get; set; }
        public string CajaNro { get; set; }
        public string Status { get; set; }
        public JugadasViewModel Jugadas { get; set; }       
    }
}
