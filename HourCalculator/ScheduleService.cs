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
            model.StartTime = CutMilisecounds(DateTime.Now);
            model.PredictStopTime = model.StartTime.Add(EightHours);
            model.Pauses = new List<PauseModel>();
            return model;
        }

        public void Stop (DayScheduleModel model)
        {
            model.StopTime = CutMilisecounds(DateTime.Now);
        }

        public void UpdatePredictStopTime(DayScheduleModel model)
        {
            model.PredictStopTime = model.StartTime.Add(EightHours) + SumPausesTime(model.Pauses);
        }

        public void UpdateSpentTime(DayScheduleModel model)
        {
            model.SpentTime = (CutMilisecounds(DateTime.Now) - model.StartTime) - SumPausesTime(model.Pauses);
            if (model.SpentTime > EightHours)
            {
                model.OverTime = model.SpentTime - EightHours;
            }
        }

        public void AddPause (DayScheduleModel model, string comment = null)
        {
            var pause = new PauseModel
            {
                StartTime = CutMilisecounds(DateTime.Now),
                Comment = comment
            };
            model.Pauses.Add(pause);
        }
        public void EndPause(DayScheduleModel model)
        {
            var pause = model.Pauses.FirstOrDefault(p => !p.StopTime.HasValue);
            pause.StopTime = CutMilisecounds(DateTime.Now);
            UpdatePredictStopTime(model);
        }

        private TimeSpan SumPausesTime(List<PauseModel> pauses)
        {
            var pauseTime = new TimeSpan();
            foreach (var pause in pauses)
            {
                var diff = (pause.StopTime ?? CutMilisecounds(DateTime.Now)) - pause.StartTime;
               pauseTime = pauseTime.Add(diff);
            }

            return pauseTime;
        }
        //TODO: Make extension
        private DateTime CutMilisecounds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
            dateTime.Hour, dateTime.Minute, 0); ;
        }
    
    }
}
