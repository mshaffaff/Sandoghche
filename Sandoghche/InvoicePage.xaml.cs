using Rg.Plugins.Popup.Services;
using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Sandoghche.NotePopupPage;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InvoicePage : ContentPage
    {
        private static OrderViewModel order;
        private static double Tax1, Tax2;

        public InvoicePage(int ClientId, string ClientName)
        {
            InitializeComponent();

            lblClient.Text = ClientName;
            lblClientId.Text = ClientId.ToString();
            order = new OrderViewModel();

            if (order.Order.OrderId == null)
            {
                order.Order.OrderId = 1;
                order.Order.ReceiptNumber = 1;
                lblOrderId.Text = "1";
                lblReceipNumber.Text = "1";
            }
            else
            {
                lblOrderId.Text = (order.Order.OrderId + 1).ToString();
                lblReceipNumber.Text =(order.Order.ReceiptNumber + 1).ToString();

            }
        }

        async protected override void OnAppearing()
        {
            await SandoghcheController.GetConnection().CreateTableAsync<Category>();
            await SandoghcheController.GetConnection().CreateTableAsync<Product>();
            await SandoghcheController.GetConnection().CreateTableAsync<Order>();
            await SandoghcheController.GetConnection().CreateTableAsync<OrderDetail>();
            await SandoghcheController.GetConnection().CreateTableAsync<SandoghcheSetting>();

            await getCategories();
            await getSetting();

            base.OnAppearing();
        }

        async Task getSetting()
        {
            var tax = await SandoghcheController._connection.Table<SandoghcheSetting>().FirstOrDefaultAsync();
            if (tax == null)
            {
                Tax1 = 0;
                Tax2 = 0;
            }
            else
            {
                Tax1 = tax.Tax1;
                Tax2 = tax.Tax2;
            }
        }


        async Task getCategories(string Searchtext = null)
        {
            var categories = await SandoghcheController._connection.Table<Category>().Where(c => c.IsDeleted != true).ToListAsync();

            var result = new List<Category>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = categories;
            else
                result = categories.Where(c => c.CategoryText.Contains(Searchtext) && c.IsDeleted != true).ToList();

            lstCategory.ItemsSource = result;
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
        }

        private void lblClients_Tapped(object sender, EventArgs e)
        {
            var page = new ClientsPage();
            Navigation.PushAsync(page);
        }

        async private void srchCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            lstProducts.ItemsSource = null;
            //lblCategoryId.Text = "";
            //lblCategoryText.Text = "انتخاب نشده";
            //txtItem.Text = "";
            //txtProductPrice.Text = "";
            //srchProduct.Text = "";
            //btnAddItem.IsVisible = true;
            //btnCancelItem.IsVisible = false;
            //btnUpdateItem.IsVisible = false;
            //btnDeleteItem.IsVisible = false;
            //frmItems.IsEnabled = false;
            //frmItems.Opacity = 0.3;


            await getCategories(e.NewTextValue);




        }

        async private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var category = (Category)e.SelectedItem;
            lblCategoryId.Text = category.CategoryId.ToString();
            await getProducts(category.CategoryId);

        }

        private void lstProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        double totalPrice = 0;

        private void lstProducts_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var product = (Product)e.Item;

            order.Order.ClientId = Convert.ToInt32(lblClientId.Text);

            order.Order.ReceiptNumber = Convert.ToInt32(lblReceipNumber.Text);

            //order.Order.PaymentType = 1;

            //جمع کل یه محصول
            var detail = order.OrderDetail.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (detail != null)
            {
                var n = detail.Number + 1;
                detail.Number = n;
                detail.TotalPrice = n * detail.Price;
            }
            else
                order.OrderDetail.Add(new OrderDetailViewModel { RowNumber = order.OrderDetail.Count + 1, ProductId = product.ProductId, ProductText = product.ProductText, Number = 1, Price = product.ProductPrice, CategoryId = product.CategoryId, TotalPrice = product.ProductPrice });


            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetail;

            TotalPriceCalculator();


        }

        ////محاسبه جمع کل فاکتور 

        void TotalPriceCalculator()
        {
            double totalPrice = 0;

            foreach (var orderDetail in order.OrderDetail)
            {
                totalPrice += orderDetail.TotalPrice;
            }

            lblTotalPrice.Text = totalPrice.ToString();
            order.Order.TotalPrice = totalPrice;


            ///////
            order.Order.Tax1 = order.Order.TotalPrice * (Tax1 * 0.01);
            order.Order.Tax2 = order.Order.TotalPrice * (Tax2 * 0.01);

            lblTax.Text = (order.Order.Tax1 + order.Order.Tax2).ToString();

            ///////////////////



            var discountType = order.Order.DiscountType;
            var totalDiscount = order.Order.TotalPrice * (order.Order.DiscountPercent * 0.01);
            var discountPercent = order.Order.DiscountPercent;
            if (discountType == 0)
            {
                totalDiscount = order.Order.TotalPrice * (order.Order.DiscountPercent * 0.01);
                order.Order.TotalDiscount = totalDiscount;
                lblDiscount.Text = totalDiscount.ToString();
            }
            else
            {
                totalDiscount = order.Order.TotalDiscount;
                order.Order.TotalDiscount = totalDiscount;
                lblDiscount.Text = totalDiscount.ToString();
            }


            var serviceType = order.Order.ServiceType;
            var totalService = order.Order.TotalPrice * (order.Order.ServicePercent * 0.01);
            var servicePercent = order.Order.ServicePercent;
            if (serviceType == 0)
            {
                totalService = order.Order.TotalPrice * (order.Order.ServicePercent * 0.01);
                order.Order.TotalServiceFee = totalService;
                lblService.Text = totalService.ToString();
            }
            else
            {
                totalService = order.Order.TotalServiceFee;
                order.Order.TotalServiceFee = totalService;
                lblService.Text = totalService.ToString();
            }




            order.Order.FinalPayment = totalPrice - (totalDiscount + order.Order.TotalServiceFee + order.Order.DeliveryFee);
            if (order.Order.FinalPayment < 0)
            {
                DisplayAlert("اخطار", "مبلغ پرداختی نمیتواند منفی باشد", "باشه");
            }

            lblFinalPayment.Text = (totalPrice - (totalDiscount + order.Order.TotalServiceFee + order.Order.DeliveryFee + order.Order.Tax1 + order.Order.Tax2)).ToString();
        }


        async private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            await getProducts(Convert.ToInt32(lblCategoryId.Text), e.NewTextValue);
        }

        private void btnPlus_Clicked(object sender, EventArgs e)
        {
            var s = sender as ImageButton;
            var selectedItem = s.BindingContext;
            var orderDetail = (OrderDetailViewModel)selectedItem;

            orderDetail.Number++;
            orderDetail.TotalPrice = orderDetail.TotalPrice + orderDetail.Price;

            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetail;
            TotalPriceCalculator();

        }

        private void btnMines_Clicked(object sender, EventArgs e)
        {
            var s = sender as ImageButton;
            var selectedItem = s.BindingContext;
            var orderDetail = (OrderDetailViewModel)selectedItem;
            orderDetail.Number--;
            orderDetail.TotalPrice = orderDetail.TotalPrice - orderDetail.Price;
            if (orderDetail.Number == 0)
                order.OrderDetail.Remove(orderDetail);

            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetail;

            TotalPriceCalculator();
        }

        private void btnNote_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<PopupViewModel>(this, "txtComment", (value) =>
            {
                order.Order.Comment = value.CommentText;
            });

            PopupNavigation.Instance.PushAsync(new NotePopupPage());
        }

        private void btnDiscount_Tapped(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<PopupViewModel>(this, "Discount", (value) =>
            {
                if (value.DiscountType == 0)
                {
                    order.Order.DiscountType = 0;
                    order.Order.DiscountPercent = value.DiscountAmount;
                    order.Order.TotalDiscount = order.Order.TotalPrice * (value.DiscountAmount * 0.01);
                    lblDiscount.Text = (order.Order.TotalPrice * (value.DiscountAmount * 0.01)).ToString();
                    TotalPriceCalculator();

                }
                else
                {
                    order.Order.DiscountType = Convert.ToInt16(value.DiscountType);
                    order.Order.DiscountPercent = 0;
                    order.Order.TotalDiscount = value.DiscountAmount;
                    lblDiscount.Text = value.DiscountAmount.ToString();
                    TotalPriceCalculator();

                }


            });

            PopupNavigation.Instance.PushAsync(new DiscountPopupPage(order.Order.TotalPrice.ToString()));
        }

        private void btnDelivey_Tapped(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<PopupViewModel>(this, "txtDeliveryFee", (value) =>
            {
                order.Order.DeliveryFee = value.DeliveryFee;
                lblDelivery.Text = value.DeliveryFee.ToString();
                TotalPriceCalculator();

            });

            PopupNavigation.Instance.PushAsync(new DeliveryPopupPage());
        }

        private void btnService_Tapped(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<PopupViewModel>(this, "Service", (value) =>
            {
                if (value.ServiceType == 0)
                {
                    order.Order.ServiceType = 0;
                    order.Order.ServicePercent = value.ServiceAmount;
                    order.Order.TotalServiceFee = order.Order.TotalPrice * (value.ServiceAmount * 0.01);
                    lblService.Text = (order.Order.TotalPrice * (value.ServiceAmount * 0.01)).ToString();
                    TotalPriceCalculator();

                }
                else
                {
                    order.Order.ServiceType = Convert.ToInt16(value.ServiceType);
                    order.Order.ServicePercent = 0;
                    order.Order.TotalServiceFee = value.ServiceAmount;
                    lblService.Text = value.ServiceAmount.ToString();
                    TotalPriceCalculator();

                }


            });

            PopupNavigation.Instance.PushAsync(new ServicePopupPage(order.Order.TotalPrice.ToString()));

        }

        async private void btnSaveInvoice_Tapped(object sender, EventArgs e)
        {
            double finalPayment =Convert.ToDouble(lblFinalPayment.Text);
            if(finalPayment == 0)
            {
                await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر نمیتواتد در سیستم ثبت گردد", "باشه");
            }else
            {
                //await SandoghcheController._connection.InsertAsync(order.Order);
               //await SandoghcheController._connection.InsertAllAsync(order.OrderDetail);

                await DisplayAlert("صدور فاکتور", string.Format(" فاکتور {0}  شماره فیش {1} به مبلغ {2} ثبت شد", order.Order.ReceiptNumber, order.Order.OrderId+1,Convert.ToDouble(lblFinalPayment.Text)), "باشه");
                await Navigation.PushAsync(new InvoicePage(Convert.ToInt32(lblClientId.Text), lblClient.Text));
            }
        }

        private void btnMainMenu_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }





    }

}