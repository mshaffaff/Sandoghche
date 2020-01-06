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

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InvoicePage : ContentPage
    {
        private static OrderViewModel order;

        public class Item
        {
            public int ItemId { get; set; }

            public string ItemText { get; set; }
        }
        public InvoicePage(int ClientId, string ClientName)
        {
            InitializeComponent();
            lblClient.Text = ClientName;
            lblClientId.Text = ClientId.ToString();

            order = new OrderViewModel();
        }

        async protected override void OnAppearing()
        {
            await SandoghcheController.GetConnection().CreateTableAsync<Category>();
            await SandoghcheController.GetConnection().CreateTableAsync<Product>();
            await SandoghcheController.GetConnection().CreateTableAsync<Order>();
            await SandoghcheController.GetConnection().CreateTableAsync<OrderDetail>();
            await getCategories();
            base.OnAppearing();
        }



        //lstItems.ItemsSource = new List<Item>
        //{ 
        //    new Item{ItemId = 1,ItemText="لیوانی نارگیلی" },
        //    new Item{ItemId = 2,ItemText="لیوانی شکلاتی" },
        //    new Item{ItemId = 3,ItemText="لیوانی نارگیلی" },
        //    new Item{ItemId = 4,ItemText="لیوانی نسکافه" },
        //    new Item{ItemId = 5,ItemText="لیوانی آناناس" }


        //};

        //DataGrid.ItemsSource = new List<Item>
        //{
        //    new Item{ItemId = 1,ItemText="لیوانی شکلاتی لیوانی شکلات" },
        //    new Item{ItemId = 2,ItemText="لیوانی نارگیلی" },
        //    new Item{ItemId = 3,ItemText="لیوانی نسکافه" },
        //    new Item{ItemId = 4,ItemText="لیوانی نارگیلی" },
        //    new Item{ItemId = 5,ItemText="لیوانی شکلاتی" },
        //    new Item{ItemId = 6,ItemText="لیوانی نارگیلی" },
        //    new Item{ItemId = 7,ItemText="لیوانی نسکافه" },
        //    new Item{ItemId = 8,ItemText="لیوانی آناناس" },
        //    new Item{ItemId = 9,ItemText="لیوانی آناناس" },
        //    new Item{ItemId = 10,ItemText="لیوانی آناناس" },
        //    new Item{ItemId = 11,ItemText="لیوانی آناناس" },
        //    new Item{ItemId = 12,ItemText="لیوانی آناناس" },



        //};


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
        private void lstProducts_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var product = (Product)e.Item;
                     
            order.Order.ClientId =Convert.ToInt32(lblClientId.Text);
            order.Order.ReceiptNumber = 1;
            order.Order.Tax1 = 1;
            order.Order.Tax2 = 2;
            order.Order.DeliveryFee = 1500;
            order.Order.ServiceType = 1;
            order.Order.TotalServiceFee = 1700;
            order.Order.DiscountType = 2;
            order.Order.DiscountPercent = 20;
            order.Order.TotalDiscount = 1700;
            order.Order.Comment = "Comment";
            order.Order.PaymentType = 1;
            order.Order.TotalPrice = 57400;
            
            var detail = order.OrderDetail.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (detail != null)
            {
                var n = detail.Number + 1;
                detail.Number = n;
                detail.TotalPrice = n * detail.Price;
            }
            else
                order.OrderDetail.Add(new OrderDetailViewModel { ProductId = product.ProductId,ProductText = product.ProductText, Number = 1, Price = product.ProductPrice, CategoryId = product.CategoryId,TotalPrice=product.ProductPrice });

            var t = order;

            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = order.OrderDetail;
           
            
        }
        async private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            await getProducts(Convert.ToInt32(lblCategoryId.Text), e.NewTextValue);
        }


    }
}