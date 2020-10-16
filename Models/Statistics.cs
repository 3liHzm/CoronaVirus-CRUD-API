using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaVirusApi.Models
{
    public class Statistics
    {
        public int id { get; set; }
        public string country { get; set; }
        public double cases { get; set; }
        public double cured { get; set; }
        public double deaths { get; set; }
    }
}
