using Sandoghche.Components;
using Sandoghche.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.XamarinForms.Input.AutoComplete;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {
        private static int clientId;
        private static int productId;
        private string userRoll;
        public ReportPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

            lblPersianDate.Text = SandoghcheController.GetPersianDate(null);
            lblUser.Text = Application.Current.Properties["userRollName"].ToString() + " : " + Application.Current.Properties["FullName"].ToString();
            pkrReceiptType.Items.Add("روش پرداخت : همه");
            pkrReceiptType.Items.Add("نقدی");
            pkrReceiptType.Items.Add("اعتباری");
            pkrReceiptType.SelectedIndex = 0;



            userRoll = Application.Current.Properties["userRollName"].ToString();
            switch (userRoll)
            {
                case "مدیر ارشد":
                    {
                        isEdited.IsVisible = true;
                        isDeleted.IsVisible = true;
                        lblIsEdited.IsVisible = true;
                        lblIsDeleted.IsVisible = true;

                          
                    }
                    break;
                case "مدیر":
                    {
                        isEdited.IsVisible = true;
                        isDeleted.IsVisible = true;
                        lblIsEdited.IsVisible = true;
                        lblIsDeleted.IsVisible = true;
                    }
                    break;
                case "صندوقدار":
                    {
                        isEdited.IsVisible = false;
                        isDeleted.IsVisible = false;
                        lblIsEdited.IsVisible = false;
                        lblIsDeleted.IsVisible = false;
                    }
                    break;
                case "میزبان":
                    {
                        isEdited.IsVisible = false;
                        isDeleted.IsVisible = false;
                        lblIsEdited.IsVisible = false;
                        lblIsDeleted.IsVisible = false;
                    }
                    break;
                default:
                    break;
            }

        }
        async protected override void OnAppearing()
        {
            await GetClients();
            base.OnAppearing();
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
                     
        async Task GetClients()
        {
            var query = "select  Clients.ClientId,Clients.ClientName from Clients where Clients.IsDeleted!=1 AND Clients.IsActive=1";
            var clients = await SandoghcheController.GetConnection().QueryAsync<Client>(query);
            srchClients.ItemsSource = clients.ToList();
            srchClients.Filter = new ClientAutoCompleteViewFilter();
        }


        public class CustomOrderDetails
        {
            public int RowNumber { get; set; }
            public string ProductText { get; set; }
            public int Number { get; set; }
            public double TotalPrice { get; set; }
        }

        public class ReceivedCredit
        {
            public double receivedCreditAmount { get; set; }
        }

        async private void btnSearch_Clicked(object sender, EventArgs e)
        {
            DateTime? startDate = srchStartDate.SelectedDateTime;
            DateTime? endDate = srchEndDate.SelectedDateTime;
            int receiptType = pkrReceiptType.SelectedIndex;


            if (startDate == null)
            {
                await DisplayAlert("خطا", "تاریخ نامشخص است", "باشه");
                return;
            }


            if (endDate == null)
                endDate = startDate;




            string condition = string.Concat(" date(Orders.DateCreated) >=  date('", startDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), "') AND date(Orders.DateCreated) <= date('", endDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), "')");

            if (!String.IsNullOrWhiteSpace(srchClients.Text))
                condition += (" and Orders.ClientId = " + clientId);

            if (isEdited.IsChecked)
                condition += (" and Orders.isEdited = " + 1);

            if (isDeleted.IsChecked || userRoll == "مدیر ارشد" || userRoll == "مدیر")
                condition += (" or Orders.isDeleted = " + 1);
            else
                condition += (" and Orders.isDeleted = " + 0);
                      
           
            if (pkrReceiptType.SelectedIndex == 2)
                condition += (" and Orders.PaymentType = " + 1);
            else if (pkrReceiptType.SelectedIndex == 1)
                condition += (" and Orders.PaymentType = " + 0);


            var query = String.Concat("SELECT Sum(TotalPrice) as TotalPrice,Sum(TotalServiceFee) as TotalServiceFee,Sum(DeliveryFee) as DeliveryFee ,Sum(TotalDiscount) as TotalDiscount , Sum(FinalPayment) as FinalPayment, Sum(Tax1) as Tax1 , Sum(Tax2) as Tax2 from Orders where " + condition);

            var query2 = String.Concat("SELECT ROW_NUMBER() OVER(ORDER BY DetailId) AS RowNumber ,Products.ProductText, sum(OrderDetails.number) as Number,sum(OrderDetails.TotalPrice) as TotalPrice from OrderDetails left join Products on OrderDetails.ProductId=Products.ProductId where OrderDetails.OrderId in  (SELECT OrderId from Orders where ", condition, ") GROUP by OrderDetails.ProductId");
            var Orders = await SandoghcheController.GetConnection().QueryAsync<Order>(query);

            lblTotalPrice.Text = Orders.FirstOrDefault().TotalPrice.ToString();
            lblService.Text = Orders.FirstOrDefault().TotalServiceFee.ToString();
            lblDiscount.Text = Orders.FirstOrDefault().TotalDiscount.ToString();
            lblDelivery.Text = Orders.FirstOrDefault().DeliveryFee.ToString();
            lblTax.Text = (Orders.FirstOrDefault().Tax1 + Orders.FirstOrDefault().Tax2).ToString();

            var receivedCreditQuery = "select sum(CreditorAmount) as 'receivedCreditAmount' from Accounting WHERE date(DateCreated) BETWEEN  date('" + startDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "') AND date('"+endDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "')";



            var receivedCredit = await SandoghcheController.GetConnection().QueryAsync<ReceivedCredit>(receivedCreditQuery);

            lblReceivedCredit.Text = receivedCredit[0].receivedCreditAmount.ToString();


            lblFinalPayment.Text =(Convert.ToDouble(lblReceivedCredit.Text) +  Orders.FirstOrDefault().FinalPayment).ToString();


            var OrderDetails = await SandoghcheController.GetConnection().QueryAsync<CustomOrderDetails>(query2);

            ProductsDataGrid.ItemsSource = OrderDetails;



        }

        private void srchClients_SuggestionItemSelected(object sender, SuggestionItemSelectedEventArgs e)
        {
            var client = (Client)e.DataItem;
            clientId = client.ClientId;

        }

        private void srchProdcuts_SuggestionItemSelected(object sender, SuggestionItemSelectedEventArgs e)
        {
            var product = (Product)e.DataItem;
            productId = product.ProductId;
        }


        private void btnHome_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SandoghcheMainPage());
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}