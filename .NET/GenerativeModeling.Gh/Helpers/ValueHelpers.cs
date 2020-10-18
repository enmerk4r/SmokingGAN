using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerativeModeling.Gh.Helpers
{
    public static class ValueHelpers
    {
        public static double Remap(this double value, double InputLow, double InputHigh, double OutputLow, double OutputHigh)
        {
            return ((value - InputLow) / (InputHigh - InputLow)) * (OutputHigh - OutputLow) + OutputLow;
        }

        public static double Remap(this double value, double OutputLow, double OutputHigh)
        {
            return value.Remap(0, 1, OutputLow, OutputHigh);
        }
    }
}
