using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models.ViewModels
{
    public class ParametersViewModel
    {
        public int ParameterId { get; set; }
        public string TableId { get; set; }
        public int ParameterCode { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public string Semantic { get; set; }
    }
}
