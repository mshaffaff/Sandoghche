using Sandoghche.Components;
using Sandoghche.Models;
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
    public partial class InvoicePageMobile : ContentPage
    {
        private static Order order;
        private static int clientId;
        private static int categoryId;
        private static double Tax1, Tax2;


        public InvoicePageMobile()
        {
            InitializeComponent();
            order = new Order();


        }
        async protected override void OnAppearing()
        {
            await GetClients();
            await getCategories();
            await getSetting();
            await setOrderNumber();
            await setReceiptNumber();

            base.OnAppearing();
        }

        async Task setOrderNumber()
        {
            var lastOrder = await SandoghcheController._connection.Table<Order>().OrderByDescending(x => x.OrderId).FirstOrDefaultAsync();

            if (lastOrder == null)
            {
                lblOrderId.Text = "1";
            }
            else
            {
                lblOrderId.Text = (lastOrder.OrderId + 1).ToString();
                // lblReceipNumber.Text = (lastOrder.ReceiptNumber + 1).ToString();
            }
        }

        async Task setReceiptNumber()
        {
            var settings = await SandoghcheController._connection.Table<SandoghcheSetting>().FirstOrDefaultAsync();

            if (settings == null)
            {
                await DisplayAlert("خطا", "تنظیمات سیستم هنوز اعمال نشده است", "باشه");
                return;
            }
            var lastOrder = await SandoghcheController._connection.Table<Order>().OrderByDescending(x => x.OrderId).FirstOrDefaultAsync();

            var startFrom = settings.ReceiptNumberStartFrom;
            var resetTime = settings.ResetReceiptTime;

            if (lastOrder == null)
                lblReceipNumber.Text = startFrom.ToString();
            else if (Convert.ToDateTime(lastOrder.DateCreated).Date == DateTime.Today.Date)
                lblReceipNumber.Text = (lastOrder.ReceiptNumber + 1).ToString();
            else if (DateTime.Now.TimeOfDay > resetTime)
                lblReceipNumber.Text = startFrom.ToString();




        }

        public class ClientAutoCompleteViewFilter : IAutoCompleteFilter
        {
            public bool Filter(object item, string searchText, CompletionMode completionMode)
            {
                Client client = (Client)item;
                string clientName = client.ClientName;
                return clientName.Contains(searchText);
            }
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

        async Task GetClients()
        {
            var query = "select  Clients.ClientId,Clients.ClientName from Clients where Clients.IsDeleted!=1 AND Clients.IsActive=1";
            var clients = await SandoghcheController.GetConnection().QueryAsync<Client>(query);
            srchClients.ItemsSource = clients.ToList();
            srchClients.Filter = new ClientAutoCompleteViewFilter();
        }
        async Task getCategories()
        {
            var categories = await SandoghcheController.GetConnection().Table<Category>().Where(c => c.IsDeleted != true).ToListAsync();
            pkrCategory.ItemsSource = categories;

            //var result = new List<Category>();
            //if (String.IsNullOrWhiteSpace(Searchtext))
            //    result = categories;
            //else
            //    result = categories.Where(c => c.CategoryText.Contains(Searchtext) && c.IsDeleted != true).ToList();

            //lstCategory.ItemsSource = result;
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
        public class ClientCreditViewModel
        {
            public double Amount { get; set; }
        }
        async private Task ClientCreditStatus(int ClientId)
        {
            var query = "select (sum(DebtorAmount)-sum(CreditorAmount)) as 'Amount' from Accounting WHERE ClientId=" + ClientId;
            var amount = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);



            lblCreditStatus.Text = "مانده : " + amount.FirstOrDefault()?.Amount.ToString() ?? 0.ToString();
        }

        async private void srchClients_SuggestionItemSelected(object sender, SuggestionItemSelectedEventArgs e)
        {
            var client = (Client)e.DataItem;
            clientId = client.ClientId;
            await ClientCreditStatus(clientId);

        }

        async private void pkrCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            var category = (Category)pkrCategory.SelectedItem;

            categoryId = category.CategoryId;
            await getProducts(categoryId);
        }

        async private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            await getProducts(categoryId, e.NewTextValue);
        }

         private void btnAddToBasket_Clicked(object sender, EventArgs e)
        {

            var product = (sender as Button).CommandParameter as Product;

            if (product.IsActive)
            {
                order.ClientId = clientId;

                order.ReceiptNumber = Convert.ToInt32(lblReceipNumber.Text);


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
        private void btnMines_Clicked(object sender, EventArgs e)
        {

        }

        private void tabView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //DisplayAlert("test",e.PropertyName, "test");
            if (e.PropertyName == "SelectedItem")
            {
                var selectedTab = tabView.SelectedItem as TabViewItem;
                if (selectedTab.HeaderText == "TabBasket")
                {
                    ProductsDataGrid.ItemsSource = null;

                    ProductsDataGrid.ItemsSource = order.OrderDetails;
                }
            }
        }



        private void btnPlus_Clicked(object sender, EventArgs e)
        {

        }
    }
}