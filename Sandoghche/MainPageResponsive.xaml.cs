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
using Xamarin.Essentials;

namespace Sandoghche
{
    [DesignTimeVisible(false)]
    public partial class MainPageResponsive : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        public MainPageResponsive()
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
            SandoghcheLogo.Source = ImageSource.FromResource("Sandoghche.Images.SandoghcheLogo.png");

            if (Device.Idiom == TargetIdiom.Phone)
            {
                RightPanel.IsVisible = false;
                MainPanel.Children.Add(LeftPanel);
                Grid.SetColumn(LeftPanel, 0);

            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                MainPanel.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(2, GridUnitType.Star)
                });
                MainPanel.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                MainPanel.Children.Add(LeftPanel);
                Grid.SetColumn(LeftPanel, 0);
                MainPanel.Children.Add(RightPanel);
                Grid.SetColumn(RightPanel, 1);

                RightPanel.IsVisible = true;

                lblPersianYear.FontSize = 40;
                lblPersianMonth.FontSize = 30;
                lblPersianDay.FontSize = 30;
                lblQuote.FontSize = 20;

            }
            else if (Device.Idiom == TargetIdiom.Desktop)
            {
                MainPanel.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(2, GridUnitType.Star)
                });
                MainPanel.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                MainPanel.Children.Add(LeftPanel);
                Grid.SetColumn(LeftPanel, 0);
                MainPanel.Children.Add(RightPanel);
                Grid.SetColumn(RightPanel, 1);
                RightPanel.IsVisible = true;
                lblPersianYear.FontSize = 50;
                lblPersianMonth.FontSize = 35;
                lblPersianDay.FontSize = 35;

            }

        }
        async protected override void OnAppearing()
        {
            await SandoghcheController.GetConnection().CreateTableAsync<User>();
            await SandoghcheController.GetConnection().CreateTableAsync<Roll>();
            await SandoghcheController.GetConnection().CreateTableAsync<Category>();
            await SandoghcheController.GetConnection().CreateTableAsync<Client>();
            await SandoghcheController.GetConnection().CreateTableAsync<Product>();
            await SandoghcheController.GetConnection().CreateTableAsync<Order>();
            await SandoghcheController.GetConnection().CreateTableAsync<OrderDetail>();
            await SandoghcheController.GetConnection().CreateTableAsync<EditedOrdersLogs>();
            await SandoghcheController.GetConnection().CreateTableAsync<EditedOrderDetailsLogs>();
            await SandoghcheController.GetConnection().CreateTableAsync<SandoghcheSetting>();
            await SandoghcheController.GetConnection().CreateTableAsync<Accounting>();
            await SandoghcheController.GetConnection().CreateTableAsync<UserRoll>();

            lblQuote.Text = await SandoghcheController.GetQuote();
            base.OnAppearing();
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
            if (user == null)
            {
                await DisplayAlert("خطا", "نام کاربری معتبر نیست", "باشه");
                return;
            }
            var userRoll = await _connection.Table<UserRoll>().FirstOrDefaultAsync(ur => ur.UserId == user.UserId);
            if (userRoll == null)
            {
                await DisplayAlert("خطا", "نام کاربری معتبر نیست", "باشه");
                return;
            }

            var rollName = await _connection.Table<Roll>().FirstOrDefaultAsync(r => r.RollId == userRoll.RollId);
            if (user != null && userRoll != null)
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
                Application.Current.Properties["FullName"] = user.FullName;
                Application.Current.Properties["userRollName"] = rollName.RollName;

                await Application.Current.SavePropertiesAsync();


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
