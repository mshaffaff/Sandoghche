using System.IO;
using System.Threading.Tasks;

namespace Sandoghche
{
    public interface ISaveWindows
    {
        Task Save(string filename, string contentType, MemoryStream stream);
    }
}
