using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fitbit.Models
{
    public class ActivityReference
    {
        public String ActivityId { get; set;}
        public String Description { get; set; }
        public Double? Mets { get; set; }
        public String Name { get; set; }
    }
}
