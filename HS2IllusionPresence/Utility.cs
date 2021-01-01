using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS2IllusionPresence
{
    public class Utility
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        public static long GetTimestamp(DateTime date)
        {
            return (long)date.Subtract(Epoch).TotalSeconds;
        }
    }
}
