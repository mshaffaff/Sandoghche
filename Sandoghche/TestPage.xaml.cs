using Sandoghche.Components;
using Sandoghche.Models;
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
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
        }
        async protected override void OnAppearing()
        {
            //await SandoghcheController.GetConnection().CreateTableAsync<Client>();
            await GetClients();
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

        private void ClientDataGrid_SelectionChanged(object sender, Telerik.XamarinForms.DataGrid.DataGridSelectionChangedEventArgs e)
        {

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}