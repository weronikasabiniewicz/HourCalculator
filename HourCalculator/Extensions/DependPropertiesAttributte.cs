using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HourCalculator
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DependentPropertiesAttribute : Attribute
    {
        public DependentPropertiesAttribute(params string[] dp)
        {
            Properties = dp;
        }

        public string[] Properties { get; }
    }
}
