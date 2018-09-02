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
    public class ViewModel : INotifyPropertyChanged
    {
        private TimeSpan EightHours = new TimeSpan(0, 1, 0);
        private NotifyIconHandler notifIcon;

        public ViewModel(NotifyIconHandler notifyIconHandler)
        {
            notifIcon = notifyIconHandler;
            Timer _timer = new Timer(1000);
            _timer.Start();
            _timer.Elapsed += _timer_Elapsed;
            ConfigureNofifyIcon();
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
                NotifyPropertyChanged();
            }
        }

        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsStartPropertyVisible");
                NotifyPropertyChanged("EndTime");

            }
        }

        public DateTime? EndTime
        {
            get { return StartTime.HasValue ? StartTime.Value.Add(EightHours) : (DateTime?)null; }
        }

        private TimeSpan? _spendTime;
        public TimeSpan? SpendTime
        {
            get
            {
                return _spendTime;
            }
            set
            {
                _spendTime = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("SpendHoursColour");
                NotifyPropertyChanged("IsOverTime");
                NotifyPropertyChanged("OverTime");
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

        public Visibility IsStartPropertyVisible
        {
            get { return StartTime.HasValue ? Visibility.Visible : Visibility.Hidden; }
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


      

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NowDateTime = DateTime.Now;
            SpendTime = DateTime.Now - StartTime;
          
        }


        public void Start()
        {
            StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            DateTime.Now.Hour, DateTime.Now.Minute, 0);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
