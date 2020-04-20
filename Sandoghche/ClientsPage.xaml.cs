using Sandoghche.Components;
using Sandoghche.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientsPage : ContentPage
    {


        public ClientsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            lblPersianDate.Text = SandoghcheController.GetPersianDate(null);

            lblUser.Text = Application.Current.Properties["userRollName"].ToString() + " : " + Application.Current.Properties["FullName"].ToString();

        }

        async protected override void OnAppearing()
        {
            //await SandoghcheController.GetConnection().CreateTableAsync<Client>();


            await GetClients();
            btnPayCredit.IsEnabled = false;
            base.OnAppearing();
        }

        async Task GetClients(string Searchtext = null)
        {
            var clients = await SandoghcheController.GetConnection().Table<Client>().Where(c => c.IsDeleted != true).ToListAsync();

            var result = new List<Client>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = clients;
            else
                result = clients.Where(c => c.ClientName.Contains(Searchtext) && c.IsDeleted != true).ToList();


            //var result = new List<Client>() { new Client { ClientId=1,ClientName="Test1",Address="hgflashdfgasjdhgfalskjhdfgsajhdf",Email="1@3.com",MobileNumber="09173842445",PhoneNumber="712331615" }, new Client { ClientId = 2, ClientName = "Test2", Address = "hgflashdfgasjdhgfalskjhdfgsajhdf", Email = "1@3.com", MobileNumber = "09173842445", PhoneNumber = "712331615" }, new Client { ClientId = 3, ClientName = "Test3", Address = "hgflashdfgasjdhgfalskjhdfgsajhdf", Email = "1@3.com", MobileNumber = "09173842445", PhoneNumber = "712331615" } };

            ClientDataGrid.ItemsSource = result;
        }

        private async void srchClients_TextChanged(object sender, TextChangedEventArgs e)
        {
            await GetClients(e.NewTextValue);
        }

        async private void btnClientRegister_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtClientFullName.Text))
                await DisplayAlert("خطا", "نام مشترک نمیتواند خالی باشد", "باشه");
            else
            {
                var client = new Client();
                client.ClientName = txtClientFullName.Text;
                client.PhoneNumber = txtClientPhoneNumber.Text;
                client.MobileNumber = txtClientMobileNumber.Text;
                client.Email = txtClientEmail.Text;
                client.Address = txtClientAddress.Text;
                client.IsActive = true;

                await SandoghcheController._connection.InsertAsync(client);
                await DisplayAlert("ثبت مشخصات", "مشترک جدید در سیستم با موفقیت ثبت گردید", "باشه");
                String.IsNullOrEmpty(txtClientFullName.Text);
                String.IsNullOrEmpty(txtClientPhoneNumber.Text);
                String.IsNullOrEmpty(txtClientMobileNumber.Text);
                String.IsNullOrEmpty(txtClientAddress.Text);
                String.IsNullOrEmpty(txtClientEmail.Text);
                txtClientFullName.Text = "";
                txtClientPhoneNumber.Text = "";
                txtClientMobileNumber.Text = "";
                txtClientEmail.Text = "";
                txtClientAddress.Text = "";

                await GetClients();


            }
        }

        private void btnClientCancel_Clicked(object sender, EventArgs e)
        {
            txtClientFullName.Text = "";
            txtClientPhoneNumber.Text = "";
            txtClientMobileNumber.Text = "";
            txtClientEmail.Text = "";
            txtClientAddress.Text = "";
            btnClientCancel.IsVisible = false;
            btnClientDelete.IsVisible = false;
            btnClientUpdate.IsVisible = false;
            btnClientRegister.IsVisible = true;
            txtDebtAmount.Text = "";
            btnPayCredit.IsEnabled = false;

        }

        async private void btnClientUpdate_Clicked(object sender, EventArgs e)
        {
            var client = (Client)ClientDataGrid.SelectedItems[0];

            if (String.IsNullOrWhiteSpace(txtClientFullName.Text))
                await DisplayAlert("خطا", "نام مشترک نمیتواند خالی باشد", "باشه");
            else
            {
                client.ClientName = txtClientFullName.Text;
                client.PhoneNumber = txtClientPhoneNumber.Text;
                client.MobileNumber = txtClientMobileNumber.Text;
                client.Email = txtClientEmail.Text;
                client.Address = txtClientAddress.Text;
                client.IsActive = swtClientIsActive.IsToggled;

                await SandoghcheController._connection.UpdateAsync(client);
                await DisplayAlert("ثبت", "بروز رسانی با موفقیت انجام شد", "باشه");

                txtClientFullName.Text = "";
                txtClientPhoneNumber.Text = "";
                txtClientMobileNumber.Text = "";
                txtClientEmail.Text = "";
                txtClientAddress.Text = "";
                btnClientCancel.IsVisible = false;
                btnClientDelete.IsVisible = false;
                btnClientUpdate.IsVisible = false;
                btnClientRegister.IsVisible = true;

                ClientDataGrid.ItemsSource = null;

                await GetClients();
            }




        }

        async private void btnClientDelete_Clicked(object sender, EventArgs e)
        {
            var client = (Client)ClientDataGrid.SelectedItems[0];

            var action = await DisplayAlert("اخطار", "آیا میخواید این مشترک حذف شود", "بله", "خیر");

            if (action)
            {

               // var query = "select (sum(DebtorAmount)-sum(CreditorAmount)) as 'Amount' from Accounting WHERE ClientId=" + Convert.ToInt32(client.ClientId);
                var query = "select Count(OrderId) as 'Amount' from Orders WHERE ClientId=" + Convert.ToInt32(client.ClientId);
                var amount = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);
               
                if (amount[0].Amount > 0)
                {
                    await DisplayAlert("اخطار", "امکان حذف به دلیل سابقه خرید در سیستم وجود ندارد ", "باشه");
                    return;
                }

                    client.IsDeleted = true;
                await SandoghcheController._connection.UpdateAsync(client);

                txtClientFullName.Text = "";
                txtClientPhoneNumber.Text = "";
                txtClientMobileNumber.Text = "";
                txtClientEmail.Text = "";
                txtClientAddress.Text = "";
                btnClientCancel.IsVisible = false;
                btnClientDelete.IsVisible = false;
                btnClientUpdate.IsVisible = false;
                btnClientRegister.IsVisible = true;

                await GetClients();
            }




        }

        async private void ClientDataGrid_SelectionChanged(object sender, Telerik.XamarinForms.DataGrid.DataGridSelectionChangedEventArgs e)
        {


            if (ClientDataGrid.SelectedItems != null && ClientDataGrid.SelectedItems.Count > 0 && ClientDataGrid.SelectedItems[0] != null)
            {
                var client = (Client)ClientDataGrid.SelectedItems[0];
                txtClientFullName.Text = client.ClientName;
                txtClientPhoneNumber.Text = client.PhoneNumber;
                txtClientMobileNumber.Text = client.MobileNumber;
                txtClientEmail.Text = client.Email;
                txtClientAddress.Text = client.Address;
                btnClientRegister.IsVisible = false;
                btnClientCancel.IsVisible = true;
                btnClientUpdate.IsVisible = true;
                btnClientDelete.IsVisible = true;
                txtDebtAmount.Text = "0";
                txtCreditAmount.Value = null;
                await ClientCreditStatus(client.ClientId.ToString());

            }
        }
        
        public class ClientCreditViewModel
        {
            public double Amount { get; set; }
        }
        async private Task ClientCreditStatus(string ClientId)
        {
            var query = "select (sum(DebtorAmount)-sum(CreditorAmount)) as 'Amount' from Accounting WHERE ClientId=" + Convert.ToInt32(ClientId);
            var amount = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);
            if (amount[0].Amount > 0)
            {
                txtDebtAmount.Text = amount.FirstOrDefault()?.Amount.ToString() ?? 0.ToString(); ;
                btnPayCredit.IsEnabled = true;
            }
            else
                btnPayCredit.IsEnabled = false;

        }

        async private void imgSelectClient_Tapped(object sender, EventArgs e)
        {
            var s = sender as Image;
            var selectedItem = s.BindingContext;
            var client = (Client)selectedItem;
            await Navigation.PushAsync(new InvoicePage(client.ClientId, client.ClientName));
        }

        async private void btnPayCredit_Clicked(object sender, EventArgs e)
        {
            var client = (Client)ClientDataGrid.SelectedItems[0];

            Accounting accounting = new Accounting();
            accounting.ClientId = client.ClientId;

            accounting.CreditorAmount = Convert.ToDouble(txtCreditAmount.Value);
            accounting.DebtorAmount = 0;


            if (Convert.ToDouble(txtCreditAmount.Value) > Convert.ToDouble(txtDebtAmount.Text))
            {
                await DisplayAlert("خطا", "مبلغ پرداختی از مبلغ بدهی بیشتر است", "باشه");
                return;
            }                
            await SandoghcheController._connection.InsertAsync(accounting);
            txtCreditAmount.Value = null;
            await ClientCreditStatus(client.ClientId.ToString());
        }

        private void btnHome_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }


        protected override bool OnBackButtonPressed()
        {
            return true;
        }



    }
}