using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using Sandoghche.Droid;

[assembly: Dependency(typeof(SQLiteDb))]

namespace Sandoghche.Droid
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

