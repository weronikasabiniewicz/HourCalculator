using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator.Models
{
    public class PauseModel
    {
        public DateTime StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public string Comment { get; set; }
    }
}
