using HourCalculator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HourCalculator.ViewModels;

namespace HourCalculator
{
    public class ViewModel : ViewModelBase
    {
        private readonly NotifyIconHandler _notifIcon;
        private readonly ScheduleService _scheduleService;
        private DayScheduleModel _model;
        
        private States _state;
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

        public ViewModel(NotifyIconHandler notifyIconHandler, ScheduleService scheduleService)
        {
            State = States.NotStarted;
            _notifIcon = notifyIconHandler;
            _scheduleService = scheduleService;
            ConfigureTimer();
            ConfigureNotifyIcon();

        }

        private void ConfigureTimer()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private void ConfigureNotifyIcon()
        {
            _notifIcon.OnStartClicked = Start;
            _notifIcon.OnNotifyIconClicked = PrepareSpendTimeMessage;

        }
        private string PrepareSpendTimeMessage()
        {
            var balloonTipText = new StringBuilder("Please press start");

            if (State == States.Started || State == States.Paused)
            {                
                balloonTipText.Clear();
                if(State == States.Paused)
                {
                    balloonTipText.AppendLine("Application paused");
                }
                if (SpendTime.HasValue)
                {
                    balloonTipText.AppendLine(SpendTime.Value.Hours + "h " + SpendTime.Value.Minutes + "m");
                }

                if (IsOverTime)
                {
                    balloonTipText.AppendLine();
                    balloonTipText.Append("Overtime: ");
                    balloonTipText.Append($"{OverTime.Value.Hours}h {OverTime.Value.Minutes}m");
                }
            }
            return balloonTipText.ToString();
        }

        private DateTime _nowDateTime;
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

        private TimeSpan? _spendTime;
        public TimeSpan? SpendTime
        {
            get => _spendTime;
            set
            {
                _spendTime = value;
                RaiseProperty();
            }
        }
        
        private TimeSpan? _overTime;
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

        public Brush SpendHoursColor => IsOverTime ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

        public bool IsStartPropertyVisible => State != States.NotStarted;

        public bool IsOverTimeVisible => IsStartPropertyVisible && IsOverTime;

        public bool IsStartButtonVisible => State != States.Started;

        public bool IsPauseVisible => State == States.Started;

        private ICommand _startCommand;

        public ICommand StartCommand
        {
            get { return _startCommand ?? (_startCommand = new Command(param => Start())); }
        }

        private ICommand _pauseCommand;

        public ICommand PauseCommand
        {
            get { return _pauseCommand ?? (_pauseCommand = new Command(param => Pause())); }
        }

        private ICommand _stopCommand;

        public ICommand StopCommand
        {
            get { return _stopCommand ?? (_stopCommand = new Command(param => Stop())); }
        }


        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            NowDateTime = DateTime.Now;
            if (State == States.NotStarted) return;
            _scheduleService.UpdateSpentTime(_model);
            SpendTime = _model.SpentTime;
            OverTime = _model.OverTime;
        }


        private void Start()
        {
            if (State == States.NotStarted || State == States.Stopped)
            {
                _model = _scheduleService.GetEmptyDaySchedule();
                RaiseProperty(nameof(StartTime));
            }
            else if (State == States.Paused)
            {
                _scheduleService.EndPause(_model);
                RaiseProperty(nameof(EndTime));
            }
            
            State = States.Started;

        }

        private void Pause()
        {
            _scheduleService.AddPause(_model);
            State = States.Paused;
        }

        private void Stop()
        {
            _scheduleService.Stop(_model);
            State = States.Stopped;

        }
    }
}

