using Sandoghche.Components;
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
    public partial class EditPageMobile : ContentPage
    {
        public EditPageMobile()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {

            await getOrders(DateTime.Now.Date, 0);
            base.OnAppearing();

        }

        async Task getOrders(DateTime? createdDate, int orderId)
        {
            //var query = "SELECT Orders.OrderId,Orders.ReceiptNumber,Orders.FinalPayment,date(Orders.DateCreated),Orders.isDeleted,Orders.isEditedClients.ClientName from Orders LEFT  JOIN Clients ON Orders.ClientId = Clients.ClientId where Orders.isDeleted <>1";
            var query = "SELECT Orders.OrderId,Orders.ReceiptNumber,Orders.FinalPayment,date(Orders.DateCreated),Orders.isDeleted,Orders.isEdited,Clients.ClientName from Orders LEFT  JOIN Clients ON Orders.ClientId = Clients.ClientId where 1 ";

            if (createdDate != null)
                query += string.Concat("and (date(Orders.DateCreated) = date('", createdDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), "'))");

            //if (createdDate == null)
            //    query += query.Substring(0, query.Length - 5);
            ////str = str.Substring(0, str.Length - 2);

            if (orderId != 0)
                query += "  and Orders.OrderId=" + orderId;



            //if (receiptId != 0)
            //    query += "  and Orders.ReceiptNumber=" + receiptId;


            //if (!(userRoll == "مدیر ارشد" || userRoll == "مدیر"))
            //    query += (" and Orders.isDeleted = " + 0);

            var Orders = await SandoghcheController.GetConnection().QueryAsync<OrderDetailForSearchViewModel>(query);
            
            foreach (var order in Orders)
            {
                order.IsEditedDeleted = order.isDeleted + "#" + order.isEdited;
            }

            OrderslistView.ItemsSource = Orders;
        }
        async private void tabViewOrdersList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                var selectedTab = tabView.SelectedItem as TabViewItem;
                if (selectedTab.HeaderText == "TabClientsList")
                {
                    //await GetClients();

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

        private void btnSearch_Clicked(object sender, EventArgs e)
        {

        }

        private void SortByPrice_Tapped(object sender, EventArgs e)
        {

        }

        private void SortByClient_Tapped(object sender, EventArgs e)
        {

        }

        private void SortByOrderId_Tapped(object sender, EventArgs e)
        {

        }

        private void btnShareOrder_Clicked(object sender, EventArgs e)
        {

        }

        private void btnDeleteOrder_Clicked(object sender, EventArgs e)
        {

        }

        private void btnEditOrder_Clicked(object sender, EventArgs e)
        {

        }
    }
}