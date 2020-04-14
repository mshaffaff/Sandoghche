using Sandoghche.Components;
using Sandoghche.Models;
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
            var category = (Category)pkrCategory.SelectedItem;
            if (category != null)
                categoryId = category.CategoryId;
            //   await getProducts(categoryId);


            //DisplayAlert("test", pkrCategory.SelectedIndex.ToString(), "ok");
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

        private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {

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
            btnDeleteItem.IsVisible = false;
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
                btnDeleteItem.IsVisible = false;
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
            btnDeleteItem.IsVisible = false;
            // frmItems.IsEnabled = false;
            //frmItems.Opacity = 0.3;


            await getCategories(e.NewTextValue);
        }

        async private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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
                    btnDeleteItem.IsVisible = false;
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
                    btnDeleteItem.IsVisible = false;
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
    }
}