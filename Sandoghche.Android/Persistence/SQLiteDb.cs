using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using Sandoghche.Droid;
using SQLiteNetExtensions.Attributes;


[assembly: Dependency(typeof(SQLiteDb))]

namespace Sandoghche.Droid
{
	public class SQLiteDb : ISQLiteDb
	{
		public SQLiteAsyncConnection GetConnection()
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documentsPath, "SandoghcheDb3.db3");

			return new SQLiteAsyncConnection(path);
		}
	}
}

//var db = new SQLiteConnection(new SQLitePlatformAndroid(), Constants.DbFilePath);