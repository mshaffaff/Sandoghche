﻿using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ViewModels;
using SQLiteNetExtensionsAsync.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPage : ContentPage
    {
        private string userRoll;
        public EditPage()
        {
            InitializeComponent();
            lblPersianDate.Text = SandoghcheController.GetPersianDate(null);
            lblUser.Text = Application.Current.Properties["userRollName"].ToString() + " : " + Application.Current.Properties["FullName"].ToString();
            userRoll = Application.Current.Properties["userRollName"].ToString();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await getOrders(DateTime.Now.Date, 0, 0);
        }

        async private void btnSearch_Clicked(object sender, EventArgs e)
        {

            if (srchCreatedDate.SelectedDateTime == null && (srchReceiptNumber.Value == null || (srchReceiptNumber.Value != null && Convert.ToInt32(srchReceiptNumber.Value) == 0)) && (srchOrderId.Value == null || (srchOrderId.Value != null && Convert.ToInt32(srchOrderId.Value) == 0)))
                await DisplayAlert("اخطار", "جهت جستجو ورود یکی از فیلد ها الزامی است", "باشه");
            else
                await getOrders(srchCreatedDate.SelectedDateTime != null ? srchCreatedDate.SelectedDateTime.Value.Date : (DateTime?)null, srchOrderId.Value != null ? Convert.ToInt32(srchOrderId.Value) : 0, srchReceiptNumber.Value != null ? Convert.ToInt32(srchReceiptNumber.Value) : 0);
        }
        async Task getOrders(DateTime? createdDate, int orderId, int receiptId)
        {
            //var query = "SELECT Orders.OrderId,Orders.ReceiptNumber,Orders.FinalPayment,date(Orders.DateCreated),Orders.isDeleted,Orders.isEditedClients.ClientName from Orders LEFT  JOIN Clients ON Orders.ClientId = Clients.ClientId where Orders.isDeleted <>1";
            var query = "SELECT Orders.OrderId,Orders.ReceiptNumber,Orders.FinalPayment,date(Orders.DateCreated),Orders.isDeleted,Orders.isEdited,Clients.ClientName from Orders LEFT  JOIN Clients ON Orders.ClientId = Clients.ClientId where ";

            if (createdDate != null)
                query += string.Concat(" (date(Orders.DateCreated) = date('", createdDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), "'))");

            if (orderId != 0)
                query += " and Orders.OrderId=" + orderId;

            if (receiptId != 0)
                query += " and Orders.ReceiptNumber=" + receiptId;


            if (!(userRoll == "مدیر ارشد" || userRoll == "مدیر"))
                query += (" and Orders.isDeleted = " + 0);





            var Orders = await SandoghcheController.GetConnection().QueryAsync<OrderDetailForSearchViewModel>(query);
            foreach (var order in Orders)
            {
                order.IsEditedDeleted = order.isDeleted + "#" + order.isEdited;
            }

            OrdersDataGrid.ItemsSource = Orders;
        }

        async private void btnPrint_Clicked(object sender, EventArgs e)
        {
            var s = sender as Button;
            var selectedItem = s.BindingContext;
            var orderModel = (OrderDetailForSearchViewModel)selectedItem;
            var order = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == orderModel.OrderId);
            order.OrderDetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(od => od.OrderId == orderModel.OrderId).ToListAsync();
            var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync();

            foreach (var item in order.OrderDetails)
            {
                item.ProductText = products.FirstOrDefault(p => p.ProductId == item.ProductId).ProductText;
            }


            DependencyService.Get<IPrint>().Print(order, "چاپ مجدد");
        }



        async private void btnEdit_Clicked(object sender, EventArgs e)
        {
            var s = sender as Button;
            var selectedItem = s.BindingContext;
            var order = (OrderDetailForSearchViewModel)selectedItem;
            // await DisplayAlert("test",order.OrderId.ToString(),"test");
            await Navigation.PushAsync(new EditOrderPage(order.OrderId));



        }

        private void btnOrderHistory(object sender, EventArgs e)
        {
            var s = sender as Label;
            var selectedOrder = s.BindingContext;
            var order = (OrderDetailForSearchViewModel)selectedOrder;
            if (order.isEdited || order.isDeleted)
                Navigation.PushAsync(new OrderHistoryPage(order.OrderId));


        }
    }
}