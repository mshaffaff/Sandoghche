using SQLite;

namespace Sandoghche
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    
    }
    
}

