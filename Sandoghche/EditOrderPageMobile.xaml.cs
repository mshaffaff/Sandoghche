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
    public partial class EditOrderPageMobile : ContentPage
    {
        private static int OrderId;
        private static int TotalNumberOfItem = 0;
        private static int ClientId;
        private static Order order;
        private static int categoryId;
        private static double Tax1, Tax2;

        public EditOrderPageMobile(int orderId = 0)
        {
            OrderId = orderId;

            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            //lblTotalNumberOfItem.Text = "سبد " + "( " + 0.ToString() + " )";
            TotalNumberOfItem = 0;
            order = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == OrderId);
            var orderdetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(od => od.OrderId == order.OrderId).ToListAsync();
            order.OrderDetails = orderdetails;

            var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync();
            int rowNumber = 1;
            foreach (var item in order.OrderDetails)
            {
                item.ProductText = products.FirstOrDefault(p => p.ProductId == item.ProductId).ProductText;
                item.RowNumber = rowNumber;

                rowNumber++;
            }



            ClientId = order.ClientId;
            foreach (var item in orderdetails)
            {
                TotalNumberOfItem = TotalNumberOfItem + item.Number;
            }

            lblTotalNumberOfItem.Text = "سبد " + "( " + TotalNumberOfItem.ToString() + " )";

            // ProductsDataGrid.ItemsSource = orderdetails;

            TabBasket.IsSelected = true;

            await GetClient();
            await getCategories();
            await getSetting();

            lblFinalPayment.Text = order.FinalPayment.ToString();
            lblDelivery.Text = order.DeliveryFee.ToString();
            lblDiscount.Text = order.TotalDiscount.ToString();
            lblService.Text = order.TotalServiceFee.ToString();
            lblTax.Text = (order.Tax1 + order.Tax2).ToString();
            lblTotalPrice.Text = order.TotalPrice.ToString();

            base.OnAppearing();
        }

        async Task getSetting()
        {
            var tax = await SandoghcheController._connection.Table<SandoghcheSetting>().FirstOrDefaultAsync();
            if (tax == null)
            {
                await DisplayAlert("خطا", "تنظیمات سیستم هنوز اعمال نشده است", "باشه");
                await Navigation.PushAsync(new SettingsPage());
            }
            else
            {
                Tax1 = tax.Tax1;
                Tax2 = tax.Tax2;
            }
        }

        async Task getProducts(int categoryId, string Searchtext = null)
        {
            var products = await SandoghcheController._connection.Table<Product>().Where(p => p.IsDeleted != true && p.CategoryId == categoryId).ToListAsync();
            var result = new List<Product>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = products;
            else
                result = products.Where(p => p.ProductText.Contains(Searchtext) && p.IsDeleted != true && p.CategoryId == categoryId).ToList();

            lstProducts.ItemsSource = result;
            // DataGridProduct.ItemsSource = result;
        }

        async Task GetClient()
        {
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == ClientId);
            //srchClients.ItemsSource = clients.ToList();
            //srchClients.Filter = new ClientAutoCompleteViewFilter();
            ClientId = client.ClientId;
            srchClients.Text = client.ClientName;
            srchClients.IsEnabled = false;
        }

        async Task getCategories()
        {
            var categories = await SandoghcheController.GetConnection().Table<Category>().Where(c => c.IsDeleted != true).ToListAsync();
            pkrCategory.ItemsSource = categories;

        }

        async private void tabView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                var selectedTab = tabView.SelectedItem as TabViewItem;
                if (selectedTab.HeaderText == "TabBasket")
                {
                    ProductsDataGrid.ItemsSource = new List<string>();
                    //var orderdetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(od => od.OrderId == OrderId).ToListAsync();

                    //lblTotalNumberOfItem.Text = "سبد " + "( " + TotalNumberOfItem.ToString() + " )";

                    ProductsDataGrid.ItemsSource = order.OrderDetails;
                }
                if (selectedTab.HeaderText == "TabHome")
                {
                    await Navigation.PushAsync(new SandoghcheMainPage());
                }
            }
        }

        private void btnMines_Clicked(object sender, EventArgs e)
        {
            var s = sender as Label;
            var selectedItem = s.BindingContext;
            var orderDetail = (OrderDetail)selectedItem;
            orderDetail.Number--;
            orderDetail.TotalPrice = orderDetail.TotalPrice - orderDetail.Price;
            if (orderDetail.Number == 0)
                order.OrderDetails.Remove(orderDetail);

            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetails;
            TotalNumberOfItem--;
            lblTotalNumberOfItem.Text = "سبد " + "( " + TotalNumberOfItem.ToString() + " )";
            TotalPriceCalculator();
        }



        private void btnPlus_Clicked(object sender, EventArgs e)
        {
            var s = sender as Label;
            var selectedItem = s.BindingContext;
            var orderDetail = (OrderDetail)selectedItem;

            orderDetail.Number++;
            orderDetail.TotalPrice = orderDetail.TotalPrice + orderDetail.Price;

            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetails;
            TotalNumberOfItem++;
            lblTotalNumberOfItem.Text = "سبد " + "( " + TotalNumberOfItem.ToString() + " )";
            TotalPriceCalculator();
        }

        private void btnService_Clicked(object sender, EventArgs e)
        {

        }

        private void btnDelivery_Clicked(object sender, EventArgs e)
        {

        }

        private void btnDiscount_Clicked(object sender, EventArgs e)
        {

        }

        private void btnSaveInvoiceCredit_Clicked(object sender, EventArgs e)
        {

        }

        private void btnSaveInvoice_Clicked(object sender, EventArgs e)
        {

        }

        private void srchClients_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void srchClients_SuggestionItemSelected(object sender, Telerik.XamarinForms.Input.AutoComplete.SuggestionItemSelectedEventArgs e)
        {

        }

        private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        async private void pkrCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var category = (Category)pkrCategory.SelectedItem;
            if (category != null)
            {
                categoryId = category.CategoryId;
                await getProducts(categoryId);
            }
        }

        private void btnAddToBasket_Clicked(object sender, EventArgs e)
        {
            //if (ClientId == 0 || srchClients.Text == "")
            //{
            //    await DisplayAlert("خطا", "مشتری انتخاب نشده است", "باشه");
            //}
            //else
            //{
            var product = (sender as Button).CommandParameter as Product;
            if (product.IsActive)
            {
                order.ClientId = ClientId;
                // order.ReceiptNumber = Convert.ToInt32(lblReceipNumber.Text);

                //جمع کل یه محصول
                if (order.OrderDetails == null)
                    order.OrderDetails = new List<OrderDetail>();

                var detail = order.OrderDetails?.FirstOrDefault(x => x.ProductId == product.ProductId);

                if (detail != null)
                {
                    var n = detail.Number + 1;
                    detail.Number = n;
                    detail.TotalPrice = n * detail.Price;
                }
                else
                    order.OrderDetails.Add(new OrderDetail { RowNumber = order.OrderDetails.Count + 1, ProductId = product.ProductId, ProductText = product.ProductText, Number = 1, Price = product.ProductPrice, CategoryId = product.CategoryId, TotalPrice = product.ProductPrice });


                //ProductsDataGrid.ItemsSource = null;

                //ProductsDataGrid.ItemsSource = order.OrderDetails;

                TotalNumberOfItem++;
                lblTotalNumberOfItem.Text = "سبد " + "( " + TotalNumberOfItem.ToString() + " )";
                TotalPriceCalculator();
            }
        }

        void TotalPriceCalculator()
        {
            double totalPrice = 0;

            foreach (var orderDetail in order.OrderDetails)
            {
                totalPrice += orderDetail.TotalPrice;
            }

            lblTotalPrice.Text = totalPrice.ToString();
            order.TotalPrice = totalPrice;


            ///////

            order.Tax1 = order.TotalPrice * (Tax1 * 0.01);
            order.Tax1Percent = Tax1;
            order.Tax2 = order.TotalPrice * (Tax2 * 0.01);
            order.Tax2Percent = Tax2;
            lblTax.Text = (order.Tax1 + order.Tax2).ToString();

            ///////////////////



            var discountType = order.DiscountType;
            var totalDiscount = order.TotalPrice * (order.DiscountPercent * 0.01);
            var discountPercent = order.DiscountPercent;
            if (discountType == 0)
            {
                totalDiscount = order.TotalPrice * (order.DiscountPercent * 0.01);
                order.TotalDiscount = totalDiscount;
                lblDiscount.Text = totalDiscount.ToString();
            }
            else
            {
                totalDiscount = order.TotalDiscount;
                order.TotalDiscount = totalDiscount;
                lblDiscount.Text = totalDiscount.ToString();
            }


            var serviceType = order.ServiceType;
            var totalService = order.TotalPrice * (order.ServicePercent * 0.01);
            var servicePercent = order.ServicePercent;
            if (serviceType == 0)
            {
                totalService = order.TotalPrice * (order.ServicePercent * 0.01);
                order.TotalServiceFee = totalService;
                lblService.Text = totalService.ToString();
            }
            else
            {
                totalService = order.TotalServiceFee;
                order.TotalServiceFee = totalService;
                lblService.Text = totalService.ToString();
            }




            order.FinalPayment = totalPrice + order.TotalServiceFee + order.DeliveryFee + order.Tax1 + order.Tax2 - (totalDiscount);
            if (order.FinalPayment < 0)
            {
                DisplayAlert("اخطار", "مبلغ پرداختی نمیتواند منفی باشد", "باشه");
            }

            lblFinalPayment.Text = (totalPrice + order.TotalServiceFee + order.DeliveryFee + order.Tax1 + order.Tax2 - (totalDiscount)).ToString();
        }
    }

}