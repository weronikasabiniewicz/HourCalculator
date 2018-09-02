using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator
{
    [AttributeUsage(System.AttributeTargets.Property)]
    public class DependentPropertiesAttribute : Attribute
    {
        private readonly string[] properties;

        public DependentPropertiesAttribute(params string[] dp)
        {
            properties = dp;
        }

        public string[] Properties
        {
            get
            {
                return properties;
            }
        }
    }
}
