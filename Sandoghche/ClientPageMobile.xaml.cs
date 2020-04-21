using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Input.AutoComplete;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientPageMobile : ContentPage
    {
        private static int clientId;
        public ClientPageMobile()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            await GetClients();
            base.OnAppearing();
        }

        async Task GetClients(string Searchtext = null)
        {

            var query = "select Clients.ClientId,Clients.ClientName,Clients.MobileNumber,(select sum(Accounting.DebtorAmount)-sum(Accounting.CreditorAmount) from Accounting where Accounting.ClientId=Clients.ClientId) as Amount from Clients WHERE Clients.IsDeleted == false";

            var clients = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);

            var result = new List<ClientCreditViewModel>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = clients;
            else
                result = clients.Where(c => c.ClientName.Contains(Searchtext)).ToList();


            ClientslistView.ItemsSource = result;

            srchClients.ItemsSource = clients.ToList();
            srchClients.Filter = new ClientAutoCompleteViewFilter();

        }




        //async private Task ClientCreditStatus(string ClientId)
        //{
        //    var query = "select (sum(DebtorAmount)-sum(CreditorAmount)) as 'Amount' from Accounting WHERE ClientId=" + Convert.ToInt32(ClientId);
        //    var amount = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);
        //    if (amount[0].Amount > 0)
        //    {
        //        // txtDebtAmount.Text = amount.FirstOrDefault()?.Amount.ToString() ?? 0.ToString(); ;
        //        // btnPayCredit.IsEnabled = true;
        //    }
        //    else
        //    {

        //    }
        //    // btnPayCredit.IsEnabled = false;

        //}

        async private void tabViewClient_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                var selectedTab = tabView.SelectedItem as TabViewItem;
                if (selectedTab.HeaderText == "TabClientsList")
                {
                    await GetClients();

                    //ProductsDataGrid.ItemsSource = new List<string>();

                    //ProductsDataGrid.ItemsSource = order.OrderDetails;

                    //await DisplayAlert("test", e.PropertyName, "test");
                }
                if (selectedTab.HeaderText == "TabHome")
                {
                    await Navigation.PushAsync(new SandoghcheMainPage());
                }
            }
        }

        async private void srchClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            await GetClients(e.NewTextValue);
        }


        private void SortByDebt_Tapped(object sender, EventArgs e)
        {

        }

        private void SortByMobile_Tapped(object sender, EventArgs e)
        {

        }

        private void SortByName_Tapped(object sender, EventArgs e)
        {

        }

        async private void btnDeleteClient_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var clientViewModel = button.CommandParameter as ClientCreditViewModel;
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == clientViewModel.ClientId);

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

                //txtClientFullName.Text = "";
                // txtClientPhoneNumber.Text = "";
                //txtClientMobileNumber.Text = "";
                //txtClientEmail.Text = "";
                //txtClientAddress.Text = "";
                //btnClientCancel.IsVisible = false;
                // btnClientDelete.IsVisible = false;
                // btnClientUpdate.IsVisible = false;
                // btnClientRegister.IsVisible = true;

                await GetClients();
            }

        }

        async private void btnEditClient_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var clientViewModel = button.CommandParameter as ClientCreditViewModel;
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == clientViewModel.ClientId);
            clientId = client.ClientId;

            TabAddClient.IsSelected = true;
            txtClientFullName.Text = client.ClientName;
            txtClientPhoneNumber.Text = client.PhoneNumber;
            txtClientMobileNumber.Text = client.MobileNumber;
            txtClientEmail.Text = client.Email;
            txtClientAddress.Text = client.Address;
            btnClientRegister.IsVisible = false;
            btnClientCancel.IsVisible = true;
            btnClientUpdate.IsVisible = true;
            //txtDebtAmount.Text = "0";
            //txtCreditAmount.Value = null;
            //await ClientCreditStatus(client.ClientId.ToString());
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

                txtClientFullName.Text = "";
                txtClientPhoneNumber.Text = "";
                txtClientMobileNumber.Text = "";
                txtClientEmail.Text = "";
                txtClientAddress.Text = "";

                await GetClients();

            }
        }

        async private void btnClientUpdate_Clicked(object sender, EventArgs e)
        {
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == clientId);

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

                txtClientFullName.Text = "";
                txtClientPhoneNumber.Text = "";
                txtClientMobileNumber.Text = "";
                txtClientEmail.Text = "";
                txtClientAddress.Text = "";
                btnClientCancel.IsVisible = false;
                btnClientUpdate.IsVisible = false;
                btnClientRegister.IsVisible = true;
                await GetClients();
                TabClientsList.IsSelected = true;
                await DisplayAlert("ثبت", "بروز رسانی با موفقیت انجام شد", "باشه");
            }
        }

        private void btnClientCancel_Clicked(object sender, EventArgs e)
        {
            TabClientsList.IsSelected = true;

            txtClientFullName.Text = "";
            txtClientPhoneNumber.Text = "";
            txtClientMobileNumber.Text = "";
            txtClientEmail.Text = "";
            txtClientAddress.Text = "";
            btnClientCancel.IsVisible = false;
            btnClientUpdate.IsVisible = false;
            btnClientRegister.IsVisible = true;
            //txtDebtAmount.Text = "";
            //btnPayCredit.IsEnabled = false;
        }

        async private void btnPayCredit_Clicked(object sender, EventArgs e)
        {
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == clientId);

            Accounting accounting = new Accounting();
            accounting.ClientId = client.ClientId;

            accounting.CreditorAmount = Convert.ToDouble(txtCreditAmount.Value);
            accounting.DebtorAmount = 0;
            
                       //if (Convert.ToDouble(txtCreditAmount.Value) > Convert.ToDouble(txtDebtAmount.Text))
            //{
            //    await DisplayAlert("خطا", "مبلغ پرداختی از مبلغ بدهی بیشتر است", "باشه");
            //    return;
            //}
            await SandoghcheController._connection.InsertAsync(accounting);
            txtCreditAmount.Value = null;
            txtDebtAmount.Text = "0";
            srchClients.Text = null;
            lblClientCreditStatus.Text = "وضعیت حساب";
            lblClientCreditStatus.TextColor = Color.Black;

            //await ClientCreditStatus(client.ClientId);

        }

        public class ClientAutoCompleteViewFilter : IAutoCompleteFilter
        {
            public bool Filter(object item, string searchText, CompletionMode completionMode)
            {

                ClientCreditViewModel client = (ClientCreditViewModel)item;
                string clientName = client.ClientName;
                return clientName.Contains(searchText);
            }
        }



        async private Task ClientCreditStatus(int ClientId)
        {
            var query = "select (sum(DebtorAmount)-sum(CreditorAmount)) as 'Amount' from Accounting WHERE ClientId=" + ClientId;
            var amount = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);
            txtDebtAmount.Text = amount.FirstOrDefault()?.Amount.ToString() ?? 0.ToString(); ;

            if (Convert.ToDouble(txtDebtAmount.Text) > 0)
            {
                lblClientCreditStatus.Text = "وضعیت حساب / بدهکار";
                lblClientCreditStatus.TextColor = Color.Red;
            }
            else if (Convert.ToDouble(txtDebtAmount.Text) < 0)
            {
                lblClientCreditStatus.Text = "وضعیت حساب / بستانکار ";
                lblClientCreditStatus.TextColor = Color.Green;
            }
            else
            {
                lblClientCreditStatus.Text = "وضعیت حساب";
                lblClientCreditStatus.TextColor = Color.Black;
            }


            btnPayCredit.IsEnabled = true;
            txtCreditAmount.IsEnabled = true;



            //if (amount[0].Amount > 0)
            //{
            //    txtDebtAmount.Text = amount.FirstOrDefault()?.Amount.ToString() ?? 0.ToString(); ;
            //    btnPayCredit.IsEnabled = true;
            //    txtCreditAmount.IsEnabled = true;
            //}
            //else
            //    btnPayCredit.IsEnabled = false;

        }

        async private void srchClients_SuggestionItemSelected(object sender, Telerik.XamarinForms.Input.AutoComplete.SuggestionItemSelectedEventArgs e)
        {
            //order = new Order();
            var client = (ClientCreditViewModel)e.DataItem;
            clientId = client.ClientId;
            await ClientCreditStatus(clientId);
        }

        private void srchClients_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtDebtAmount.Text = "0";
            txtCreditAmount.Value = 0;
            txtCreditAmount.IsEnabled = false;
            btnPayCredit.IsEnabled = false;

        }
    }
}