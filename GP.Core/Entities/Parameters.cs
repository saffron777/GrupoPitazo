using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Parameters")]
    public class Parameters
    {
        [Key]
        public int ParameterId { get; set; }
        public string TableId { get; set; }
        public int ParameterCode { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public string Semantic { get; set; }
    }
}
