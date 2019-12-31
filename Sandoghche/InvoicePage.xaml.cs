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
    public partial class InvoicePage : ContentPage
    {
        public class Category
        {
            public int CatId { get; set; }
            public string CatText { get; set; }
        }

        public class Item
        {
            public int ItemId { get; set; }

            public string ItemText { get; set; }
        }
        public InvoicePage(int ClientId , string ClientName)
        {
            InitializeComponent();
            lblClient.Text = ClientName;


            lstCategory.ItemsSource = new List<Category>
            {
                new Category{CatId =1,CatText="بستنی"},
                new Category{CatId =2,CatText="فالوده"},
                new Category{CatId =3,CatText="نوشیدنی گرم"},
                new Category{CatId =4,CatText="نوشیدنی سرد"},
                new Category{CatId =5,CatText="شیک"},
                new Category{CatId =6,CatText="کیلویی"},
                new Category{CatId =7,CatText="دسر"}

            };
            
            

            lstItems.ItemsSource = new List<Item>
            { 
                new Item{ItemId = 1,ItemText="لیوانی نارگیلی" },
                new Item{ItemId = 2,ItemText="لیوانی شکلاتی" },
                new Item{ItemId = 3,ItemText="لیوانی نارگیلی" },
                new Item{ItemId = 4,ItemText="لیوانی نسکافه" },
                new Item{ItemId = 5,ItemText="لیوانی آناناس" }


            };

            DataGrid.ItemsSource = new List<Item>
            {
                new Item{ItemId = 1,ItemText="لیوانی شکلاتی لیوانی شکلات" },
                new Item{ItemId = 2,ItemText="لیوانی نارگیلی" },
                new Item{ItemId = 3,ItemText="لیوانی نسکافه" },
                new Item{ItemId = 4,ItemText="لیوانی نارگیلی" },
                new Item{ItemId = 5,ItemText="لیوانی شکلاتی" },
                new Item{ItemId = 6,ItemText="لیوانی نارگیلی" },
                new Item{ItemId = 7,ItemText="لیوانی نسکافه" },
                new Item{ItemId = 8,ItemText="لیوانی آناناس" },
                new Item{ItemId = 9,ItemText="لیوانی آناناس" },
                new Item{ItemId = 10,ItemText="لیوانی آناناس" },
                new Item{ItemId = 11,ItemText="لیوانی آناناس" },
                new Item{ItemId = 12,ItemText="لیوانی آناناس" },



            };
        }

        private void lblClients_Tapped(object sender, EventArgs e)
        {
            var page = new ClientsPage();
            //page.clientDataGrid.SelectedItem += (source, args) =>
            //{
            //    //lblClient.Text = args.Selected
 
            //};
            //page.lstclients.SelectedItem += (source, args) =>
            //{
            //    lblClient.Text = args.SelectedItem.ToString();
            //    Navigation.PopAsync();
            //};
            Navigation.PushAsync(page);
        }
    }
}