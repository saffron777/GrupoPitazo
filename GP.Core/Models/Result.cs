using GP.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models
{
    public class Result
    {
        public int Codigo { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }

        public List<DatosEjemplarViewModel> Ejemplares { get; set; }
    }
}
