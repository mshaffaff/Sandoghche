using Sandoghche.Models;
namespace Sandoghche
{
    public interface IPrint
    {
        void Print(Order content, string receiptType);
    }
}
