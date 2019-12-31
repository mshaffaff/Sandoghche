using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using Sandoghche.iOS;

[assembly: Dependency(typeof(SQLiteDb))]

namespace Sandoghche.iOS
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 
            var path = Path.Combine(documentsPath, "SandoghcheDb.db3");

            return new SQLiteAsyncConnection(path);
        }
    }
}

