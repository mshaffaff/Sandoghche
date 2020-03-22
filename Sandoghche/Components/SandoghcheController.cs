using Sandoghche.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Globalization;

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

        public static string GetPersianDate(DateTime? OriginDate)
        {

            PersianCalendar pc = new PersianCalendar();

            DateTime thisDate = OriginDate != null ? OriginDate.Value : DateTime.Now;



            string persianYear = "";
            persianYear = pc.GetYear(thisDate).ToString();


            string persianDayOfWeek = "";
            persianDayOfWeek = pc.GetDayOfWeek(thisDate).ToString();
            switch (pc.GetDayOfWeek(thisDate))
            {
                case System.DayOfWeek.Monday:
                    persianDayOfWeek = "دوشنبه";
                    break;
                case System.DayOfWeek.Tuesday:
                    persianDayOfWeek = "سه شنبه";
                    break;
                case System.DayOfWeek.Wednesday:
                    persianDayOfWeek = "چهار شنبه";
                    break;
                case System.DayOfWeek.Thursday:
                    persianDayOfWeek = "پنج شنبه";
                    break;
                case System.DayOfWeek.Friday:
                    persianDayOfWeek = "جمعه";
                    break;
                case System.DayOfWeek.Saturday:
                    persianDayOfWeek = "شنبه";
                    break;
                case System.DayOfWeek.Sunday:
                    persianDayOfWeek = "یک شنبه";
                    break;
            }

            string persinaDayOfMonth = "";
            persinaDayOfMonth = pc.GetDayOfMonth(thisDate).ToString();


            string persianMonth = "";
            switch (pc.GetMonth(thisDate))
            {
                case 1:
                    persianMonth = "فروردین";
                    break;
                case 2:
                    persianMonth = "اردیبهشت";
                    break;
                case 3:
                    persianMonth = "خرداد";
                    break;
                case 4:
                    persianMonth = "تیر";
                    break;
                case 5:
                    persianMonth = "مرداد";
                    break;
                case 6:
                    persianMonth = "شهریور";
                    break;
                case 7:
                    persianMonth = "مهر";
                    break;
                case 8:
                    persianMonth = "آبان";
                    break;
                case 9:
                    persianMonth = "آذر";
                    break;
                case 10:
                    persianMonth = "دی";
                    break;
                case 11:
                    persianMonth = "بهمن";
                    break;
                case 12:
                    persianMonth = "اسفند";
                    break;
            }




            return (persianDayOfWeek + " " + persinaDayOfMonth + " " + persianMonth + " " + persianYear);
        }

        async public static Task<string> GetQuote()
        {
            var setting = await SandoghcheController.GetConnection().Table<SandoghcheSetting>().FirstOrDefaultAsync();
            if (setting == null)
                return "به صفحه تنظیمات مراجعه کنید";
            else
                return setting.QuoteText;
        }

    }
}
