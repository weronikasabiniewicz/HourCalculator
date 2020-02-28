using HourCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator
{
    public class ScheduleService
    {
        private readonly TimeSpan _eightHours = new TimeSpan(8, 0, 0);

        public DayScheduleModel GetEmptyDaySchedule()
        {
            var model = new DayScheduleModel {StartTime = DateTime.Now.CutSecond()};
            model.PredictStopTime = model.StartTime.Add(_eightHours);
            model.Pauses = new List<PauseModel>();
            return model;
        }

        public void UpdatePredictStopTime(DayScheduleModel model)
        {
            model.PredictStopTime = model.StartTime.Add(_eightHours) + SumPausesTime(model.Pauses);
        }

        public void UpdateSpentTime(DayScheduleModel model)
        {
            model.SpentTime = (DateTime.Now.CutSecond() - model.StartTime) - SumPausesTime(model.Pauses);
            model.OverTime = model.SpentTime > _eightHours ? model.SpentTime - _eightHours : (TimeSpan?)null;
          
        }

        public void AddPause (DayScheduleModel model, string comment = null)
        {
            var pause = new PauseModel
            {
                StartTime = DateTime.Now.CutSecond(),
                Comment = comment
            };
            model.Pauses.Add(pause);
        }
        public void EndPause(DayScheduleModel model)
        {
            var pause = model.Pauses.FirstOrDefault(p => !p.StopTime.HasValue);
            if (pause != null) pause.StopTime = DateTime.Now.CutSecond();
            UpdatePredictStopTime(model);
        }

        private TimeSpan SumPausesTime(List<PauseModel> pauses)
        {
            var pauseTime = new TimeSpan();
            foreach (var pause in pauses)
            {
                var diff = (pause.StopTime ?? DateTime.Now.CutSecond()) - pause.StartTime;
               pauseTime = pauseTime.Add(diff);
            }

            return pauseTime;
        }
      
    
    }
}
