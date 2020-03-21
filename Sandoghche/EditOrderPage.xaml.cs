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
        private static int EditedLogId;
        static Order order;
        static EditedOrdersLogs edited = new EditedOrdersLogs();
        static EditedOrdersLogs EditedOrder;
        static OrderDetail orderDetail;
        static string ClientName;
        private static double Tax1, Tax2;
        private static int EditDelayTime;
        private static bool ForHistory;

        public EditOrderPage(int orderId, bool forHistory = false, int editedLogId = 0)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            OrderId = orderId;
            ForHistory = forHistory;
            EditedLogId = editedLogId;

        }

        protected override async void OnAppearing()
        {

            if (ForHistory)
            {
                EditedOrder = await SandoghcheController.GetConnection().Table<EditedOrdersLogs>().FirstOrDefaultAsync(o => o.EditedLogId == EditedLogId);
                btnSaveInvoiceUpdate.IsVisible = false;
                btnPrintInvoiceUpdate.IsVisible = false;
                btnCreditInvoiceUpdate.IsVisible = false;
                btnDeleteOrder.IsVisible = false;



                var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == EditedOrder.ClientId);
                var EditedOrderDetail = await SandoghcheController.GetConnection().Table<EditedOrderDetailsLogs>().Where(d => d.EditedLogId == EditedLogId).ToListAsync();
                var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync();
                ClientName = client.ClientName;
                EditedOrder.EditedOrderDetailsLogs = EditedOrderDetail;


                lblOrderId.Text = EditedOrder.OrderId.ToString();
                lblReceipNumber.Text = EditedOrder.ReceiptNumber.ToString();
                lblPersianDate.Text = SandoghcheController.GetPersianDate(Convert.ToDateTime(EditedOrder.DateCreated));
                lblClient.Text = client.ClientName;
                lblFinalPayment.Text = EditedOrder.FinalPayment.ToString();
                lblDelivery.Text = EditedOrder.DeliveryFee.ToString();
                lblDiscount.Text = EditedOrder.TotalDiscount.ToString();
                lblService.Text = EditedOrder.TotalServiceFee.ToString();
                lblTax.Text = (EditedOrder.Tax1 + EditedOrder.Tax2).ToString();
                lblTotalPrice.Text = EditedOrder.TotalPrice.ToString();


                int rowNumber = 1;
                foreach (var item in EditedOrderDetail)
                {
                    item.ProductText = products.FirstOrDefault(p => p.ProductId == item.ProductId).ProductText;
                    item.RowNumber = rowNumber;
                    item.isEnabled = false;
                    rowNumber++;
                }

                ProductsDataGrid.ItemsSource = EditedOrderDetail;

            }
            else
            {
                order = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == OrderId);

                if (order.isDeleted)
                {
                    btnSaveInvoiceUpdate.IsVisible = false;
                    btnPrintInvoiceUpdate.IsVisible = false;
                    btnCreditInvoiceUpdate.IsVisible = false;
                    btnDeleteOrder.IsVisible = false;
                }
                else
                {
                    btnSaveInvoiceUpdate.IsVisible = true;
                    btnPrintInvoiceUpdate.IsVisible = true;
                    btnCreditInvoiceUpdate.IsVisible = true;
                    btnDeleteOrder.IsVisible = true;
                }


                if (order.PaymentType == 1 || order.isDeleted)
                {
                    btnSaveInvoiceUpdate.IsVisible = false;
                    btnPrintInvoiceUpdate.IsVisible = false;


                }
                else
                {
                    btnSaveInvoiceUpdate.IsVisible = true;
                    btnPrintInvoiceUpdate.IsVisible = true;
                }




                var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == order.ClientId);
                var orderDetail = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(d => d.OrderId == order.OrderId).ToListAsync();
                var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync();

                order.OrderDetails = orderDetail;
                ClientName = client.ClientName;

                await getSetting();

                lblOrderId.Text = order.OrderId.ToString();
                lblReceipNumber.Text = order.ReceiptNumber.ToString();
                lblPersianDate.Text = SandoghcheController.GetPersianDate(Convert.ToDateTime(order.DateCreated));
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

                await ClientCreditStatus(order.ClientId.ToString());

                var EditAlloweTime = Convert.ToDateTime(order.DateCreated).AddMinutes(EditDelayTime);

                if (DateTime.Now > EditAlloweTime)
                {
                    btnSaveInvoiceUpdate.IsVisible = false;
                    btnPrintInvoiceUpdate.IsVisible = false;
                    btnCreditInvoiceUpdate.IsVisible = false;
                    await DisplayAlert("خطا", "زمان ویرایش به اتمام رسیده است", "باشه");
                }
            }


            base.OnAppearing();

        }

        async private Task UpdateProductsAmount(bool forDelate = false)
        {
            var products = await SandoghcheController.GetConnection().Table<Product>().Where(p => p.IsDeleted != true).ToListAsync();

            if (forDelate)
            {
                var AddedOfNewProduct = order.OrderDetails;

                foreach (var product in products)
                    foreach (var item in AddedOfNewProduct)
                    {
                        if (product.ProductId == item.ProductId)
                            product.ProductAmount = product.ProductAmount + item.Number;
                    }

                await SandoghcheController._connection.UpdateAllAsync(products);
            }
            else
            {
                var ListOfeditedProduct = edited.EditedOrderDetailsLogs;
                foreach (var product in products)
                    foreach (var item in ListOfeditedProduct)
                    {
                        if (product.ProductId == item.ProductId)
                            product.ProductAmount = product.ProductAmount + item.Number;
                    }

                await SandoghcheController._connection.UpdateAllAsync(products);

                var AddedOfNewProduct = order.OrderDetails;

                foreach (var product in products)
                    foreach (var item in AddedOfNewProduct)
                    {
                        if (product.ProductId == item.ProductId)
                            product.ProductAmount = product.ProductAmount - item.Number;
                    }

                await SandoghcheController._connection.UpdateAllAsync(products);

            }

        }

        public class ClientCreditViewModel
        {
            public double Amount { get; set; }
        }
        async private Task ClientCreditStatus(string ClientId)
        {
            var query = "select (sum(DebtorAmount)-sum(CreditorAmount)) as 'Amount' from Accounting WHERE ClientId=" + Convert.ToInt32(ClientId);
            var amount = await SandoghcheController.GetConnection().QueryAsync<ClientCreditViewModel>(query);



            lblCreditStatus.Text = "مانده : " + amount.FirstOrDefault()?.Amount.ToString() ?? 0.ToString();
        }

        async Task SaveInvoice(int InvoiceType)
        {
            if (InvoiceType == 0)
            {
                order.PaymentType = 0;
                order.isEdited = true;
                order.EditedTime = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
                if (finalPayment <= 0)
                {
                    await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر و کمتر نمیتواتد در سیستم ویرایش گردد", "باشه");
                }
                else
                {

                    var OriginalOrder = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == OrderId);
                    var OriginalOrderDetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(d => d.OrderId == order.OrderId).ToListAsync();

                    //order = OriginalOrder;
                    //order.OrderDetails = OriginalOrderDetails;

                    await SandoghcheController._connection.DeleteAllAsync(OriginalOrderDetails);
                    await SandoghcheController._connection.InsertAllAsync(order.OrderDetails);
                    await SandoghcheController._connection.UpdateAsync(order);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(order);


                    edited.ClientId = OriginalOrder.ClientId;
                    edited.Comment = OriginalOrder.Comment;
                    edited.DateCreated = OriginalOrder.DateCreated;
                    edited.DeliveryFee = OriginalOrder.DeliveryFee;
                    edited.DiscountPercent = OriginalOrder.DiscountPercent;
                    edited.DiscountType = OriginalOrder.DiscountType;
                    edited.EditedDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                    edited.FinalPayment = OriginalOrder.FinalPayment;
                    edited.OrderId = OriginalOrder.OrderId;
                    edited.PaymentType = OriginalOrder.PaymentType;
                    edited.ReceiptNumber = OriginalOrder.ReceiptNumber;
                    edited.ServicePercent = OriginalOrder.ServicePercent;
                    edited.ServiceType = OriginalOrder.ServiceType;
                    edited.Tax1 = OriginalOrder.Tax1;
                    edited.Tax1Percent = OriginalOrder.Tax1Percent;
                    edited.Tax2 = OriginalOrder.Tax2;
                    edited.Tax2Percent = OriginalOrder.Tax2Percent;
                    edited.TotalDiscount = OriginalOrder.TotalDiscount;
                    edited.TotalPrice = OriginalOrder.TotalPrice;
                    edited.TotalServiceFee = OriginalOrder.TotalServiceFee;

                    edited.EditedOrderDetailsLogs = new List<EditedOrderDetailsLogs>();
                    foreach (var item in OriginalOrderDetails)
                    {
                        edited.EditedOrderDetailsLogs.Add(new EditedOrderDetailsLogs
                        {
                            CategoryId = item.CategoryId,
                            DetailId = item.DetailId,
                            Number = item.Number,
                            OrderId = item.OrderId,
                            Price = item.Price,
                            ProductId = item.ProductId,
                            TotalPrice = item.TotalPrice
                        });

                    }

                    await SandoghcheController._connection.InsertAsync(edited);
                    await SandoghcheController._connection.InsertAllAsync(edited.EditedOrderDetailsLogs);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(edited);

                    //edited.EditedOrderDetailsLogs = editedOrderDetailsLogs;

                    await UpdateProductsAmount();

                    await DisplayAlert("ویرایش فاکتور", string.Format(" فاکتور {0}  شماره فیش {1} به مبلغ {2} ویرایش شد", order.ReceiptNumber, order.OrderId, Convert.ToDouble(lblFinalPayment.Text)), "باشه");

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

            else if (InvoiceType == 1)
            {
                order.PaymentType = 1;
                order.isEdited = true;
                order.EditedTime = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"); ;
                double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
                if (finalPayment <= 0)
                {
                    await DisplayAlert("ویرایش فاکتور", "فاکتور به مبلغ صفر و کمتر نمیتواتد در سیستم ویرایش گردد", "باشه");
                }
                else
                {
                    Accounting accounting = new Accounting();
                    accounting.ClientId = order.ClientId;

                    accounting.CreditorAmount = 0;


                    var OriginalOrder = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == OrderId);
                    var OriginalOrderDetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(d => d.OrderId == order.OrderId).ToListAsync();

                    if (OriginalOrder.PaymentType == 0)
                        accounting.DebtorAmount = order.FinalPayment;
                    else if (OriginalOrder.PaymentType == 1)
                        accounting.DebtorAmount = order.FinalPayment - OriginalOrder.FinalPayment;





                    await SandoghcheController._connection.DeleteAllAsync(OriginalOrderDetails);
                    await SandoghcheController._connection.InsertAllAsync(order.OrderDetails);
                    await SandoghcheController._connection.UpdateAsync(order);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(order);

                    await SandoghcheController._connection.InsertAsync(accounting);


                    edited.ClientId = OriginalOrder.ClientId;
                    edited.Comment = OriginalOrder.Comment;
                    edited.DateCreated = OriginalOrder.DateCreated;
                    edited.DeliveryFee = OriginalOrder.DeliveryFee;
                    edited.DiscountPercent = OriginalOrder.DiscountPercent;
                    edited.DiscountType = OriginalOrder.DiscountType;
                    edited.EditedDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                    edited.FinalPayment = OriginalOrder.FinalPayment;
                    edited.OrderId = OriginalOrder.OrderId;
                    edited.PaymentType = OriginalOrder.PaymentType;
                    edited.ReceiptNumber = OriginalOrder.ReceiptNumber;
                    edited.ServicePercent = OriginalOrder.ServicePercent;
                    edited.ServiceType = OriginalOrder.ServiceType;
                    edited.Tax1 = OriginalOrder.Tax1;
                    edited.Tax1Percent = OriginalOrder.Tax1Percent;
                    edited.Tax2 = OriginalOrder.Tax2;
                    edited.Tax2Percent = OriginalOrder.Tax2Percent;
                    edited.TotalDiscount = OriginalOrder.TotalDiscount;
                    edited.TotalPrice = OriginalOrder.TotalPrice;
                    edited.TotalServiceFee = OriginalOrder.TotalServiceFee;


                    edited.EditedOrderDetailsLogs = new List<EditedOrderDetailsLogs>();
                    foreach (var item in OriginalOrderDetails)
                    {
                        edited.EditedOrderDetailsLogs.Add(new EditedOrderDetailsLogs
                        {
                            //EditedLogId = edited.EditedLogId,
                            CategoryId = item.CategoryId,
                            DetailId = item.DetailId,
                            Number = item.Number,
                            OrderId = item.OrderId,
                            Price = item.Price,
                            ProductId = item.ProductId,
                            TotalPrice = item.TotalPrice
                        });

                    }

                    await SandoghcheController._connection.InsertAsync(edited);
                    await SandoghcheController._connection.InsertAllAsync(edited.EditedOrderDetailsLogs);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(edited);


                    await UpdateProductsAmount();



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
                    if (order.PaymentType == 1)
                    {
                        await DisplayAlert("خطا", "به دلیل وجود اطلاعات مرتبط یا  بدهی امکان حذف نمی باشد", "باشه");
                        return;
                    }

                    var OriginalOrder = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == OrderId);
                    var OriginalOrderDetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(od => od.OrderId == OrderId).ToListAsync();
                    OriginalOrder.isDeleted = true;
                    OriginalOrder.DeletedTime = OriginalOrder.DeletedTime = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");

                    await SandoghcheController._connection.UpdateAsync(OriginalOrder);
                    order = OriginalOrder;
                    order.OrderDetails = OriginalOrderDetails;

                    await UpdateProductsAmount(true);

                    await Navigation.PushAsync(new SandoghcheMainPage());
                }

            }
        }

        async Task getSetting()
        {
            var tax = await SandoghcheController._connection.Table<SandoghcheSetting>().FirstOrDefaultAsync();
            EditDelayTime = tax.EditDelayTime;
            if (tax == null)
            {
                await DisplayAlert("خطا", "تنظیمات سیستم هنوز اعمال نشده است", "باشه");
                await Navigation.PushAsync(new SettingsPage());
            }
            else
            {
                Tax1 = order.Tax1Percent;
                Tax2 = order.Tax2Percent;
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
            if (ForHistory)
                await Navigation.PushAsync(new OrderHistoryPage(EditedOrder.OrderId));
            else
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
                if (ForHistory)
                    PopupNavigation.Instance.PushAsync(new ServicePopupPage(EditedOrder.TotalPrice.ToString(), EditedOrder.ServiceType, EditedOrder.ServicePercent, EditedOrder.TotalServiceFee, true));
                else
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
                if (ForHistory)
                    PopupNavigation.Instance.PushAsync(new DeliveryPopupPage(EditedOrder.DeliveryFee, true));
                else
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
                if (ForHistory)
                    PopupNavigation.Instance.PushAsync(new DiscountPopupPage(EditedOrder.TotalPrice.ToString(), EditedOrder.DiscountType, EditedOrder.DiscountPercent, EditedOrder.TotalDiscount, true));
                else
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
                if (ForHistory)
                    PopupNavigation.Instance.PushAsync(new NotePopupPage(EditedOrder.Comment, true));
                else
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
            order.EditedTime = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"); ;

            double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
            if (finalPayment == 0)
            {
                await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر نمیتواتد در سیستم ثبت گردد", "باشه");
            }
            else
            {
                var OriginalOrder = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == OrderId);
                var OriginalOrderDetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(d => d.OrderId == order.OrderId).ToListAsync();


                await SandoghcheController._connection.DeleteAllAsync(OriginalOrderDetails);
                await SandoghcheController._connection.InsertAllAsync(order.OrderDetails);
                await SandoghcheController._connection.UpdateAsync(order);
                await SandoghcheController._connection.UpdateWithChildrenAsync(order);




                edited.ClientId = OriginalOrder.ClientId;
                edited.Comment = OriginalOrder.Comment;
                edited.DateCreated = OriginalOrder.DateCreated;
                edited.DeliveryFee = OriginalOrder.DeliveryFee;
                edited.DiscountPercent = OriginalOrder.DiscountPercent;
                edited.DiscountType = OriginalOrder.DiscountType;
                edited.EditedDate = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                edited.FinalPayment = OriginalOrder.FinalPayment;
                edited.OrderId = OriginalOrder.OrderId;
                edited.PaymentType = OriginalOrder.PaymentType;
                edited.ReceiptNumber = OriginalOrder.ReceiptNumber;
                edited.ServicePercent = OriginalOrder.ServicePercent;
                edited.ServiceType = OriginalOrder.ServiceType;
                edited.Tax1 = OriginalOrder.Tax1;
                edited.Tax1Percent = OriginalOrder.Tax1Percent;
                edited.Tax2 = OriginalOrder.Tax2;
                edited.Tax2Percent = OriginalOrder.Tax2Percent;
                edited.TotalDiscount = OriginalOrder.TotalDiscount;
                edited.TotalPrice = OriginalOrder.TotalPrice;
                edited.TotalServiceFee = OriginalOrder.TotalServiceFee;


                var editedOrderDetailsLogs = new List<EditedOrderDetailsLogs>();

                foreach (var item in OriginalOrderDetails)
                {
                    editedOrderDetailsLogs.Add(new EditedOrderDetailsLogs
                    {

                        CategoryId = item.CategoryId,
                        DetailId = item.DetailId,
                        Number = item.Number,
                        OrderId = item.OrderId,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        TotalPrice = item.TotalPrice
                    });

                }

                await SandoghcheController._connection.InsertAsync(edited);
                await SandoghcheController._connection.InsertAllAsync(editedOrderDetailsLogs);
                await SandoghcheController._connection.UpdateWithChildrenAsync(edited);

                edited.EditedOrderDetailsLogs = editedOrderDetailsLogs;
                await UpdateProductsAmount();

                DependencyService.Get<IPrint>().Print(order, "فاکتور فروش - ویرایش شده",ClientName);

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


        protected override bool OnBackButtonPressed()
        {
            return true;
        }


    }
}