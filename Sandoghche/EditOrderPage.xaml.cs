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
    public partial class EditOrderPage : ContentPage
    {
        private int OrderId = -1;
        static Order order;
        public EditOrderPage(int orderId)
        {
            InitializeComponent();
            OrderId = orderId;
            

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            order = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == OrderId);
            
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == order.ClientId);
            var orderDetail = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(d => d.OrderId == order.OrderId).ToListAsync();
            var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync() ;
           
            
            lblOrderId.Text = order.OrderId.ToString();
            lblReceipNumber.Text = order.ReceiptNumber.ToString();
            lblPersianDate.Text = SandoghcheController.GetPersianDate(order.DateCreated);
            lblClient.Text = client.ClientName;
            lblFinalPayment.Text = order.FinalPayment.ToString();
            lblDelivery.Text = order.DeliveryFee.ToString();
            lblDiscount.Text = order.TotalDiscount.ToString();
            lblService.Text = order.TotalServiceFee.ToString();
            lblTax.Text = (order.Tax1 + order.Tax2).ToString();
            lblTotalPrice.Text = order.TotalPrice.ToString();

            int rowNumber = 1;
            foreach (var item in orderDetail)
            {
                item.ProductText =products.FirstOrDefault(p => p.ProductId == item.ProductId).ProductText;
                item.RowNumber = rowNumber;
                rowNumber++;
            }

            ProductsDataGrid.ItemsSource = orderDetail;




        }


        private void btnMines_Clicked(object sender, EventArgs e)
        {
            
        }

        private void btnPlus_Clicked(object sender, EventArgs e)
        {

        }

        private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lstProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void lstProducts_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }

        private void srchCategory_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void btnMainMenu_Tapped(object sender, EventArgs e)
        {

        }

        private void btnService_Tapped(object sender, EventArgs e)
        {

        }
        private void btnDelivey_Tapped(object sender, EventArgs e)
        {

        }
        private void btnDiscount_Tapped(object sender, EventArgs e)
        {

        }
        private void btnNote_Clicked(object sender, EventArgs e)
        {

        }

        private void btnCreditInvoice_Tapped(object sender, EventArgs e)
        {

        }
        private void btnSaveInvoice_Tapped(object sender, EventArgs e)
        {

        }

        private void PrintInvoice_Tapped(object sender, EventArgs e)
        {

        }





    }
}