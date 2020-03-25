using System;
using System.Collections.Generic;
using System.Text;

namespace Sandoghche
{
    public interface IPrintPdf
    {
        void PrintPdf(byte[] content,int OrderId);
    }
}
