using Sandoghche.Components;
using Sandoghche.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgetPasswordPage : ContentPage
    {
        public ForgetPasswordPage()
        {
            InitializeComponent();
            //NavigationPage.SetHasBackButton(this, false);
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;

            backgroundImage.Source = ImageSource.FromResource("Sandoghche.Images.background.jpg");
            sloganLine.Source = ImageSource.FromResource("Sandoghche.Images.sloganLine.png");
            Location.Source = ImageSource.FromResource("Sandoghche.Images.Location.png");
            mainLogo.Source = ImageSource.FromResource("Sandoghche.Images.mainLogo.png");
            
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
            lblQuote.Text = await SandoghcheController.GetQuote();

            base.OnAppearing();
        }
        async private void btnPasswordSend_Clicked(object sender, EventArgs e)
        {
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

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                await DisplayAlert("ورود اطلاعات", "اطلاعات ناقص است", "باشه");
                return;
            }

            SandoghcheController._connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            var user = await SandoghcheController._connection.Table<User>().FirstOrDefaultAsync(x => x.Email.ToLower() == txtEmail.Text.ToLower());


            //if (user != null)
            //{
            //    await DisplayAlert("خطا", "این ایمیل قبلا ثبت شده است ", "باشه");
            //    txtEmail.Text = string.Empty;
            //    return;
            //}

            if (!IsValidEmail(txtEmail.Text))
            {
                await DisplayAlert("ورود اطلاعات", "ایمل نامعتبر", "باشه");
            }
            else if (user == null)
            {
                await DisplayAlert("خطا", "این ایمیل  ثبت نشده است ", "باشه");
                txtEmail.Text = string.Empty;
                return;
            }
            else 
            { 
                try
                {
                    var newPassword = DateTime.Now.Ticks.ToString();
                    var hash = new PasswordHash(newPassword);
                    user.PasswordHash = Convert.ToBase64String(hash.ToArray());
                   
                    await SandoghcheController._connection.UpdateAsync(user);

                    
                   
                    MailMessage mail = new MailMessage();
                    SmtpClient smtpServer = new SmtpClient("smtp-mail.outlook.com");

                    mail.From = new MailAddress("mshaffaf@outlook.com");
                    mail.To.Add(txtEmail.Text);
                    mail.Subject = "بازیابی رمز عبور";
                    mail.Body = "رمز عبور شما به این شرح است : " + newPassword;

                    smtpServer.Port = 587;
                    smtpServer.Credentials = new System.Net.NetworkCredential("mshaffaf@outlook.com", "@Fastrack");
                    smtpServer.EnableSsl = true;
                    

                    smtpServer.Send(mail);

                }
                catch (Exception ex)
                {
                    await DisplayAlert("خطا", ex.ToString(), "باشه");
                    return;
                }
            }



            await Navigation.PopAsync();
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    return true;
        //}
    }
}