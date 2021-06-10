using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models.ViewModels
{
    public class CarrerasViewModel
    {
        public long CarreraID { get; set; }
        public string HipodromoID { get; set; }
        public int NumeroCarrera { get; set; }
        public int NumeroEjemplar { get; set; }
        public int? LlegadaEjemplar { get; set; }
        public string NombreEjemplar { get; set; }
        public bool EstatusEjemplar { get; set; }
        public DateTime FechaCarrera { get; set; }
        public TimeSpan HoraCarrera { get; set; }
        public DateTime? FechaCierreCarrera { get; set; }
        public TimeSpan? HoraCierreCarrera { get; set; }
    }

    public class CarrerasSelectModel
    {        
        public string HipodromoID { get; set; }
        public long CarreraID { get; set; }
        public int NumeroCarrera { get; set; }
        public string Info { get; set; }
    }
}
