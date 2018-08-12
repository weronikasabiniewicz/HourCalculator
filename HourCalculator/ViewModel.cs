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
        private string _timeValue;
        public string TimeValue
        {
            get { return _timeValue; }
            private set
            {
                _timeValue = value;
                NotifyPropertyChanged();
            }
        }

        public string DateValue
        {
            get { return DateTime.Now.ToShortDateString(); }
        }

        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
            private set
            {
                _startTime = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsStartPropertyVisible");
            }
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
            }
        }

        public Brush SpendHoursColour
        {
            get { return SpendTime > new TimeSpan(8, 0, 0) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green); }
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
        

        public ViewModel()
        {
            Timer _timer = new Timer(1000 );
            _timer.Start();
            _timer.Elapsed += _timer_Elapsed;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeValue = DateTime.Now.ToLongTimeString();
            SpendTime = DateTime.Now - StartTime;
        }
       

        public void Start()
        {
            StartTime = DateTime.Now;
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
