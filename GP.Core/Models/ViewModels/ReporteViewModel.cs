using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models.ViewModels
{
    public class ReporteViewModel
    {
        public string Ticket { get; set; }
        public string Tipo { get; set; }
        public string Detalle  {get; set; }
        public Decimal Monto { get; set; }
        public Decimal Comision { get; set; }
        public string Status { get; set; }
    }
}
