using Sandoghche.Models;
using Sandoghche.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(Print))]

namespace Sandoghche.UWP
{
    public class Print : IPrint
    {
        void IPrint.Print(Order order,string receiptType)
        {
            Print_UWP printing = new Print_UWP();
            printing.PrintUWpAsync(order,receiptType);
        }
    }
}
