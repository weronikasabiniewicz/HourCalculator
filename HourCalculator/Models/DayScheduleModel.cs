using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator.Models
{
    public class DayScheduleModel
    {
        public DateTime StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public DateTime PredictStopTime { get; set; }
        public TimeSpan SpentTime { get; set; }
        public TimeSpan? OverTime { get; set; }
        public List<PauseModel> Pauses { get; set; }

    }
}
