using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathPlot
{
    public static class Api
    {
        public static void PrintPlot(Func<double,double> function)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).PrintGraph(function);
        }

        public static void UpdatePlot(Func<double, double> function)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateGraph(function);

        }
    }
}
