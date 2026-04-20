using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
  
     
    public class MeterReadingInfo
    {
        public DateTime? ReadingDate { get; set; }
        public decimal? PreviousReading { get; set; }
        public decimal? CurrentReading { get; set; }
    }
}