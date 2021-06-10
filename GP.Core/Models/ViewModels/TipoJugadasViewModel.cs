using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models.ViewModels
{
    public class TipoJugadasViewModel
    {
        public int TipoJugadaID { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
