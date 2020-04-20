using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientPageMobile : ContentPage
    {
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
            //            var products = await SandoghcheController.GetConnection().QueryAsync<ProductCategoryPriceViewModel>(query);

            var query = "select Clients.ClientId,Clients.ClientName,Clients.MobileNumber,sum(Accounting.DebtorAmount)-sum(Accounting.CreditorAmount) as 'Amount' from Accounting LEFT join Clients on Accounting.ClientId = Clients.ClientId WHERE Clients.IsDeleted == false GROUP by Accounting.ClientId";

            var clients = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);

            var result = new List<ClientCreditViewModel>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = clients;
            else
                result = clients.Where(c => c.ClientName.Contains(Searchtext)).ToList();


            ClientslistView.ItemsSource = result;
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

        private void btnDeleteClient_Clicked(object sender, EventArgs e)
        {

        }

        private void btnEditClient_Clicked(object sender, EventArgs e)
        {

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

            private void btnClientUpdate_Clicked(object sender, EventArgs e)
        {

        }

        private void btnClientCancel_Clicked(object sender, EventArgs e)
        {

        }
    }
}