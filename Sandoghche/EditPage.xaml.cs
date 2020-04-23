using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Sandoghche.Components;
using Sandoghche.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Syncfusion.Pdf.Grid;
using System.Collections.Generic;
using System.Data;
using Syncfusion.Pdf.Tables;
using Syncfusion.SfDataGrid.XForms.Exporting;
using System.Reflection;
using Syncfusion.Pdf.Lists;
using Xamarin.Essentials;
using Sandoghche.ViewModels;

namespace Sandoghche

{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPage : ContentPage
    {
        private string userRoll;

        public EditPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

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
            var query = "SELECT Orders.OrderId,Orders.ReceiptNumber,Orders.FinalPayment,date(Orders.DateCreated),Orders.isDeleted,Orders.isEdited,Clients.ClientName from Orders LEFT  JOIN Clients ON Orders.ClientId = Clients.ClientId where 1 ";

            if (createdDate != null)
                query += string.Concat("and (date(Orders.DateCreated) = date('", createdDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), "'))");

            //if (createdDate == null)
            //    query += query.Substring(0, query.Length - 5);
            ////str = str.Substring(0, str.Length - 2);

            if (orderId != 0)
                query += "  and Orders.OrderId=" + orderId;
            


            if (receiptId != 0)
                query += "  and Orders.ReceiptNumber=" + receiptId;


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
            var s = sender as Xamarin.Forms.Button;
            var selectedItem = s.BindingContext;
            var orderModel = (OrderDetailForSearchViewModel)selectedItem;
            var order = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == orderModel.OrderId);
            order.OrderDetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(od => od.OrderId == orderModel.OrderId).ToListAsync();
            var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync();
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == order.ClientId);
            foreach (var item in order.OrderDetails)
            {
                item.ProductText = products.FirstOrDefault(p => p.ProductId == item.ProductId).ProductText;
            }


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


                graphics.DrawString(client.ClientName, font, PdfBrushes.Black, new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height), format);
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
                //DependencyService.Get<IPrint>().Print(order, "فاکتور فروش", clientName);
                DependencyService.Get<IPrint>().Print(order, "چاپ مجدد", client.ClientName);

            }




        }

        async private void btnEdit_Clicked(object sender, EventArgs e)
        {
            var s = sender as Xamarin.Forms.Button;
            var selectedItem = s.BindingContext;
            var order = (OrderDetailForSearchViewModel)selectedItem;
            // await DisplayAlert("test",order.OrderId.ToString(),"test");
            await Navigation.PushAsync(new EditOrderPage(order.OrderId));
        }

        private void btnOrderHistory(object sender, EventArgs e)
        {
            var s = sender as Xamarin.Forms.Label;
            var selectedOrder = s.BindingContext;
            var order = (OrderDetailForSearchViewModel)selectedOrder;
            if (order.isEdited || order.isDeleted)
                Navigation.PushAsync(new OrderHistoryPage(order.OrderId));

        }
        private void btnHome_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SandoghcheMainPage());
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        public class orderDetailViewModel
        {
            public int RowNumber { get; set; }
            public string ProductText { get; set; }
            public int Number { get; set; }
            public double Price { get; set; }
            public double TotalPrice { get; set; }
        }

        async private void btnShare_Clicked(object sender, EventArgs e)
        {
            var s = sender as Xamarin.Forms.Button;
            var selectedOrder = s.BindingContext;
            var orderModel = (OrderDetailForSearchViewModel)selectedOrder;
            int orderId = orderModel.OrderId;

            var order = await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(o => o.OrderId == orderId);
            order.OrderDetails = await SandoghcheController.GetConnection().Table<OrderDetail>().Where(od => od.OrderId == orderModel.OrderId).ToListAsync();
            var products = await SandoghcheController.GetConnection().Table<Product>().ToListAsync();
            var client = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync(c => c.ClientId == order.ClientId);
            int rownum = 0;
            foreach (var item in order.OrderDetails)
            {
                rownum++;
                item.ProductText = products.FirstOrDefault(p => p.ProductId == item.ProductId).ProductText;
                item.RowNumber = rownum;
            }

            List<orderDetailViewModel> orderDetails = new List<orderDetailViewModel>();
            foreach (var item in order.OrderDetails)
            {
                orderDetails.Add(new orderDetailViewModel { RowNumber = item.RowNumber, ProductText = item.ProductText, Number = item.Number, Price = item.Price, TotalPrice = item.TotalPrice });
            }


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


            graphics.DrawString(client.ClientName, font, PdfBrushes.Black, new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height), format);
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


            var fn = order.OrderId.ToString() + ".pdf";
            var file = Path.Combine(FileSystem.CacheDirectory, fn);

            File.WriteAllBytes(file, test);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "فاکتور فروش",
                File = new ShareFile(file)
            });


            // await Xamarin.Forms.DependencyService.Get<ISave>().Save(order.OrderId.ToString(), "application/pdf", stream);





        }






    }

}