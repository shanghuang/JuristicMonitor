using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuristicMonitor
{
    class EarningInfo
    {
        public double grossProfitMargin;
        public double operatingProfitMargin;
        public double netProfitMarginBeforeTax;
        public double netProfitMargin;
        public double eps;

        public EarningInfo(float p1=0, float p2=0, float p3=0, float p4=0, float p5=0)
        {
            grossProfitMargin = p1;
            operatingProfitMargin = p2;
            netProfitMarginBeforeTax = p3;
            netProfitMargin = p4;
            eps = p5;
        }
    }
}
