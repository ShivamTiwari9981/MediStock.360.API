using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.Common.constaints
{
    public static class DateHelper
    {
        public static DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }

        public static TimeSpan GetTimeSpan(int time)
        {
            return TimeSpan.FromMinutes(time);
        }


    }
}
