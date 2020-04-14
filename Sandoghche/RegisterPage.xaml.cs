using Sandoghche.Components;
using Sandoghche.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {

        //private SQLiteAsyncConnection _connection;
        public RegisterPage()
        {


            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, false);

            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;

            backgroundImage.Source = ImageSource.FromResource("Sandoghche.Images.background.jpg");
            sloganLine.Source = ImageSource.FromResource("Sandoghche.Images.sloganLine.png");
            Location.Source = ImageSource.FromResource("Sandoghche.Images.Location.png");
            mainLogo.Source = ImageSource.FromResource("Sandoghche.Images.mainLogo.png");
            SandoghcheLogo.Source = ImageSource.FromResource("Sandoghche.Images.SandoghcheLogo.png");

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



            SandoghcheController._connection = DependencyService.Get<ISQLiteDb>().GetConnection();


        }

        async protected override void OnAppearing()
        {
            lblQuote.Text = await SandoghcheController.GetQuote();

            base.OnAppearing();
        }
        private async void btnRegister_Clicked(object sender, EventArgs e)
        {
            User user = new User();
            Roll roll = new Roll();
            UserRoll userRoll = new UserRoll();
           
            var users = await SandoghcheController._connection.Table<User>().ToListAsync();


            bool IsValidEmail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                (string.IsNullOrWhiteSpace(txtEmail.Text)) ||
                (string.IsNullOrWhiteSpace(txtMobileNumber.Text)) ||
                (string.IsNullOrWhiteSpace(txtPassword.Text)) ||
                (string.IsNullOrWhiteSpace(txtRepeatPassword.Text)))
            {
                await DisplayAlert("ورود اطلاعات", "اطلاعات ناقص است", "باشه");
            }
            else if (!string.Equals(txtPassword.Text, txtRepeatPassword.Text))
            {
                await DisplayAlert("ورود اطلاعات", "رمز عبور یکسان نیست", "باشه");
                txtPassword.Text = string.Empty;
                txtRepeatPassword.Text = string.Empty;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                await DisplayAlert("ورود اطلاعات", "ایمل نامعتبر", "باشه");
            }
            else
            {
                var hash = new PasswordHash(txtPassword.Text.ToLower());
                user.FullName = txtFullName.Text;
                user.Email = txtEmail.Text.ToLower();
                user.Mobile = txtMobileNumber.Text;
                user.PasswordHash = Convert.ToBase64String(hash.ToArray());
                user.IsActive = true;

                var CheckIfUserExist = await SandoghcheController._connection.Table<User>().FirstOrDefaultAsync(x => x.Email.ToLower() == txtEmail.Text.ToLower() || x.Mobile == txtMobileNumber.Text);

                var CheckIfUserTableIsEmpty = await SandoghcheController._connection.Table<User>().FirstOrDefaultAsync();

                if (CheckIfUserTableIsEmpty == null)
                {
                    

                    roll.RollName = "مدیر ارشد";
                    await SandoghcheController._connection.InsertAsync(roll);
                    roll.RollName = "مدیر";
                    await SandoghcheController._connection.InsertAsync(roll);
                    roll.RollName = "صندوقدار";
                    await SandoghcheController._connection.InsertAsync(roll);
                    roll.RollName = "میزبان";
                    await SandoghcheController._connection.InsertAsync(roll);
                    roll.RollName = "بازاریاب";
                    await SandoghcheController._connection.InsertAsync(roll);
                   
                    userRoll.RollId = 1;
                    userRoll.UserId = 1;

                    await SandoghcheController._connection.InsertAsync(user);
                    await SandoghcheController._connection.InsertAsync(userRoll);
                    
                    //Application.Current.Properties["Email"] = txtEmail.Text.ToLower();

                    Application.Current.Properties["FullName"] = user.FullName;

                    Application.Current.Properties["userRollName"] = "مدیر ارشد";


                    txtFullName.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtMobileNumber.Text = string.Empty;
                    txtRepeatPassword.Text = string.Empty;
                    txtPassword.Text = string.Empty;



                    await Application.Current.SavePropertiesAsync();

                    await Navigation.PushAsync(new SandoghcheMainPage());

                }else
                {
                    if (CheckIfUserExist != null)
                    {
                        await DisplayAlert("خطا", "این شماره موبایل یا ایمیل  قبلا ثبت شده است ", "باشه");
                        txtEmail.Text = string.Empty;
                        txtMobileNumber.Text = string.Empty;
                        return;
                    }

                    await SandoghcheController._connection.InsertAsync(user);
                    
                    //Application.Current.Properties["Email"] = txtEmail.Text.ToLower();

                    //Application.Current.Properties["FullName"] = user.FullName;

                    //Application.Current.Properties["userRollName"] = roll.RollName;


                    txtFullName.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtMobileNumber.Text = string.Empty;
                    txtRepeatPassword.Text = string.Empty;
                    txtPassword.Text = string.Empty;



                    await Application.Current.SavePropertiesAsync();

                    await Navigation.PushAsync(new MainPage());
                }


                


            }



        }
        //protected override bool OnBackButtonPressed()
        //{
        //    return true;
        //}
    }
}