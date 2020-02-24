using Sandoghche.Components;
using Sandoghche.Models;
using SQLite;
using System;
using System.Collections.Generic;
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
            backgroundImage.Source = ImageSource.FromResource("Sandoghche.Images.background.jpg");
            sloganLine.Source = ImageSource.FromResource("Sandoghche.Images.sloganLine.png");
            Location.Source = ImageSource.FromResource("Sandoghche.Images.Location.png");
            mainLogo.Source = ImageSource.FromResource("Sandoghche.Images.mainLogo.png");
            SandoghcheController._connection = DependencyService.Get<ISQLiteDb>().GetConnection();

        }

        protected override async void OnAppearing()
        {
            await SandoghcheController._connection.CreateTableAsync<User>();
            await SandoghcheController._connection.CreateTableAsync<Roll>();
            await SandoghcheController._connection.CreateTableAsync<UserRoll>();



            //var users = await _connection.Table<User>().ToListAsync();
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

                    userRoll.RollId = 1;
                    userRoll.UserId = 1;

                    await SandoghcheController._connection.InsertAsync(user);
                    await SandoghcheController._connection.InsertAsync(userRoll);
                    Application.Current.Properties["Email"] = txtEmail.Text.ToLower();
                    Application.Current.Properties["Roll"] = "مدیر ارشد";

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
                    Application.Current.Properties["Email"] = txtEmail.Text.ToLower();

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
    }
}