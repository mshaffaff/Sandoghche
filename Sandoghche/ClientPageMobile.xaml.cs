using Sandoghche.Components;
using Sandoghche.Models;
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

        async Task GetClients(string Searchtext = null)
        {
            var clients = await SandoghcheController.GetConnection().Table<Client>().Where(c => c.IsDeleted != true).ToListAsync();

            var result = new List<Client>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = clients;
            else
                result = clients.Where(c => c.ClientName.Contains(Searchtext) && c.IsDeleted != true).ToList();


            ClientslistView.ItemsSource = result;
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
               // txtDebtAmount.Text = amount.FirstOrDefault()?.Amount.ToString() ?? 0.ToString(); ;
               // btnPayCredit.IsEnabled = true;
            }
            else
            {

            }
               // btnPayCredit.IsEnabled = false;

        }





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
    }
}