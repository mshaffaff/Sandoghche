using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using Sandoghche.Models;
using Sandoghche.Components;
using SQLiteNetExtensionsAsync.Extensions;
namespace Sandoghche
{


    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {

        public SettingsPage()
        {
            InitializeComponent();

        }

        async protected override void OnAppearing()
        {
            await GetUsers();
            base.OnAppearing();
        }

        async Task GetUsers(string Searchtext = null)
        {
            var users = await SandoghcheController.GetConnection().Table<User>().Where(u => u.IsDeleted != true).ToListAsync();

            var result = new List<User>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = users;
            else
                result = users.Where(c => c.Email.Contains(Searchtext) && c.IsDeleted != true).ToList();
            UserDataGrid.ItemsSource = result;
        }

        async private void btnUserUpdate_Clicked(object sender, EventArgs e)
        {
            var user = (User)UserDataGrid.SelectedItems[0];
            var Rolls = await SandoghcheController.GetConnection().Table<Roll>().ToListAsync();
            UserRoll userRoll = new UserRoll();

            if (String.IsNullOrWhiteSpace(txtUserFullName.Text))
                await DisplayAlert("خطا", "نام مشترک نمیتواند خالی باشد", "باشه");
            else
            {
                user.FullName = txtUserFullName.Text;
                user.Mobile = txtUserMobile.Text;
                user.Email = txtUserEmail.Text;
                user.IsActive = swtUserIsActive.IsToggled;

                if ((chboxAdmin.IsChecked && chboxCashier.IsChecked) || (chboxAdmin.IsChecked && chboxHost.IsChecked) || (chboxCashier.IsChecked && chboxHost.IsChecked))
                    await DisplayAlert("خطا", "تداخل سطوح دسترسی", "باشه");
                else if (chboxAdmin.IsChecked)
                {
                    string query = "delete from UserRolls where UserId=" + user.UserId;
                    await SandoghcheController._connection.QueryAsync<UserRoll>(query);

                    userRoll.RollId = 2;
                    userRoll.UserId = user.UserId;
                    await SandoghcheController._connection.InsertAsync(userRoll);
                }
                else if (chboxCashier.IsChecked)
                {
                    string query = "delete from UserRolls where UserId=" + user.UserId;
                    await SandoghcheController._connection.QueryAsync<UserRoll>(query);
                    userRoll.RollId = 3;
                    userRoll.UserId = user.UserId;
                    await SandoghcheController._connection.InsertAsync(userRoll);
                }
                else if ((chboxHost.IsChecked))
                {
                    string query = "delete from UserRolls where UserId=" + user.UserId;
                    await SandoghcheController._connection.QueryAsync<UserRoll>(query);
                    userRoll.RollId = 4;
                    userRoll.UserId = user.UserId;
                    await SandoghcheController._connection.InsertAsync(userRoll);
                }
                else
                {
                    string query = "delete from UserRolls where UserId=" + user.UserId;
                    await SandoghcheController._connection.QueryAsync<UserRoll>(query);
                }


                await SandoghcheController._connection.UpdateAsync(user);

                txtUserFullName.Text = "";
                txtUserMobile.Text = "";
                txtUserEmail.Text = "";
                swtUserIsActive.IsToggled = true;
                btnUserCancel.IsVisible = false;
                btnUserDelete.IsVisible = false;
                btnUserUpdate.IsVisible = false;
                chboxSuperUser.IsChecked = false;
                chboxAdmin.IsChecked = false;
                chboxCashier.IsChecked = false;
                chboxHost.IsChecked = false;

                UserDataGrid.ItemsSource = null;

                await GetUsers();

            }
        }


        private void btnUserDelete_Clicked(object sender, EventArgs e)
        {

        }

        private void btnUserCancel_Clicked(object sender, EventArgs e)
        {

        }

        async private void srchUsers_TextChanged(object sender, TextChangedEventArgs e)
        {
            await GetUsers(e.NewTextValue);
        }

        async private void UserDataGrid_SelectionChanged(object sender, Telerik.XamarinForms.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            if (UserDataGrid.SelectedItems != null && UserDataGrid.SelectedItems.Count > 0 && UserDataGrid.SelectedItems[0] != null)
            {
                var user = (User)UserDataGrid.SelectedItems[0];
                txtUserFullName.Text = user.FullName;
                txtUserMobile.Text = user.Mobile;
                txtUserEmail.Text = user.Email;
                swtUserIsActive.IsToggled = user.IsActive;
                chboxAdmin.IsChecked = false;
                chboxSuperUser.IsChecked = false;
                chboxCashier.IsChecked = false;
                chboxHost.IsChecked = false;

                var Rolls = await SandoghcheController.GetConnection().Table<UserRoll>().Where(ur => ur.UserId == user.UserId).ToListAsync();

                if (Rolls.Count == 0)
                {
                    chboxSuperUser.IsChecked = false;
                    chboxSuperUser.IsEnabled = false;
                    chboxSuperUser.IsVisible = false;
                    lblSuperUser.IsVisible = false;
                    chboxAdmin.IsEnabled = true;
                    chboxCashier.IsEnabled = true;
                    chboxHost.IsEnabled = true;
                    swtUserIsActive.IsEnabled = true;
                    btnUserCancel.IsVisible = true;
                    btnUserDelete.IsVisible = true;
                    btnUserUpdate.IsVisible = true;
                    txtUserEmail.IsEnabled = true;
                    txtUserFullName.IsEnabled = true;
                    txtUserMobile.IsEnabled = true;
                    return;
                }
                else
                {
                    foreach (var Roll in Rolls)
                    {

                        switch (Roll.RollId)
                        {
                            case 1:
                                {
                                    chboxSuperUser.IsChecked = true;
                                    chboxSuperUser.IsEnabled = false;
                                    lblSuperUser.IsVisible = true;
                                    chboxSuperUser.IsVisible = true;
                                    chboxAdmin.IsEnabled = false;
                                    chboxCashier.IsEnabled = false;
                                    chboxHost.IsEnabled = false;
                                    swtUserIsActive.IsEnabled = false;
                                    btnUserCancel.IsVisible = false;
                                    btnUserDelete.IsVisible = false;
                                    btnUserUpdate.IsVisible = false;
                                    txtUserEmail.IsEnabled = false;
                                    txtUserFullName.IsEnabled = false;
                                    txtUserMobile.IsEnabled = false;
                                }
                                break;
                            case 2:
                                {
                                    chboxSuperUser.IsChecked = false;
                                    chboxSuperUser.IsEnabled = false;
                                    chboxSuperUser.IsVisible = false;
                                    lblSuperUser.IsVisible = false;

                                    chboxAdmin.IsChecked = true;
                                    chboxAdmin.IsEnabled = true;
                                    chboxCashier.IsEnabled = true;
                                    chboxHost.IsEnabled = true;
                                    swtUserIsActive.IsEnabled = true;
                                    btnUserCancel.IsVisible = true;
                                    btnUserDelete.IsVisible = true;
                                    btnUserUpdate.IsVisible = true;
                                    txtUserEmail.IsEnabled = true;
                                    txtUserFullName.IsEnabled = true;
                                    txtUserMobile.IsEnabled = true;
                                }

                                break;
                            case 3:
                                chboxSuperUser.IsChecked = false;
                                chboxSuperUser.IsEnabled = false;
                                chboxSuperUser.IsVisible = false;
                                lblSuperUser.IsVisible = false;

                                chboxCashier.IsChecked = true;
                                chboxAdmin.IsEnabled = true;
                                chboxCashier.IsEnabled = true;
                                chboxHost.IsEnabled = true;
                                swtUserIsActive.IsEnabled = true;
                                btnUserCancel.IsVisible = true;
                                btnUserDelete.IsVisible = true;
                                btnUserUpdate.IsVisible = true;
                                txtUserEmail.IsEnabled = true;
                                txtUserFullName.IsEnabled = true;
                                txtUserMobile.IsEnabled = true;
                                break;
                            case 4:
                                chboxSuperUser.IsChecked = false;
                                chboxSuperUser.IsEnabled = false;
                                chboxSuperUser.IsVisible = false;
                                lblSuperUser.IsVisible = false;

                                chboxHost.IsChecked = true;
                                chboxAdmin.IsEnabled = true;
                                chboxCashier.IsEnabled = true;
                                chboxHost.IsEnabled = true;
                                swtUserIsActive.IsEnabled = true;
                                btnUserCancel.IsVisible = true;
                                btnUserDelete.IsVisible = true;
                                btnUserUpdate.IsVisible = true;
                                txtUserEmail.IsEnabled = true;
                                txtUserFullName.IsEnabled = true;
                                txtUserMobile.IsEnabled = true;
                                break;
                            default:
                                break;
                        }
                    }

                }
            }
        }
    }
}