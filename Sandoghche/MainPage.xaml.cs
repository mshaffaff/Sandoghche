using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Sandoghche.Components;
using SQLite;
using Sandoghche.Models;
using System.Security.Cryptography;
using System.Globalization;

namespace Sandoghche
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        public MainPage()
        {
            InitializeComponent();

            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;
           
            lblPersianYear.Text = pc.GetYear(thisDate).ToString();
            lblPersianDay.Text = pc.GetDayOfMonth(thisDate).ToString();

            switch (pc.GetMonth(thisDate))
            {
                case 1:
                    lblPersianMonth.Text = "فروردین";
                    break;
                case 2:
                    lblPersianMonth.Text = "اردیبهشت";
                    break;
                case 3:
                    lblPersianMonth.Text = "خرداد";
                    break;
                case 4:
                    lblPersianMonth.Text = "تیر";
                    break;
                case 5:
                    lblPersianMonth.Text = "مرداد";
                    break;
                case 6:
                    lblPersianMonth.Text = "شهریور";
                    break;
                case 7:
                    lblPersianMonth.Text = "مهر";
                    break;
                case 8:
                    lblPersianMonth.Text = "آبان";
                    break;
                case 9:
                    lblPersianMonth.Text = "آذر";
                    break;
                case 10:
                    lblPersianMonth.Text = "دی";
                    break;
                case 11:
                    lblPersianMonth.Text = "بهمن";
                    break;
                case 12:
                    lblPersianMonth.Text = "اسفند";
                    break;
            }



            NavigationPage.SetHasBackButton(this, false);
            backgroundImage.Source = ImageSource.FromResource("Sandoghche.Images.background.jpg");
            sloganLine.Source = ImageSource.FromResource("Sandoghche.Images.sloganLine.png");
            Location.Source = ImageSource.FromResource("Sandoghche.Images.Location.png");
            mainLogo.Source = ImageSource.FromResource("Sandoghche.Images.mainLogo.png");
        }

        async private void btnForgetPassword_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgetPasswordPage());
        }

        async private void btnRegister_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        async private void btnLogin_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                await DisplayAlert("خطا", "نام کاربری یا رمز عبور خالی است", "باشه");
            }

            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            var user = await _connection.Table<User>().FirstOrDefaultAsync(x => x.Email.ToLower() == txtEmail.Text.ToLower() && x.IsActive && !x.IsDeleted);

            if (user != null)
            {
                byte[] hashBytes = Convert.FromBase64String(user.PasswordHash);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                var pbkdf2 = new Rfc2898DeriveBytes(txtPassword.Text, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                    {
                        await DisplayAlert("خطا", "رمز اشتباه است", "باشه");
                        return;
                    }

                Application.Current.Properties["Email"] = txtEmail.Text;
                await Navigation.PushAsync(new SandoghcheMainPage());
            }
            else
            {
                await DisplayAlert("خطا", "نام کاربری معتبر نیست", "باشه");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
