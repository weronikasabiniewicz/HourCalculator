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

namespace HourCalculator
{
    public class ViewModel : ViewModelBase
    {
        private TimeSpan EightHours = new TimeSpan(0, 1, 0);
        private NotifyIconHandler _notifIcon;
        private ScheduleService _scheduleService;
        private DayScheduleModel _model;
        
        private States _state;
        [DependentProperties("IsStartPropertyVisible", "IsStartButtonVisible", "IsOverTimeVisible", "IsPauseVisible")]
        public States State
        {
            get { return _state; }
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
            ConfigureNofifyIcon();

        }

        private void ConfigureTimer()
        {
            Timer _timer = new Timer(1000 * 30);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void ConfigureNofifyIcon()
        {
            _notifIcon.OnStartClicked = () => Start();
            _notifIcon.OnNotifyIconClicked = PrepareSpendTimeMessage;

        }
        private string PrepareSpendTimeMessage()
        {
            var ballonTipText = new StringBuilder("Please press start");

            if (SpendTime.HasValue)
            {
                ballonTipText.Clear();
                ballonTipText.Append(SpendTime.Value.Hours + "h " + SpendTime.Value.Minutes + "m");
            }

            if (IsOverTime)
            {
                ballonTipText.AppendLine();
                ballonTipText.Append("Overtime: ");
                ballonTipText.Append(OverTime.Value.Hours + "h " + OverTime.Value.Minutes + "m");
            }
            return ballonTipText.ToString();
        }

        private DateTime _nowDateTime;
        public DateTime NowDateTime
        {
            get { return _nowDateTime; }
            set
            {
                _nowDateTime = value;
                RaiseProperty();
            }
        }

        [DependentProperties("EndTime")]
        public DateTime? StartTime
        {
            get
            {
                return _model != null ? _model.StartTime : (DateTime?)null;
            }
            set
            {
                _model.StartTime = value.Value;
                _scheduleService.UpdatePredictStopTime(_model);
                RaiseProperty();
            }
        }

        
        public DateTime? EndTime
        {
            get { return _model != null ? _model.PredictStopTime : (DateTime?)null; }
        }

        private TimeSpan? _spendTime;
        public TimeSpan? SpendTime
        {
            get { return _spendTime; }
            set
            {
                _spendTime = value;
                RaiseProperty();
            }
        }
        
        private TimeSpan? _overTime;
        [DependentProperties("IsOverTime", "SpendHoursColour", "IsOverTimeVisible")]
        public TimeSpan? OverTime
        {
            get { return _overTime; }
            set
            {
                _overTime = value;
                RaiseProperty();
            }
        }


        public bool IsOverTime
        {
            get { return OverTime.HasValue; }
        }

        public Brush SpendHoursColour
        {
            get { return IsOverTime ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green); }
        }

        public bool IsStartPropertyVisible
        {
            get { return State != States.NotStarted; }
        }
        public bool IsOverTimeVisible
        {
            get { return IsStartPropertyVisible && IsOverTime; }
        }

        public bool IsStartButtonVisible
        {
            get { return State != States.Started; }
        }

        public bool IsPauseVisible
        {
            get { return State == States.Started; }
        }

        private ICommand _startCommand;

        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                {
                    _startCommand = new Command(param => this.Start());
                }
                return _startCommand;
            }
        }

        private ICommand _pauseCommand;

        public ICommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                {
                    _pauseCommand = new Command(param => this.Pause());
                }
                return _pauseCommand;
            }
        }

        private ICommand _stopCommand;

        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new Command(param => this.Stop());
                }
                return _stopCommand;
            }
        }


        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NowDateTime = DateTime.Now;
            if( State != States.NotStarted)
            {
                _scheduleService.UpdateSpentTime(_model);
                SpendTime = _model.SpentTime;
                OverTime = _model.OverTime;
            }
        }


        private void Start()
        {
            if (State == States.NotStarted || State == States.Stopped)
            {
                _model = _scheduleService.GetEmptyDaySchedule();
                RaiseProperty("StartTime");
            }
            else if (State == States.Paused)
            {
                _scheduleService.EndPause(_model);
                RaiseProperty("EndTime");
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

