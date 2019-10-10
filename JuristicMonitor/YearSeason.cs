using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuristicMonitor
{
    public class YearSeason
    {
        public int year { get; set; }
        public int season { get; set; }

        public YearSeason(int year, int season)
        {
            this.year = year;
            this.season = season;
        }

        public YearSeason next()
        {
            return (season == 4) ? new YearSeason(year + 1, 0) : new YearSeason(year, season+1);
        }
    }
}
