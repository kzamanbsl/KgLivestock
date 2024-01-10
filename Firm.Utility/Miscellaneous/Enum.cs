using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Utility.Miscellaneous
{
    public class Enum
    {
        public enum Gender
        {
            Male = 1,
            Female = 2
        }
        public enum LivestockType
        {
            Ox = 1,
            Cow = 2,
            Heifer = 3,
            Calf = 4
        }
        public enum Shift
        {
            Morning = 1,
            Day,
            Evening
        }
        public enum Status
        {
            Live = 1,
            Sold = 2,
            Missing = 3,
            Dead = 4

        }
    }
}
