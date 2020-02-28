using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HourCalculator.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseProperty([CallerMemberName] string propertyName = null,
            List<string> calledProperties = null)
        {
            RaisePropertyChanged(propertyName);

            if (calledProperties == null) calledProperties = new List<string>();

            calledProperties.Add(propertyName);

            var pInfo = GetType().GetProperty(propertyName);

            if (pInfo == null) return;
            foreach (var ca in
                pInfo.GetCustomAttributes(false).OfType<DependentPropertiesAttribute>())
            {
                if (ca.Properties == null) continue;
                foreach (var prop in ca.Properties)
                    if (prop != propertyName && !calledProperties.Contains(prop))
                        RaiseProperty(prop, calledProperties);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}