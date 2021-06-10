using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GP.Core.Models.ViewModels
{
    public class DatosHipodromoViewModel
    {
        public string TipoForm { get; set; }
        
        [Required(ErrorMessage = "*"), StringLength(50)]
        public string Locacion { get; set; }

        [Required(ErrorMessage = "*"), StringLength(1)]
        public string Clasificacion { get; set; }

        [Required(ErrorMessage = "*")]
        public string HipodromoID { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaCarrera { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:hh\\mm}", ApplyFormatInEditMode = true)]
        public TimeSpan HoraCarrera { get; set; }

        [Range(1, 15)]
        [Required(ErrorMessage = "*")]
        public int NumeroCarrera { get; set; }

        [Range(1, 21)]
        [Required(ErrorMessage = "*")]        
        public int CantidadEjemplar { get; set; }
        public DateTime? FechaCierreCarrera { get; set; }
        public TimeSpan? HoraCierreCarrera { get; set; }
        public List<DatosEjemplarViewModel> Ejemplares { get; set; }
        
        public List<HipodromosViewModels> Hipodromos { get; set; }

        //public List<CarrerasSelectModel> Carreras { get; set; }
    }

    public class DatosHipodromoViewForm
    {
        public string Operacion { get; set; }
        public string TipoForm { get; set; }

        [Required(ErrorMessage = "*"), StringLength(50)]
        public string Locacion { get; set; }

        [Required(ErrorMessage = "*"), StringLength(1)]
        public string Clasificacion { get; set; }

        [Required(ErrorMessage = "*")]
        public string HipodromoID { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaCarrera { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:hh\\mm}", ApplyFormatInEditMode = true)]
        public string HoraCarrera { get; set; }
        
        [Range(1, 15)]
        [Required(ErrorMessage = "*")]
        public int NumeroCarrera { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, 21)]
        public int CantidadEjemplar { get; set; }

        public int NumeroNuevoEjemplar { get; set; }
        public DatosEjemplarViewModel[] Ejemplares { get; set; }
    }

    public class DatosEjemplarViewModel
    {
        public long CarreraId { get; set; }
        [Required(ErrorMessage = "*")]
        [Range(1, 21)]
        public int NumeroEjemplar { get; set; }
        [Required(ErrorMessage = "*"), StringLength(50)]
        public string NombreEjemplar { get; set; }
        [Required(ErrorMessage = "*")]
        public bool Estatusejemplar { get; set; }
        [Range(0, 21)]
        public int? LlegadaEjemplar { get; set; }
    }
}
