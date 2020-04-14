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
    public partial class ItemPageMobile : ContentPage
    {
        public ItemPageMobile()
        {
            InitializeComponent();
        }

         async private void tabView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                var selectedTab = tabView.SelectedItem as TabViewItem;
                if (selectedTab.HeaderText == "TabProduct")
                {
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

        private void pkrCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCancelItem_Clicked(object sender, EventArgs e)
        {

        }

        private void btnDeleteItem_Clicked(object sender, EventArgs e)
        {

        }

        private void btnUpdateItem_Clicked(object sender, EventArgs e)
        {

        }

        private void btnAddItem_Clicked(object sender, EventArgs e)
        {

        }

        private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ProductDataGrid_SelectionChanged(object sender, Telerik.XamarinForms.DataGrid.DataGridSelectionChangedEventArgs e)
        {

        }
    }
}