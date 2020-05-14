using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ModelView;
using Sandoghche.ViewModels;
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
    public partial class OrderHistoryPage : ContentPage
    {
        private string userRoll;
        private int OrderId = -1;
        public OrderHistoryPage(int orderId)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            OrderId = orderId;
            lblPersianDate.Text = SandoghcheController.GetPersianDate(null);
            lblUser.Text = Application.Current.Properties["userRollName"].ToString() + " : " + Application.Current.Properties["FullName"].ToString();
            userRoll = Application.Current.Properties["userRollName"].ToString();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        
            await getEditedOrders(OrderId);
        }
       
        async Task getEditedOrders(int orderId)
        {
            var query = "SELECT EditedOrdersLogs.OrderId,EditedOrdersLogs.ReceiptNumber,EditedOrdersLogs.EditedLogId,EditedOrdersLogs.FinalPayment,date(EditedOrdersLogs.DateCreated),Clients.ClientName from EditedOrdersLogs LEFT  JOIN Clients ON EditedOrdersLogs.ClientId = Clients.ClientId where EditedOrdersLogs.OrderId = " + orderId;
            var OrderHistory = await SandoghcheController.GetConnection().QueryAsync<OrderHistoryViewModel>(query);
            OrdersDataGrid.ItemsSource = OrderHistory;
        }

        private void btnPrint_Clicked(object sender, EventArgs e)
        {

        }

        private void btnShow_Clicked(object sender, EventArgs e)
        {
            var s = sender as Button;
            var selectedItem = s.BindingContext;
            var EditedOrder = (OrderHistoryViewModel)selectedItem;
            Navigation.PushAsync(new EditOrderPage(EditedOrder.OrderId,true, EditedOrder.EditedLogId));
        }
       
        private void btnHome_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditPage());
        }
        
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}