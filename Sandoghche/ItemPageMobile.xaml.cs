using Sandoghche.Components;
using Sandoghche.Models;
using Sandoghche.ViewModels;
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
        private static int categoryId;
        private static string SortBy;
        private static int SortingCounter = 0;
        private int ProductId;



        public ItemPageMobile()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            await getCategories();
            base.OnAppearing();
        }
        async Task getCategories(string Searchtext = null)
        {
            var categories = await SandoghcheController.GetConnection().Table<Category>().Where(c => c.IsDeleted != true).ToListAsync();

            pkrCategory.ItemsSource = categories;

            var result = new List<Category>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = categories;
            else
                result = categories.Where(c => c.CategoryText.Contains(Searchtext) && c.IsDeleted != true).ToList();

            lstCategory.ItemsSource = result;
        }

        async Task getProducts(string Searchtext = null, string SortBy = null)
        {
            //var products = await SandoghcheController._connection.Table<Product>().Where(p => p.IsDeleted != true).ToListAsync();

            var query = "select Products.ProductId,Products.ProductText,Products.ProductPrice,Products.isDeleted,Products.IsActive,Products.ProductAmount,Categories.CategoryText from Products LEFT JOIN Categories on Products.CategoryId = Categories.CategoryId WHERE Products.isDeleted !=1";

            switch (SortBy)
            {
                case "ProductASC":
                    query += " ORDER BY Products.ProductText ASC";
                    break;
                case "ProductDESC":
                    query += " ORDER BY Products.ProductText DESC";
                    break;
                case "CategoryASC":
                    query += " ORDER BY CategoryText ASC";
                    break;
                case "CategoryDESC":
                    query += " ORDER BY CategoryText DESC";
                    break;
                case "AmountASC":
                    query += " ORDER BY Products.ProductAmount ASC";
                    break;
                case "AmountDESC":
                    query += " ORDER BY Products.ProductAmount DESC";
                    break;
                case "PriceASC":
                    query += " ORDER BY Products.ProductPrice ASC";
                    break;
                case "PriceDESC":
                    query += " ORDER BY Products.ProductPrice DESC";
                    break;
                default:
                    query += " ";
                    break;
            }

            var products = await SandoghcheController.GetConnection().QueryAsync<ProductCategoryPriceViewModel>(query);

            var result = new List<ProductCategoryPriceViewModel>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = products.ToList();
            else
                result = products.Where(p => p.ProductText.Contains(Searchtext) && p.isDeleted != true).ToList();

            ProductslistView.ItemsSource = result;
        }
        async private void tabView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                var selectedTab = tabView.SelectedItem as TabViewItem;
                if (selectedTab.HeaderText == "TabListOfProducts")
                {
                    await getProducts();
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
            var category = (Category)pkrCategory.SelectedItem;
            if (category != null)
                categoryId = category.CategoryId;
            //   await getProducts(categoryId);


            //DisplayAlert("test", pkrCategory.SelectedIndex.ToString(), "ok");
        }

        private void btnCancelItem_Clicked(object sender, EventArgs e)
        {
            txtItem.Text = "";
            txtProductPrice.Value = 0;
            txtProductAmount.Value = 0;
            swchItemStatus.IsToggled = true;
            btnAddItem.IsVisible = true;
            btnUpdateItem.IsVisible = false;
            //btnDeleteItem.IsVisible = false;
            btnCancelItem.IsVisible = false;
            srchProduct.Text = "";
            pkrCategory.SelectedIndex = -1;
            TabListOfProducts.IsSelected = true;
        }
        //private void btnDeleteItem_Clicked(object sender, EventArgs e)
        //{

        //}

        async private void btnUpdateItem_Clicked(object sender, EventArgs e)
        {
            var product = await SandoghcheController.GetConnection().Table<Product>().FirstOrDefaultAsync(p => p.ProductId == ProductId);

            if (String.IsNullOrWhiteSpace(txtItem.Text) ||
                Convert.ToDouble(txtProductAmount.Value) < 0 || Convert.ToDouble(txtProductPrice.Value) <= 0)
                await DisplayAlert("خطا", "ورود مقادیر اشتباه است", "باشه");
            else if (pkrCategory.SelectedIndex == -1)
            {
                await DisplayAlert("خطا", " طبقه را مشخص کنید ", "باشه");
            }
            else
            {
                product.CategoryId = categoryId;
                product.ProductText = txtItem.Text;
                product.ProductPrice = Convert.ToDouble(txtProductPrice.Value);
                product.ProductAmount = Convert.ToDouble(txtProductAmount.Value);
                product.IsActive = swchItemStatus.IsToggled;

                await SandoghcheController._connection.UpdateAsync(product);

                txtItem.Text = "";
                txtProductPrice.Value = 0;
                txtProductAmount.Value = 0;
                srchProduct.Text = "";
                btnAddItem.IsVisible = true;
                btnCancelItem.IsVisible = false;
                btnUpdateItem.IsVisible = false;
                pkrCategory.SelectedIndex = -1;
                await getProducts();
                TabListOfProducts.IsSelected = true;
                await DisplayAlert("ویرایش ", "به روز رسانی با موفقیت انجام شد", "باشه");

            }
        }

        async private void btnAddItem_Clicked(object sender, EventArgs e)
        {
            if (
                String.IsNullOrWhiteSpace(txtItem.Text) ||
                Convert.ToDouble(txtProductAmount.Value) < 0 || Convert.ToDouble(txtProductPrice.Value) <= 0)
                await DisplayAlert("خطا", "ورود مقادیر اشتباه است", "باشه");
            else if (pkrCategory.SelectedIndex == -1)
            {
                await DisplayAlert("خطا", " طبقه را مشخص کنید ", "باشه");
            }
            else
            {
                var product = new Product();
                product.ProductText = txtItem.Text;
                product.ProductPrice = Convert.ToDouble(txtProductPrice.Value);
                product.ProductAmount = Convert.ToDouble(txtProductAmount.Value);
                product.IsActive = swchItemStatus.IsToggled;
                product.CategoryId = categoryId;

                await SandoghcheController._connection.InsertAsync(product);

                await DisplayAlert("ثبت ", "محصول جدید در سیستم با موفقیت ثبت گردید", "باشه");

                txtItem.Text = "";
                txtProductPrice.Value = 0;
                txtProductAmount.Value = 0;
                swchItemStatus.IsToggled = true;

                //await getProducts(product.CategoryId);



            }

        }

        async private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            await getProducts(e.NewTextValue);
        }

        private void ProductDataGrid_SelectionChanged(object sender, Telerik.XamarinForms.DataGrid.DataGridSelectionChangedEventArgs e)
        {

        }

        async private void btnCancelCategory_Clicked(object sender, EventArgs e)
        {
            txtCategory.Text = "";
            swchCategoryStatus.IsToggled = true;
            btnDeleteCategory.IsVisible = false;
            btnCancelCategory.IsVisible = false;
            btnUpdateCategory.IsVisible = false;
            btnAddCategory.IsVisible = true;

            btnAddItem.IsVisible = true;
            btnUpdateItem.IsVisible = false;
            btnCancelItem.IsVisible = false;
            //btnDeleteItem.IsVisible = false;
            srchCategory.Text = "";
            //txtItem.Text = "";
            //txtProductPrice.Text = "";
            //srchCategory.Text = "";
            //frmItems.IsEnabled = false;
            //frmItems.Opacity = .3;
            // lblCategoryText.Text = "انتخاب نشده";
            //lblCategoryId.Text = "";
            //DataGridProduct.ItemsSource = null;


            await getCategories();
        }

        async private void btnDeleteCategory_Clicked(object sender, EventArgs e)
        {
            var category = (Category)lstCategory.SelectedItem;

            var products = await SandoghcheController._connection.Table<Product>().Where(p => p.CategoryId == category.CategoryId).ToListAsync();

            bool action;
            if (products.Count() > 0)
            {
                action = await DisplayAlert("اخطار", string.Format("این طبقه بندی شامل {0} محصول می باشد و با حذف آن محصولات زیرمجموعه نیز حذف میگردد.  آیا از حذف این طبقه اطمینان دارید؟", products.Count()), "بله", "خیر");
            }
            else
            {
                action = await DisplayAlert("اخطار", string.Format(" آیا از حذف این طبقه اطمینان دارید؟"), "بله", "خیر");
            }

            if (action)
            {
                category.IsDeleted = true;

                if (products.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        product.IsDeleted = true;
                    }
                    await SandoghcheController._connection.UpdateAllAsync(products);
                    // DataGridProduct.ItemsSource = null;

                }


                await SandoghcheController._connection.UpdateAsync(category);

                //frmItems.IsEnabled = false;
                //frmItems.Opacity = 0.3;
                //lblCategoryText.Text = "";
                //lblCategoryId.Text = "";


                txtCategory.Text = "";
                swchCategoryStatus.IsToggled = true;
                btnAddCategory.IsVisible = true;
                btnUpdateCategory.IsVisible = false;
                btnDeleteCategory.IsVisible = false;
                btnCancelCategory.IsVisible = false;
                srchCategory.Text = "";

                await getCategories();

            }

        }

        async private void btnUpdateCategory_Clicked(object sender, EventArgs e)
        {
            var category = (Category)lstCategory.SelectedItem;

            var products = await SandoghcheController._connection.Table<Product>().Where(c => c.CategoryId == category.CategoryId).ToListAsync();


            if (String.IsNullOrWhiteSpace(txtCategory.Text))
                await DisplayAlert("خطا", "طبقه بندی نمی تواند خالی باشد", "باشه");
            else
            {
                category.CategoryText = txtCategory.Text;
                category.IsActive = swchCategoryStatus.IsToggled;
                foreach (var item in products)
                {
                    item.IsActive = swchCategoryStatus.IsToggled;
                }




                await SandoghcheController._connection.UpdateAsync(category);
                await SandoghcheController._connection.UpdateAllAsync(products);
                //DataGridProduct.ItemsSource = null;
                ///lblCategoryId.Text = "";
                //lblCategoryText.Text = "انتخاب نشده";
                // frmItems.IsEnabled = false;
                //frmItems.Opacity = 0.3;


                await DisplayAlert("ثبت", "بروز رسانی با موفقیت انجام شد", "باشه");



                txtCategory.Text = "";
                swchCategoryStatus.IsToggled = true;
                btnAddCategory.IsVisible = true;
                btnUpdateCategory.IsVisible = false;
                btnCancelCategory.IsVisible = false;
                btnDeleteCategory.IsVisible = false;

                btnAddItem.IsVisible = true;
                //btnDeleteItem.IsVisible = false;
                btnUpdateItem.IsVisible = false;
                btnCancelItem.IsVisible = false;


                srchCategory.Text = "";

                //if (!category.IsActive)
                //{
                //    frmItems.IsEnabled = false;
                //    frmItems.Opacity = 0.3;
                //}else
                //{
                //    frmItems.IsEnabled = true;
                //    frmItems.Opacity = 1;
                //}

                await getCategories();
                //await getProducts(category.CategoryId);
            }
        }

        async private void btnAddCategory_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtCategory.Text))
                await DisplayAlert("خطا", "فیلد طبقه بندی خالی است", "باشه");
            else
            {
                var category = new Category();
                category.CategoryText = txtCategory.Text;
                category.IsActive = swchCategoryStatus.IsToggled;


                await SandoghcheController._connection.InsertAsync(category);
                await DisplayAlert("ثبت ", "طبقه بندی جدید در سیستم با موفقیت ثبت گردید", "باشه");

                txtCategory.Text = string.Empty;
                srchCategory.Text = string.Empty;

                await getCategories();
            }
        }

        async private void srchCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            //lblCategoryId.Text = null;
            //lblCategoryText.Text = "انتخاب نشده";
            //txtItem.Text = "";
            //txtProductPrice.Text = "";
            //srchProduct.Text = "";
            //btnAddItem.IsVisible = true;
            //btnCancelItem.IsVisible = false;
            btnUpdateItem.IsVisible = false;
            //btnDeleteItem.IsVisible = false;
            // frmItems.IsEnabled = false;
            //frmItems.Opacity = 0.3;


            await getCategories(e.NewTextValue);
        }

        private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var category = (Category)e.SelectedItem;
            if (category != null)
            {
                if (category.IsActive)
                {
                    txtCategory.Text = category.CategoryText;
                    swchCategoryStatus.IsToggled = category.IsActive;
                    btnAddCategory.IsVisible = false;
                    btnUpdateCategory.IsVisible = true;
                    btnCancelCategory.IsVisible = true;
                    btnDeleteCategory.IsVisible = true;
                    btnAddItem.IsVisible = true;
                    btnUpdateItem.IsVisible = false;
                    btnCancelItem.IsVisible = false;
                    //btnDeleteItem.IsVisible = false;
                    //frmItems.IsEnabled = true;
                    //frmItems.Opacity = 1;
                    txtItem.Text = "";
                    //txtProductPrice.Text = "";
                    swchItemStatus.IsToggled = true;
                    // lblCategoryId.Text = category.CategoryId.ToString();
                    // lblCategoryText.Text = category.CategoryText;


                    // var categoryId = Convert.ToInt32(lblCategoryId.Text);

                    //await getProducts(categoryId);

                }
                else
                {
                    txtCategory.Text = category.CategoryText;
                    swchCategoryStatus.IsToggled = category.IsActive;
                    btnAddCategory.IsVisible = false;
                    btnUpdateCategory.IsVisible = true;
                    btnCancelCategory.IsVisible = true;
                    btnDeleteCategory.IsVisible = true;
                    btnAddItem.IsVisible = true;
                    btnUpdateItem.IsVisible = false;
                    btnCancelItem.IsVisible = false;
                    //btnDeleteItem.IsVisible = false;
                    // frmItems.IsEnabled = false;
                    // frmItems.Opacity = 0.3;
                    // txtItem.Text = "";
                    //  txtProductPrice.Text = "";
                    //  lblCategoryId.Text = category.CategoryId.ToString();
                    //  lblCategoryText.Text = category.CategoryText;


                    // var categoryId = Convert.ToInt32(lblCategoryId.Text);

                    // await getProducts(categoryId);
                }
            }
        }

        async private void SortByAmount_Tapped(object sender, EventArgs e)
        {
            SortingCounter++;

            if (SortingCounter % 2 == 0)
            {
                SortBy = "AmountASC";
                await getProducts(null, SortBy);
            }
            else
            {
                SortBy = "AmountDESC";
                await getProducts(null, SortBy);
            }
        }

        async private void SortByPrice_Tapped(object sender, EventArgs e)
        {
            SortingCounter++;

            if (SortingCounter % 2 == 0)
            {
                SortBy = "PriceASC";
                await getProducts(null, SortBy);
            }
            else
            {
                SortBy = "PriceDESC";
                await getProducts(null, SortBy);
            }
        }

        async private void SortByCategory_Tapped(object sender, EventArgs e)
        {
            SortingCounter++;

            if (SortingCounter % 2 == 0)
            {
                SortBy = "CategoryASC";
                await getProducts(null, SortBy);
            }
            else
            {
                SortBy = "CategoryDESC";
                await getProducts(null, SortBy);
            }

        }
        async private void SortByProductText_Tapped(object sender, EventArgs e)
        {
            SortingCounter++;

            if (SortingCounter % 2 == 0)
            {
                SortBy = "ProductASC";
                await getProducts(null, SortBy);
            }
            else
            {
                SortBy = "ProductDESC";
                await getProducts(null, SortBy);
            }

        }

        async private void btnDeleteItem_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var productViewModel = button.CommandParameter as ProductCategoryPriceViewModel;
            var product = await SandoghcheController.GetConnection().Table<Product>().FirstOrDefaultAsync(p => p.ProductId == productViewModel.ProductId);

            bool action = await DisplayAlert("اخطار", "آیا از حذف این محصول اطمینان دارید ؟", "بله", "خیر");
            if (action)
            {
                product.IsDeleted = true;
                await SandoghcheController._connection.UpdateAsync(product);

                await getProducts();

                //swchItemStatus.IsToggled = true;
                //btnAddItem.IsVisible = true;
                //btnCancelItem.IsVisible = false;
                // btnUpdateItem.IsVisible = false;
                //btnDeleteItem.IsVisible = false;
                //txtItem.Text = "";
                //txtProductPrice.Text = "";
                // txtProductAmount.Text = "0";
                //srchProduct.Text = "";
            }
        }
      
        async private void btnEditItem_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var productViewModel = button.CommandParameter as ProductCategoryPriceViewModel;
            var product = await SandoghcheController.GetConnection().Table<Product>().FirstOrDefaultAsync(p => p.ProductId == productViewModel.ProductId);
            ProductId = product.ProductId;
            var category = await SandoghcheController.GetConnection().Table<Category>().FirstOrDefaultAsync(c => c.CategoryId == product.CategoryId);

            TabProduct.IsSelected = true;

            int index = 0;
            foreach (var item in pkrCategory.Items)
            {
                if (item == category.CategoryText)
                {
                    break;
                }
                index++;
            }
            pkrCategory.SelectedIndex = index;

            txtItem.Text = product.ProductText;
            txtProductAmount.Value = product.ProductAmount;
            txtProductPrice.Value = product.ProductPrice;
            swchItemStatus.IsToggled = product.IsActive;

            btnAddItem.IsVisible = false;
            btnUpdateItem.IsVisible = true;
            btnCancelItem.IsVisible = true;

        }


    }
}