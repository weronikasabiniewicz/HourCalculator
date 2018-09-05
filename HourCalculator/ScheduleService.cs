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
        private TimeSpan EightHours = new TimeSpan(8, 0, 0);
        public ScheduleService()
        {

        }
        public DayScheduleModel GetEmptyDaySchedule()
        {
            var model = new DayScheduleModel();
            model.StartTime = DateTime.Now.CutSecound();
            model.PredictStopTime = model.StartTime.Add(EightHours);
            model.Pauses = new List<PauseModel>();
            return model;
        }

        public void Stop (DayScheduleModel model)
        {
            model.StopTime = DateTime.Now.CutSecound();
        }

        public void UpdatePredictStopTime(DayScheduleModel model)
        {
            model.PredictStopTime = model.StartTime.Add(EightHours) + SumPausesTime(model.Pauses);
        }

        public void UpdateSpentTime(DayScheduleModel model)
        {
            model.SpentTime = (DateTime.Now.CutSecound() - model.StartTime) - SumPausesTime(model.Pauses);
            model.OverTime = model.SpentTime > EightHours ? model.SpentTime - EightHours : (TimeSpan?)null;
          
        }

        public void AddPause (DayScheduleModel model, string comment = null)
        {
            var pause = new PauseModel
            {
                StartTime = DateTime.Now.CutSecound(),
                Comment = comment
            };
            model.Pauses.Add(pause);
        }
        public void EndPause(DayScheduleModel model)
        {
            var pause = model.Pauses.FirstOrDefault(p => !p.StopTime.HasValue);
            pause.StopTime = DateTime.Now.CutSecound();
            UpdatePredictStopTime(model);
        }

        private TimeSpan SumPausesTime(List<PauseModel> pauses)
        {
            var pauseTime = new TimeSpan();
            foreach (var pause in pauses)
            {
                var diff = (pause.StopTime ?? DateTime.Now.CutSecound()) - pause.StartTime;
               pauseTime = pauseTime.Add(diff);
            }

            return pauseTime;
        }
      
    
    }
}
