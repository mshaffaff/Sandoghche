using Sandoghche.Components;
using Sandoghche.Models;
using System;
using System.Collections.Generic;
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
            NavigationPage.SetHasBackButton(this, false);

            backgroundImage.Source = ImageSource.FromResource("Sandoghche.Images.background.jpg");
            sloganLine.Source = ImageSource.FromResource("Sandoghche.Images.sloganLine.png");
            Location.Source = ImageSource.FromResource("Sandoghche.Images.Location.png");
            mainLogo.Source = ImageSource.FromResource("Sandoghche.Images.mainLogo.png");
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

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}