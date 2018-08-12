using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
        

        public ViewModel()
        {
            Timer _timer = new Timer(1000);
            _timer.Start();
            _timer.Elapsed += _timer_Elapsed;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeValue = DateTime.Now.ToLongTimeString();
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
