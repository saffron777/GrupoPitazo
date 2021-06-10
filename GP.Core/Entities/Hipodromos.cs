using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Hipodromos")]
    public class Hipodromos
    {
        [Key]
        [StringLength(3)]
        public string HipodromoID { get; set; }
        [StringLength(50)]
        public string Nombre { get; set; }
        [StringLength(1)]
        public string Clasificacion { get; set; }
        [StringLength(50)]
        public string Locacion { get; set; }

        public List<Carreras> Carreras { get; set; }
    }
}
