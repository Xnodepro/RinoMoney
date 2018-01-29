using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMONEY
{
   static  class ArgumentsChek
    {
        public static EventHandler ValueChanged = delegate { };
        private static string _value;

        public static string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueChanged(null, EventArgs.Empty);
            }
        }
    }
}
