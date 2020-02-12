using Rg.Plugins.Popup.Services;
using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ModelView;
using SQLiteNetExtensionsAsync.Extensions;
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
        static OrderDetail orderDetail;
        private static double Tax1, Tax2;

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
            var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync();

            order.OrderDetails = orderDetail;

            await SandoghcheController.GetConnection().CreateTableAsync<SandoghcheSetting>();
            await getSetting();



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
                item.ProductText = products.FirstOrDefault(p => p.ProductId == item.ProductId).ProductText;
                item.RowNumber = rowNumber;
                rowNumber++;
            }
            await getCategories();


            ProductsDataGrid.ItemsSource = orderDetail;




        }

        async Task SaveInvoice(int InvoiceType)
        {
            if (InvoiceType == 0)
            {
                order.PaymentType = 0;
                order.isEdited = true;
                order.EditedTime = DateTime.Now;
                double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
                if (finalPayment <= 0)
                {
                    await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر و کمتر نمیتواتد در سیستم ویرایش گردد", "باشه");
                }
                else
                {
                    await SandoghcheController._connection.UpdateAsync(order);
                    await SandoghcheController._connection.UpdateAllAsync(order.OrderDetails);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(order);

                    await DisplayAlert("ویرایش فاکتور", string.Format(" فاکتور {0}  شماره فیش {1} به مبلغ {2} ویرایش شد", order.ReceiptNumber, order.OrderId, Convert.ToDouble(lblFinalPayment.Text)), "باشه");
                    //await setOrderNumber();
                    lblTax.Text = "0";
                    lblDiscount.Text = "0";
                    lblService.Text = "0";
                    lblTax.Text = "0";
                    lblFinalPayment.Text = "0";
                    lblTotalPrice.Text = "0";
                    lblDelivery.Text = "0";

                    ProductsDataGrid.ItemsSource = null;
                    lstProducts.ItemsSource = null;
                    await Navigation.PushAsync(new SandoghcheMainPage());
                    //await getCategories();
                    // order = new Order();
                }//await Navigation.PushAsync(new InvoicePage(Convert.ToInt32(lblClientId.Text), lblClient.Text));
            }
            else if (InvoiceType == 1)
            {
                order.PaymentType = 1;
                order.isEdited = true;
                order.EditedTime = DateTime.Now;
                double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
                if (finalPayment <= 0)
                {
                    await DisplayAlert("ویرایش فاکتور", "فاکتور به مبلغ صفر و کمتر نمیتواتد در سیستم ویرایش گردد", "باشه");
                }
                else
                {
                    await SandoghcheController._connection.UpdateAsync(order);
                    await SandoghcheController._connection.UpdateAllAsync(order.OrderDetails);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(order);

                    await DisplayAlert(",ویرایش فاکتور", string.Format(" فاکتور {0}  شماره فیش {1} به مبلغ {2} ویرایش شد", order.ReceiptNumber, order.OrderId, Convert.ToDouble(lblFinalPayment.Text)), "باشه");
                    lblTax.Text = "0";
                    lblDiscount.Text = "0";
                    lblService.Text = "0";
                    lblTax.Text = "0";
                    lblFinalPayment.Text = "0";
                    lblTotalPrice.Text = "0";
                    lblDelivery.Text = "0";

                    ProductsDataGrid.ItemsSource = null;
                    lstProducts.ItemsSource = null;
                    await Navigation.PushAsync(new SandoghcheMainPage());

                }
            }
            else if (InvoiceType == 3)
            {
                bool result = await DisplayAlert("حذف فاکتور", "آیا از حذف این فاکتور اطمینان دارید", "بله", "خیر");
                if (result)
                {
                    order.isDeleted = true;
                    order.DeletedTime = DateTime.Now;
                    await SandoghcheController._connection.UpdateAsync(order);
                    await SandoghcheController._connection.UpdateAllAsync(order.OrderDetails);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(order);
                    await Navigation.PushAsync(new SandoghcheMainPage());
                }

            }
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
            order.Tax2 = order.TotalPrice * (Tax2 * 0.01);

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




            order.FinalPayment = totalPrice + order.TotalServiceFee + order.DeliveryFee + order.Tax1 + order.Tax2 - (totalDiscount); ;
            if (order.FinalPayment < 0)
            {
                DisplayAlert("اخطار", "مبلغ پرداختی نمیتواند منفی باشد", "باشه");
            }

            lblFinalPayment.Text = (totalPrice + order.TotalServiceFee + order.DeliveryFee + order.Tax1 + order.Tax2 - (totalDiscount)).ToString();
        }

        private void btnMines_Clicked(object sender, EventArgs e)
        {
            var s = sender as ImageButton;
            var selectedItem = s.BindingContext;
            var orderDetail = (OrderDetail)selectedItem;
            orderDetail.Number--;
            orderDetail.TotalPrice = orderDetail.TotalPrice - orderDetail.Price;
            if (orderDetail.Number == 0)
                order.OrderDetails.Remove(orderDetail);

            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetails;

            TotalPriceCalculator();
        }

        private void btnPlus_Clicked(object sender, EventArgs e)
        {
            var s = sender as ImageButton;
            var selectedItem = s.BindingContext;
            var orderDetail = (OrderDetail)selectedItem;

            orderDetail.Number++;
            orderDetail.TotalPrice = orderDetail.TotalPrice + orderDetail.Price;

            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetails;
            TotalPriceCalculator();
        }

        private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void lstProducts_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        double totalPrice = 0;
        private void lstProducts_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var product = (Product)e.Item;

            //order.ClientId = order.ClientId;

            // order.ReceiptNumber = Convert.ToInt32(lblReceipNumber.Text);

            //order.PaymentType = 1;

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


            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetails;

            TotalPriceCalculator();

        }

        private void srchCategory_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        async private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var category = (Category)e.SelectedItem;
            lblCategoryId.Text = category.CategoryId.ToString();
            await getProducts(category.CategoryId);

        }

        async private void btnCancelEdit_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditPage());
        }

        private void btnService_Tapped(object sender, EventArgs e)
        {
            if (lblFinalPayment.Text == "0")
                DisplayAlert("اخطار", "برای فاکتور به مبلغ صفر نمیتوان حق سرویس اضافه نمود", "باشه");
            else
            {
                MessagingCenter.Subscribe<PopupViewModel>(this, "Service", (value) =>
                {
                    if (value.ServiceType == 0)
                    {
                        order.ServiceType = 0;
                        order.ServicePercent = value.ServiceAmount;
                        order.TotalServiceFee = order.TotalPrice * (value.ServiceAmount * 0.01);
                        lblService.Text = (order.TotalPrice * (value.ServiceAmount * 0.01)).ToString();
                        TotalPriceCalculator();

                    }
                    else
                    {
                        order.ServiceType = Convert.ToInt16(value.ServiceType);
                        order.ServicePercent = 0;
                        order.TotalServiceFee = value.ServiceAmount;
                        lblService.Text = value.ServiceAmount.ToString();
                        TotalPriceCalculator();

                    }


                });

                PopupNavigation.Instance.PushAsync(new ServicePopupPage(order.TotalPrice.ToString(), order.ServiceType, order.ServicePercent, order.TotalServiceFee));
            }
        }
        
        private void btnDelivey_Tapped(object sender, EventArgs e)
        {
            if (lblFinalPayment.Text == "0")
                DisplayAlert("اخطار", "برای فاکتور به مبلغ صفر نمیتوان هزینه پیک اضافه نمود", "باشه");
            else
            {
                MessagingCenter.Subscribe<PopupViewModel>(this, "txtDeliveryFee", (value) =>
                {
                    order.DeliveryFee = value.DeliveryFee;
                    lblDelivery.Text = value.DeliveryFee.ToString();
                    TotalPriceCalculator();

                });

                PopupNavigation.Instance.PushAsync(new DeliveryPopupPage(order.DeliveryFee));
            }
        }

        private void btnDiscount_Tapped(object sender, EventArgs e)
        {

            if (lblFinalPayment.Text == "0")
                DisplayAlert("اخطار", "برای فاکتور به مبلغ صفر نمیتوان تخفیفی اضافه نمود", "باشه");
            else
            {
                MessagingCenter.Subscribe<PopupViewModel>(this, "Discount", (value) =>
                {
                    if (Convert.ToInt16(value.DiscountType) == 0)
                    {
                        order.DiscountType = 0;
                        order.DiscountPercent = value.DiscountAmount;
                        order.TotalDiscount = order.TotalPrice * (value.DiscountAmount * 0.01);
                        lblDiscount.Text = (order.TotalPrice * (value.DiscountAmount * 0.01)).ToString();

                        TotalPriceCalculator();

                    }
                    else
                    {
                        order.DiscountType = 1;
                        order.DiscountPercent = 0;
                        order.TotalDiscount = value.DiscountAmount;
                        lblDiscount.Text = value.DiscountAmount.ToString();
                        TotalPriceCalculator();

                    }


                });

                PopupNavigation.Instance.PushAsync(new DiscountPopupPage(order.TotalPrice.ToString(), order.DiscountType, order.DiscountPercent, order.TotalDiscount));
            }
        }
        
        private void btnNote_Clicked(object sender, EventArgs e)
        {
            if (lblFinalPayment.Text == "0")
                DisplayAlert("اخطار", "برای فاکتور به مبلغ صفر نمیتوان توضیحاتی اضافه نمود", "باشه");
            else
            {
                MessagingCenter.Subscribe<PopupViewModel>(this, "txtComment", (value) =>
                {
                    order.Comment = value.CommentText;
                });

                PopupNavigation.Instance.PushAsync(new NotePopupPage(order.Comment));
            }
        }

        async private void btnCreditInvoiceUpdate_Tapped(object sender, EventArgs e)
        {
            await SaveInvoice(1);
        }
        
        async private void btnSaveInvoiceUpdate_Tapped(object sender, EventArgs e)
        {
            await SaveInvoice(0);
        }
        
        async private void PrintInvoiceUpdate_Tapped(object sender, EventArgs e)
        {
            order.PaymentType = 0;
            order.isEdited = true;
            order.EditedTime = DateTime.Now;

            double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
            if (finalPayment == 0)
            {
                await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر نمیتواتد در سیستم ثبت گردد", "باشه");
            }
            else
            {
                await SandoghcheController._connection.UpdateAsync(order);
                await SandoghcheController._connection.UpdateAllAsync(order.OrderDetails);
                await SandoghcheController._connection.UpdateWithChildrenAsync(order);



                DependencyService.Get<IPrint>().Print(order, "فاکتور فروش - ویرایش شده");

                lblTax.Text = "0";
                lblDiscount.Text = "0";
                lblService.Text = "0";
                lblTax.Text = "0";
                lblFinalPayment.Text = "0";
                lblTotalPrice.Text = "0";
                lblDelivery.Text = "0";

                ProductsDataGrid.ItemsSource = null;
                lstProducts.ItemsSource = null;
                await Navigation.PushAsync(new SandoghcheMainPage());

            }
        }
        
        async private void btnDeleteOrder_Tapped(object sender, EventArgs e)
        {
            await SaveInvoice(3);
        }





    }
}