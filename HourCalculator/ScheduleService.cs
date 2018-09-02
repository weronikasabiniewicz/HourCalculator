using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator
{
    public class ScheduleService
    {
        private TimeSpan EightHours = new TimeSpan(0, 1, 0);
        public ScheduleService()
        {

        }
        public DayScheduleModel GetEmptyDaySchedule()
        {
            var model = new DayScheduleModel();
            model.StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            DateTime.Now.Hour, DateTime.Now.Minute, 0);
            model.PredictStopTime = model.StartTime.Add(EightHours);
            return model;
        }

        public void Stop (DayScheduleModel model)
        {
            model.StopTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            DateTime.Now.Hour, DateTime.Now.Minute, 0);
        }

        public void UpdateStartTime(DayScheduleModel model)
        {
            model.PredictStopTime = model.StartTime.Add(EightHours);
        }

        public void UpdateSpentTime(DayScheduleModel model)
        {
            model.SpentTime = DateTime.Now - model.StartTime;
            if (model.SpentTime > EightHours)
            {
                model.OverTime = model.SpentTime - EightHours;
            }
        }
    
    }
}
