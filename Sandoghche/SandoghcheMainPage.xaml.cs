using Rg.Plugins.Popup.Services;
using Sandoghche.Components;
using Sandoghche.Models;
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
    public partial class SandoghcheMainPage : ContentPage
    {
        public SandoghcheMainPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;

            lblPersianYear.Text = pc.GetYear(thisDate).ToString();
            lblPersianDay.Text = pc.GetDayOfMonth(thisDate).ToString();

            switch (pc.GetMonth(thisDate))
            {
                case 1:
                    lblPersianMonth.Text = "فروردین";
                    break;
                case 2:
                    lblPersianMonth.Text = "اردیبهشت";
                    break;
                case 3:
                    lblPersianMonth.Text = "خرداد";
                    break;
                case 4:
                    lblPersianMonth.Text = "تیر";
                    break;
                case 5:
                    lblPersianMonth.Text = "مرداد";
                    break;
                case 6:
                    lblPersianMonth.Text = "شهریور";
                    break;
                case 7:
                    lblPersianMonth.Text = "مهر";
                    break;
                case 8:
                    lblPersianMonth.Text = "آبان";
                    break;
                case 9:
                    lblPersianMonth.Text = "آذر";
                    break;
                case 10:
                    lblPersianMonth.Text = "دی";
                    break;
                case 11:
                    lblPersianMonth.Text = "بهمن";
                    break;
                case 12:
                    lblPersianMonth.Text = "اسفند";
                    break;
            }




            backgroundImage.Source = ImageSource.FromResource("Sandoghche.Images.background.jpg");
            sloganLine.Source = ImageSource.FromResource("Sandoghche.Images.sloganLine.png");
            Location.Source = ImageSource.FromResource("Sandoghche.Images.Location.png");

            imgEdit.Source = ImageSource.FromResource("Sandoghche.Images.Edit.png");
            imgInvoice.Source = ImageSource.FromResource("Sandoghche.Images.Invoice.png");
            imgReports.Source = ImageSource.FromResource("Sandoghche.Images.reports.png");
            imgItems.Source = ImageSource.FromResource("Sandoghche.Images.items.png");
            imgSettings.Source = ImageSource.FromResource("Sandoghche.Images.Settings.png");
            imgClients.Source = ImageSource.FromResource("Sandoghche.Images.Clients.png");

            if (Application.Current.Properties.ContainsKey("FullName"))
            {
                lblUser.Text = Application.Current.Properties["FullName"].ToString();
                lblRollName.Text = Application.Current.Properties["userRollName"].ToString() + " : " ;

            }

        }

        async protected override void OnAppearing()
        {
            //await SandoghcheController.GetConnection().CreateTableAsync<Category>();
            //await SandoghcheController.GetConnection().CreateTableAsync<Client>();
            //await SandoghcheController.GetConnection().CreateTableAsync<Product>();
            //await SandoghcheController.GetConnection().CreateTableAsync<Order>();
            //await SandoghcheController.GetConnection().CreateTableAsync<OrderDetail>();
            //await SandoghcheController.GetConnection().CreateTableAsync<SandoghcheSetting>();
            //await SandoghcheController.GetConnection().CreateTableAsync<Accounting>();

            base.OnAppearing();
        }
        async private void imgEdit_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditPage());
            //await PopupNavigation.Instance.PushAsync(new EditPopupPage());
        }
        async private void imgInvoice_Tapped(object sender, EventArgs e)
        {
            var firstClient = await SandoghcheController.GetConnection().Table<Client>().FirstOrDefaultAsync();

            if (firstClient != null)
                await Navigation.PushAsync(new InvoicePage(firstClient.ClientId, firstClient.ClientName));
            else
                await Navigation.PushAsync(new InvoicePage(0, "انتخاب مشتری"));
        }
        async private void imgReports_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReportPage());
        }
        async private void imgItems_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ItemsPage());
        }
        async private void imgSettings_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
        async private void imgClients_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ClientsPage());
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }

}