using Rg.Plugins.Popup.Services;
using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Sandoghche.NotePopupPage;
using System.IO;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Grid;
using Syncfusion.SfDataGrid.XForms.Exporting;
using System.Reflection;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InvoicePage : ContentPage
    {
        private static Order order;

        private static double Tax1, Tax2;
        static string clientName;

        public InvoicePage(int ClientId, string ClientName)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            lblPersianDate.Text = SandoghcheController.GetPersianDate(null);
            lblClient.Text = "انتخاب مشتری : " + ClientName;
            lblClientId.Text = ClientId.ToString();
            clientName = ClientName;
            order = new Order();


        }

        async protected override void OnAppearing()
        {
            await getCategories();
            await getSetting();
            await setOrderNumber();
            await setReceiptNumber();
            await ClientCreditStatus(lblClientId.Text);

            base.OnAppearing();
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
            if (product.IsActive)
            {
                order.ClientId = Convert.ToInt32(lblClientId.Text);

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


                ProductsDataGrid.ItemsSource = null;
                ProductsDataGrid.ItemsSource = order.OrderDetails;

                TotalPriceCalculator();
            }




        }

        ////محاسبه جمع کل فاکتور 

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


        async private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            await getProducts(Convert.ToInt32(lblCategoryId.Text), e.NewTextValue);
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

                PopupNavigation.Instance.PushAsync(new NotePopupPage());
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
                    if (value.DiscountType == 0)
                    {
                        order.DiscountType = 0;
                        order.DiscountPercent = value.DiscountAmount;
                        order.TotalDiscount = order.TotalPrice * (value.DiscountAmount * 0.01);
                        lblDiscount.Text = (order.TotalPrice * (value.DiscountAmount * 0.01)).ToString();
                        TotalPriceCalculator();

                    }
                    else
                    {
                        order.DiscountType = Convert.ToInt16(value.DiscountType);
                        order.DiscountPercent = 0;
                        order.TotalDiscount = value.DiscountAmount;
                        lblDiscount.Text = value.DiscountAmount.ToString();
                        TotalPriceCalculator();

                    }


                });

                PopupNavigation.Instance.PushAsync(new DiscountPopupPage(order.TotalPrice.ToString()));
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

                PopupNavigation.Instance.PushAsync(new DeliveryPopupPage());
            }

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

                PopupNavigation.Instance.PushAsync(new ServicePopupPage(order.TotalPrice.ToString()));
            }


        }
        async private void btnSaveInvoice_Tapped(object sender, EventArgs e)
        {
            await SaveInvoice(0);
        }

        async private void btnCreditInvoice_Tapped(object sender, EventArgs e)
        {
            await SaveInvoice(1);
        }

        async Task SaveInvoice(int InvoiceType)
        {
            if (InvoiceType == 0)
            {
                order.PaymentType = 0;
                double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
                if (finalPayment == 0)
                {
                    await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر نمیتواتد در سیستم ثبت گردد", "باشه");
                }
                else
                {
                    await SandoghcheController._connection.InsertAsync(order);
                    await SandoghcheController._connection.InsertAllAsync(order.OrderDetails);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(order);

                    await UpdateProductsAmount();


                    await DisplayAlert("صدور فاکتور", string.Format(" فاکتور {0}  شماره فیش {1} به مبلغ {2} ثبت شد", order.ReceiptNumber, order.OrderId, Convert.ToDouble(lblFinalPayment.Text)), "باشه");

                    await setOrderNumber();
                    await setReceiptNumber();

                    lblTax.Text = "0";
                    lblDiscount.Text = "0";
                    lblService.Text = "0";
                    lblTax.Text = "0";
                    lblFinalPayment.Text = "0";
                    lblTotalPrice.Text = "0";
                    lblDelivery.Text = "0";

                    ProductsDataGrid.ItemsSource = null;
                    lstProducts.ItemsSource = null;
                    await getCategories();
                    order = new Order();
                }//await Navigation.PushAsync(new InvoicePage(Convert.ToInt32(lblClientId.Text), lblClient.Text));
            }
            else if (InvoiceType == 1)
            {
                order.PaymentType = 1;
                double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
                if (finalPayment == 0)
                {
                    await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر نمیتواتد در سیستم ثبت گردد", "باشه");
                }
                else
                {
                    Accounting accounting = new Accounting();
                    accounting.ClientId = order.ClientId;
                    accounting.DebtorAmount = order.FinalPayment;
                    accounting.CreditorAmount = 0;


                    await SandoghcheController._connection.InsertAsync(order);
                    await SandoghcheController._connection.InsertAllAsync(order.OrderDetails);
                    await SandoghcheController._connection.UpdateWithChildrenAsync(order);
                    await SandoghcheController._connection.InsertAsync(accounting);

                    await UpdateProductsAmount();


                    await DisplayAlert("صدور فاکتور", string.Format(" فاکتور {0}  شماره فیش {1} به مبلغ {2} ثبت شد", order.ReceiptNumber, order.OrderId, Convert.ToDouble(lblFinalPayment.Text)), "باشه");

                    await setOrderNumber();
                    await setReceiptNumber();

                    lblTax.Text = "0";
                    lblDiscount.Text = "0";
                    lblService.Text = "0";
                    lblTax.Text = "0";
                    lblFinalPayment.Text = "0";
                    lblTotalPrice.Text = "0";
                    lblDelivery.Text = "0";

                    ProductsDataGrid.ItemsSource = null;
                    lstProducts.ItemsSource = null;
                    await getCategories();
                    await ClientCreditStatus(lblClientId.Text);
                    order = new Order();
                }
            }
        }

        [Obsolete]
        async private void PrintInvoice_Tapped(object sender, EventArgs e)
        {
            order.PaymentType = 0;
            double finalPayment = Convert.ToDouble(lblFinalPayment.Text);
            if (finalPayment == 0)
            {
                await DisplayAlert("صدور فاکتور", "فاکتور به مبلغ صفر نمیتواتد در سیستم ثبت گردد", "باشه");
            }
            else
            {
                await SandoghcheController._connection.InsertAsync(order);
                await SandoghcheController._connection.InsertAllAsync(order.OrderDetails);
                await SandoghcheController._connection.UpdateWithChildrenAsync(order);

                await UpdateProductsAmount();

                if (Device.RuntimePlatform == Device.iOS)
                {
                    //iOS stuff
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    PdfDocument doc = new PdfDocument();
                    doc.PageSettings.Size = new Syncfusion.Drawing.SizeF(300, 800);
                    PdfMargins margins = new PdfMargins();
                    margins.All = 10;
                    doc.PageSettings.Margins = margins;
                    PdfPage page = doc.Pages.Add();

                    PdfGraphics graphics = page.Graphics;

                    //var test = GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
                    Stream fontStream = typeof(EditPage).GetTypeInfo().Assembly.GetManifestResourceStream("Sandoghche.Images.IRANSans(FaNum).ttf");
                    PdfFont font = new PdfTrueTypeFont(fontStream, 10);


                    PdfStringFormat format = new PdfStringFormat();
                    format.TextDirection = PdfTextDirection.RightToLeft;
                    format.Alignment = PdfTextAlignment.Center;

                    PdfStringFormat format2 = new PdfStringFormat();
                    format2.TextDirection = PdfTextDirection.RightToLeft;
                    format2.Alignment = PdfTextAlignment.Right;

                    PdfStringFormat format3 = new PdfStringFormat();
                    format3.TextDirection = PdfTextDirection.RightToLeft;
                    format3.Alignment = PdfTextAlignment.Left;


                    graphics.DrawString(lblClient.Text, font, PdfBrushes.Black, new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height), format);
                    graphics.DrawString("فاکتور فروش", font, PdfBrushes.Black, new RectangleF(0, 20, page.GetClientSize().Width, page.GetClientSize().Height), format);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, 40), new PointF(300, 40));
                    graphics.DrawString("تاریخ :" + SandoghcheController.GetPersianDate(Convert.ToDateTime(order.DateCreated)), font, PdfBrushes.Black, new RectangleF(0, 42, page.GetClientSize().Width, page.GetClientSize().Height), format);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, 60), new PointF(300, 60));
                    graphics.DrawString("ردیف عنوان                                    تعداد   قیمت واحد   قیمت کل          ", font, PdfBrushes.Black, new RectangleF(0, 60, page.GetClientSize().Width, page.GetClientSize().Height), format);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, 75), new PointF(300, 75));


                    string source = "";
                    int length = 5;
                    string result = source.PadRight(length).Substring(0, length);


                    int height = 75;
                    foreach (var item in order.OrderDetails)
                    {
                        string rowNumber = item.RowNumber.ToString().PadLeft(5).Substring(0, 5);
                        string productText = item.ProductText.PadRight(20).Substring(0, 20);
                        string Number = item.Number.ToString().PadLeft(5).Substring(0, 5);
                        string Price = item.Price.ToString().PadLeft(12).Substring(0, 12);
                        string TotalPrice = item.TotalPrice.ToString().PadLeft(12).Substring(0, 12);
                        graphics.DrawString(rowNumber, font, PdfBrushes.Black, 285, height, format2);
                        graphics.DrawString(productText, font, PdfBrushes.Black, 255, height, format2);
                        graphics.DrawString(Number, font, PdfBrushes.Black, 130, height, format2);
                        graphics.DrawString(Price, font, PdfBrushes.Black, 110, height, format2);
                        graphics.DrawString(TotalPrice, font, PdfBrushes.Black, 55, height, format2);
                        height += 15;
                        graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, height), new PointF(300, height));
                        productText = "";
                    }
                    graphics.DrawString("مالیات : " + (order.Tax1 + order.Tax2).ToString(), font, PdfBrushes.Black, new RectangleF(0, height, page.GetClientSize().Width, page.GetClientSize().Height), format3);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, height), new PointF(100, height));
                    height += 15;
                    graphics.DrawString("سرویس : " + order.TotalServiceFee.ToString(), font, PdfBrushes.Black, new RectangleF(0, height, page.GetClientSize().Width, page.GetClientSize().Height), format3);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, height), new PointF(100, height));
                    height += 15;
                    graphics.DrawString("پیک : " + order.DeliveryFee.ToString(), font, PdfBrushes.Black, new RectangleF(0, height, page.GetClientSize().Width, page.GetClientSize().Height), format3);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, height), new PointF(100, height));
                    height += 15;
                    graphics.DrawString‍("تخفیف : " + order.TotalDiscount.ToString(), font, PdfBrushes.Black, new RectangleF(0, height, page.GetClientSize().Width, page.GetClientSize().Height), format3);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, height), new PointF(100, height));
                    height += 15;
                    graphics.DrawString‍("توضیحات : " + order.Comment, font, PdfBrushes.Black, new RectangleF(0, height, page.GetClientSize().Width, page.GetClientSize().Height), format3);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, height), new PointF(100, height));
                    height += 15;
                    graphics.DrawString("جمع کل : " + order.FinalPayment.ToString(), font, PdfBrushes.Black, new RectangleF(0, height, page.GetClientSize().Width, page.GetClientSize().Height), format3);
                    graphics.DrawLine(new PdfPen(PdfBrushes.Black, .5f), new PointF(0, height), new PointF(300, height));


                    MemoryStream stream = new MemoryStream();

                    doc.Save(stream);
                    byte[] test = stream.ToArray();
                    doc.Close(true);


                    DependencyService.Get<IPrintPdf>().PrintPdf(test,order.OrderId);
                }
                else if (Device.RuntimePlatform == Device.UWP)
                {
                    DependencyService.Get<IPrint>().Print(order, "فاکتور فروش", clientName);

                }

                await setOrderNumber();
                await setReceiptNumber();

                lblTax.Text = "0";
                lblDiscount.Text = "0";
                lblService.Text = "0";
                lblTax.Text = "0";
                lblFinalPayment.Text = "0";
                lblTotalPrice.Text = "0";
                lblDelivery.Text = "0";

                ProductsDataGrid.ItemsSource = null;
                lstProducts.ItemsSource = null;
                await getCategories();
                order = new Order();
            }
        }

        async private Task UpdateProductsAmount()
        {
            var products = await SandoghcheController.GetConnection().Table<Product>().Where(p => p.IsDeleted != true).ToListAsync();

            foreach (var item in order.OrderDetails)
            {
                foreach (var product in products)
                {
                    if (product.ProductId == item.ProductId)
                        product.ProductAmount = product.ProductAmount - item.Number;
                }
            }
            await SandoghcheController._connection.UpdateAllAsync(products);
        }

        private void btnMainMenu_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SandoghcheMainPage());

        }


        protected override bool OnBackButtonPressed()
        {
            return true;
        }


    }

}