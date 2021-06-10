using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Carreras")]
    public class Carreras
    {
        [Key]
        public long CarreraID { get; set; }
        [StringLength(3)]
        public string HipodromoID { get; set; }

        public bool StatusCarrera { get; set; }

        public DateTime FechaCarrera { get; set; }

        public TimeSpan HoraCarrera { get; set; }
        [StringLength(50)]
        public int NumeroCarrera { get; set; }
        [StringLength(50)]
        public string ColorEjemplar { get; set; }

        public int NumeroEjemplar { get; set; }
        [StringLength(50)]
        public string NombreEjemplar { get; set; }

        public bool EstatusEjemplar { get; set; }

        public int? LlegadaEjemplar { get; set; }

        public DateTime? FechaCierreCarrera { get; set; }

        public TimeSpan? HoraCierreCarrera { get; set; }

        public List<Jugadas> Jugadas { get; set; }
        //[ForeignKey("FK_Carreras_Hipodromos")]
        public Hipodromos Hipodromos { get; set; }

    }
}
