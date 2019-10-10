using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuristicMonitor
{
    class YearSeasonMonth
    {
        public int year;
        public int season;
        public int month;

        public YearSeasonMonth(int y=0, int s=0, int m=0)
        {
            year = y;
            season = s;
            month = m;
        }

        public YearSeasonMonth(DateTime t, bool Convert_to_taiwan=false)
        {
            year = t.Year;
            if (Convert_to_taiwan)
                year -= 1911;
            month = t.Month;
            if (month < 4)
                season = 1;
            else if(month < 7)
                season = 1;
            else if (month < 10)
                season = 2;
            else 
                season = 3;
        }

        public YearSeasonMonth NextSeason()
        {
            season++;
            if (season >= 5)
            {
                season = 1;
                year++;
            }
            return this;
        }

        public YearSeasonMonth NextMonth()
        {
            month++;
            if (month >= 13)
            {
                month = 1;
                year++;
            }
            return this;
        }

        static public bool operator<(YearSeasonMonth p1, YearSeasonMonth p2)
        {
            if (p1.year == p2.year)
            {
                if (p1.season == p2.season)
                    return (p1.month < p2.month);
                else
                    return (p1.season < p2.season);
            }
            else
                return (p1.year < p2.year);
        }

        static public bool operator >(YearSeasonMonth p1, YearSeasonMonth p2)
        {
            if (p1.year == p2.year)
            {
                if (p1.season == p2.season)
                    return (p1.month > p2.month);
                else
                    return (p1.season > p2.season);
            }
            else
                return (p1.year > p2.year);
        }
    }
}
