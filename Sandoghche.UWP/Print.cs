using Sandoghche.Models;
using Sandoghche.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(Print))]

namespace Sandoghche.UWP
{
    public class Print : IPrint
    {
        void IPrint.Print(Order order,string receiptType,string ClientName)
        {

            //SilentPrint silentPrint = new SilentPrint();

            Print_UWP printing = new Print_UWP();
            printing.PrintUWpAsync(order,receiptType,ClientName);
        }
    }
}
