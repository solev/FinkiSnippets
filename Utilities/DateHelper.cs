using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class DateHelper
    {
        public static DateTime GetCurrentTime()
        {
            // TODO: Time Zone
            return DateTime.Now.AddHours(0);
        }
    }
}
