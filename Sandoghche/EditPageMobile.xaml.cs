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
    public partial class EditPageMobile : ContentPage
    {
        public EditPageMobile()
        {
            InitializeComponent();
        }

        async private void tabViewOrdersList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                var selectedTab = tabView.SelectedItem as TabViewItem;
                if (selectedTab.HeaderText == "TabClientsList")
                {
                    //await GetClients();

                    //ProductsDataGrid.ItemsSource = new List<string>();

                    //ProductsDataGrid.ItemsSource = order.OrderDetails;

                    //await DisplayAlert("test", e.PropertyName, "test");
                }
                if (selectedTab.HeaderText == "TabHome")
                {
                    await Navigation.PushAsync(new SandoghcheMainPage());
                }
            }
        }

        private void btnSearch_Clicked(object sender, EventArgs e)
        {

        }
    }
}