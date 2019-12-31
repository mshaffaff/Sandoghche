using Sandoghche.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sandoghche.Components
{
    public class SandoghcheController
    {
        public static SQLiteAsyncConnection _connection;


        public static async Task<bool> IsValidUser(string Email)
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            var users = await _connection.Table<User>().ToArrayAsync();
            foreach (var item in users)
                if (item.Email == Email && item.IsActive == true && item.IsDeleted == false)
                    return true;

            return false;

        }

        public static SQLiteAsyncConnection GetConnection()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            return _connection;

        }

    }
}
