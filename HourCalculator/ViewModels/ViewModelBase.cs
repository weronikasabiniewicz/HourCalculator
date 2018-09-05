using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator
{
        public class ViewModelBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void RaiseProperty([CallerMemberName] string propertyName = null, List<string> calledProperties = null)
            {
                RaisePropertyChanged(propertyName);

                if (calledProperties == null)
                {
                    calledProperties = new List<string>();
                }

                calledProperties.Add(propertyName);

                PropertyInfo pInfo = GetType().GetProperty(propertyName);

                if (pInfo != null)
                {
                    foreach (DependentPropertiesAttribute ca in
                      pInfo.GetCustomAttributes(false).OfType<DependentPropertiesAttribute>())
                    {
                        if (ca.Properties != null)
                        {
                            foreach (string prop in ca.Properties)
                            {
                                if (prop != propertyName && !calledProperties.Contains(prop))
                                {
                                    RaiseProperty(prop, calledProperties);
                                }
                            }
                        }
                    }
                }
            }

            private void RaisePropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
}
