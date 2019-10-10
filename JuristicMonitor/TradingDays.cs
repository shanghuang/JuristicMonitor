using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuristicMonitor
{
    public static class TradingDays
    {
        static DateTime[] Weekday_NoTrading = {new DateTime(2015,1,1), new DateTime(2015, 1, 2),
                    new DateTime(2015,2,16), new DateTime(2015, 2, 17), new DateTime(2015,2,18), new DateTime(2015, 2, 19),
                    new DateTime(2015,2,20), new DateTime(2015, 2, 23), new DateTime(2015,2,27), new DateTime(2015, 4, 3),
                    new DateTime(2015,4,6), new DateTime(2015, 1, 1), new DateTime(2015,6,19), new DateTime(2015, 9, 28),
                    new DateTime(2015,10,9),
                    new DateTime(2016, 1, 1), new DateTime(2016,2,3), new DateTime(2016, 2, 4),  new DateTime(2016, 2, 5),
                    new DateTime(2016, 2, 8), new DateTime(2016,2,9), new DateTime(2016, 2, 10),  new DateTime(2016, 2, 11),
                    new DateTime(2016, 2, 12), new DateTime(2016,2,29), new DateTime(2016, 4, 4),  new DateTime(2016, 4, 5),
                    new DateTime(2016, 5, 2), new DateTime(2016,6,9), new DateTime(2016, 6, 10),  new DateTime(2016, 9, 15),
                    new DateTime(2016, 9, 16), new DateTime(2016,10,10),
                    new DateTime(2017, 1, 2),  new DateTime(2017, 2, 5),new DateTime(2017, 1, 25),  new DateTime(2017, 1, 26),
                    new DateTime(2017, 1, 27), new DateTime(2017,1,30), new DateTime(2017, 1, 31),  new DateTime(2017, 2, 1),
                    new DateTime(2017, 2, 27), new DateTime(2017,2,28), new DateTime(2017, 4, 3),  new DateTime(2017, 4, 4),
                    new DateTime(2017, 5, 1), new DateTime(2017,5,29), new DateTime(2017, 5, 30),  new DateTime(2017, 10, 4),
                    new DateTime(2017, 10, 9), new DateTime(2017,10,10),

                    new DateTime(2018, 1, 1),  new DateTime(2018, 2, 13),new DateTime(2018, 2, 14),  new DateTime(2018, 2, 15),
                    new DateTime(2018, 2, 16), new DateTime(2018,2,19), new DateTime(2018, 2, 20),  new DateTime(2018, 2, 28),
                    new DateTime(2018, 4, 4), new DateTime(2018,4,5), new DateTime(2018, 4, 6),  new DateTime(2018, 5, 1),
                    new DateTime(2018, 6, 18), new DateTime(2018,9,24), new DateTime(2018, 10, 10),  new DateTime(2018, 12, 31),
                    new DateTime(2018, 10, 10), new DateTime(2018,10,11),

                    new DateTime(2019, 1, 1),  new DateTime(2019, 1, 2),new DateTime(2019, 1, 31),  new DateTime(2019, 2, 1),
                    new DateTime(2019, 2, 4), new DateTime(2019,2,5), new DateTime(2019, 2, 6),  new DateTime(2019, 2, 7),
                    new DateTime(2019, 2, 8), new DateTime(2019,2,28), new DateTime(2019, 3, 1),  new DateTime(2019, 4, 4),
                    new DateTime(2019, 4, 5), new DateTime(2019,5,1), new DateTime(2019, 6, 7),  new DateTime(2019, 9, 13),
                    new DateTime(2019, 10, 10), new DateTime(2019,10,11),

        };

        static DateTime[] Weekend_Trading = {
                    new DateTime(2016, 1, 30), new DateTime(2016, 6, 4), new DateTime(2016, 9, 10),
                    new DateTime(2017, 2, 18), new DateTime(2017, 6, 3), new DateTime(2017, 9, 30),
                    new DateTime(2018, 3, 31), new DateTime(2018, 12, 22), 
        };

        static HashSet<DateTime> WeekdayNotTrading = new HashSet<DateTime>(Weekday_NoTrading);
        static HashSet<DateTime> WeekendTrading = new HashSet<DateTime>(Weekend_Trading);

        public static Boolean IsTradingDay(DateTime day)
        {
            if(day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday)
            {
                return WeekendTrading.Contains(day);
            }
            else
            {
                return !WeekdayNotTrading.Contains(day);
            }
        }
    }
}
