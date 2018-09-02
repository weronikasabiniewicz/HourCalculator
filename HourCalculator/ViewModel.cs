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
        private NotifyIconHandler notifIcon;
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

        public ViewModel(NotifyIconHandler notifyIconHandler)
        {
            State = States.NotStarted;
            notifIcon = notifyIconHandler;
            ConfigureTimer();
            ConfigureNofifyIcon();

        }

        private void ConfigureTimer()
        {
            Timer _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void ConfigureNofifyIcon()
        {
            notifIcon.OnStartClicked = () => Start();
            notifIcon.OnNotifyIconClicked = PrepareSpendTimeMessage;

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

        private DateTime? _startTime;
        [DependentProperties("EndTime")]
        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                RaiseProperty();

            }
        }

        public DateTime? EndTime
        {
            get { return StartTime.HasValue ? StartTime.Value.Add(EightHours) : (DateTime?)null; }
        }

        private TimeSpan? _spendTime;
        [DependentProperties("SpendHoursColour", "IsOverTime", "OverTime", "IsOverTimeVisible")]
        public TimeSpan? SpendTime
        {
            get
            {
                return _spendTime;
            }
            set
            {
                _spendTime = value;
                RaiseProperty();
            }
        }

        public TimeSpan? OverTime
        {
            get
            {
                return IsOverTime ? SpendTime.Value.Subtract(EightHours) : (TimeSpan?)null;
            }
        }


        public bool IsOverTime
        {
            get { return this.SpendTime > EightHours; }
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
            SpendTime = DateTime.Now - StartTime;
        }


        private void Start()
        {
            State = States.Started;
            StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            DateTime.Now.Hour, DateTime.Now.Minute, 0);

        }

        private void Pause()
        {
            State = States.Paused;
        }

        private void Stop()
        {
            State = States.NotStarted;
        }
    }
}

