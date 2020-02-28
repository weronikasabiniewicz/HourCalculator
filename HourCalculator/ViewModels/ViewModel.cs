using System;
using System.Text;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using HourCalculator.Models;
using HourCalculator.ViewModels;

namespace HourCalculator
{
    public class ViewModel : ViewModelBase
    {
        private readonly NotifyIconHandler _notifyIcon;
        private readonly ScheduleService _scheduleService;
        private DayScheduleModel _model;

        private DateTime _nowDateTime;

        private TimeSpan? _overTime;

        private ICommand _pauseCommand;

        private TimeSpan? _spendTime;

        private ICommand _startCommand;

        private States _state;
        
        public ViewModel(NotifyIconHandler notifyIconHandler, ScheduleService scheduleService)
        {
            State = States.Started;
            _notifyIcon = notifyIconHandler;
            _scheduleService = scheduleService;
            _model = scheduleService.GetEmptyDaySchedule();
            ConfigureTimer();
            ConfigureNotifyIcon();
        }

        [DependentProperties("IsStartPropertyVisible", "IsStartButtonVisible", "IsOverTimeVisible", "IsPauseVisible")]
        public States State
        {
            get => _state;
            set
            {
                _state = value;
                RaiseProperty();
            }
        }

        public DateTime NowDateTime
        {
            get => _nowDateTime;
            set
            {
                _nowDateTime = value;
                RaiseProperty();
            }
        }

        [DependentProperties("EndTime")]
        public DateTime? StartTime
        {
            get => _model?.StartTime;

            set
            {
                _model.StartTime = value.Value;
                _scheduleService.UpdatePredictStopTime(_model);
                RaiseProperty();
            }
        }


        public DateTime? EndTime => _model?.PredictStopTime;

        public TimeSpan? SpendTime
        {
            get => _spendTime;
            set
            {
                _spendTime = value;
                RaiseProperty();
            }
        }

        [DependentProperties("IsOverTime", "SpendHoursColor", "IsOverTimeVisible")]
        public TimeSpan? OverTime
        {
            get => _overTime;
            set
            {
                _overTime = value;
                RaiseProperty();
            }
        }


        public bool IsOverTime => OverTime.HasValue;

        public Brush SpendHoursColor =>
            IsOverTime ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);


        public bool IsOverTimeVisible =>  IsOverTime;
        
        public ICommand StartCommand
        {
            get { return _startCommand ?? (_startCommand = new Command(param => Start())); }
        }

        public ICommand PauseCommand
        {
            get { return _pauseCommand ?? (_pauseCommand = new Command(param => Pause())); }
        }

        private void ConfigureTimer()
        {
            var timer = new Timer(1000);
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private void ConfigureNotifyIcon()
        {
            _notifyIcon.OnStartClicked = Start;
            _notifyIcon.OnNotifyIconClicked = PrepareSpendTimeMessage;
        }

        private string PrepareSpendTimeMessage()
        {
            var balloonTipText = new StringBuilder();

            if (State == States.Paused) balloonTipText.AppendLine("Application paused");
            if (SpendTime.HasValue)
                balloonTipText.AppendLine(SpendTime.Value.Hours + "h " + SpendTime.Value.Minutes + "m");

            if (IsOverTime)
            {
                balloonTipText.AppendLine();
                balloonTipText.Append("Overtime: ");
                balloonTipText.Append($"{OverTime.Value.Hours}h {OverTime.Value.Minutes}m");
            }

            return balloonTipText.ToString();
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            NowDateTime = DateTime.Now;
            _scheduleService.UpdateSpentTime(_model);
            SpendTime = _model.SpentTime;
            OverTime = _model.OverTime;
        }


        private void Start()
        {
            if (State == States.Paused)
            {
                _scheduleService.EndPause(_model);
                RaiseProperty(nameof(EndTime));
            }
            else
            {
                _model = _scheduleService.GetEmptyDaySchedule();
                RaiseProperty(nameof(StartTime));
            }

            State = States.Started;
        }

        private void Pause()
        {
            _scheduleService.AddPause(_model);
            State = States.Paused;
        }
    }
}