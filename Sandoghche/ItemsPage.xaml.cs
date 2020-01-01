using Sandoghche.Components;
using Sandoghche.Models;
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
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            await SandoghcheController.GetConnection().CreateTableAsync<Category>();
            await SandoghcheController.GetConnection().CreateTableAsync<Product>();
            await getCategories();
            base.OnAppearing();
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
                
                txtCategory.Text = null;
                srchCategory.Text = "";

                await getCategories();
            }



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
            frmItems.IsEnabled = false;
            frmItems.Opacity = .3;
            lblCategoryText.Text = "انتخاب نشده";
            lblCategoryId.Text = "";
            DataGridProduct.ItemsSource = null;

                                 
            await getCategories();


        }

        async private void btnDeleteCategory_Clicked(object sender, EventArgs e)
        {
            var category = (Category)lstCategory.SelectedItem;

            var products = await SandoghcheController._connection.Table<Product>().Where(p=> p.CategoryId == category.CategoryId).ToListAsync();

            bool action;
            if (products.Count() > 0)
            {
                action = await DisplayAlert("اخطار", string.Format("این طبقه بندی شامل {0} محصول می باشد و با حذف آن محصولات زیرمجموعه نیز حذف میگردد.  آیا از حذف این طبقه اطمینان دارید؟", products.Count()), "بله", "خیر");
            }
            else
            {
                action = await DisplayAlert("اخطار", string.Format(" آیا از حذف این طبقه اطمینان دارید؟"), "بله", "خیر");
            } 
                                 
            if(action)
            {               
                category.IsDeleted = true;
                
                if(products.Count()>0)
                {
                    foreach (var product in products)
                    {
                        product.IsDeleted = true;
                    }
                    await SandoghcheController._connection.UpdateAllAsync(products);
                    DataGridProduct.ItemsSource = null;

                }


                await SandoghcheController._connection.UpdateAsync(category);

                frmItems.IsEnabled = false;
                frmItems.Opacity = 0.3;
                lblCategoryText.Text = "";
                lblCategoryId.Text = "";


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

                if (!category.IsActive)
                {
                    frmItems.IsEnabled = false;
                    frmItems.Opacity = 0.3;
                }else
                {
                    frmItems.IsEnabled = true;
                    frmItems.Opacity = 1;
                }

                await getCategories();
                await getProducts(category.CategoryId);
            }

        }

        async Task getProducts(int categoryId, string Searchtext = null)
        {
            
            var products = await SandoghcheController._connection.Table<Product>().Where(p => p.IsDeleted != true && p.CategoryId==categoryId).ToListAsync();

            var result = new List<Product>();
            if (String.IsNullOrWhiteSpace(Searchtext))
                result = products;
            else
                result = products.Where(p => p.ProductText.Contains(Searchtext) && p.IsDeleted != true && p.CategoryId == categoryId).ToList();

            DataGridProduct.ItemsSource = result;
        }



        async private void btnAddItem_Clicked(object sender, EventArgs e)
        {
            double num;
            if (String.IsNullOrWhiteSpace(txtItem.Text) || String.IsNullOrWhiteSpace(txtProductPrice.Text) || !double.TryParse(txtProductPrice.Text,out num) )
                await DisplayAlert("خطا", " نام محصول یا قیمت خالی است", "باشه");
            else if (String.IsNullOrWhiteSpace(lblCategoryId.Text))
            {
                await DisplayAlert("خطا", " طبقه را مشخص کنید ", "باشه");
            }
            else
            {
                var product = new Product();
                product.ProductText = txtItem.Text;
                product.ProductPrice = Convert.ToDouble(txtProductPrice.Text);
                product.IsActive = swchItemStatus.IsToggled;
                product.CategoryId = Convert.ToInt32(lblCategoryId.Text);

                await SandoghcheController._connection.InsertAsync(product);
                await DisplayAlert("ثبت ", "محصول جدید در سیستم با موفقیت ثبت گردید", "باشه");

                txtItem.Text = "";
                swchItemStatus.IsToggled = true;

                await getProducts(product.CategoryId);



            }


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

        async private void srchCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            await getCategories(e.NewTextValue);
        }




        async private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var category = (Category)e.SelectedItem;
            if(category.IsActive)
            {
             txtCategory.Text = category.CategoryText;
             swchCategoryStatus.IsToggled = category.IsActive;
             btnAddCategory.IsVisible = false;
             btnUpdateCategory.IsVisible = true;
             btnCancelCategory.IsVisible = true;
             btnDeleteCategory.IsVisible = true;
             frmItems.IsEnabled = true;
             frmItems.Opacity = 1;
             txtItem.Text = "";
             txtProductPrice.Text = "";
             lblCategoryId.Text = category.CategoryId.ToString();
             lblCategoryText.Text = category.CategoryText;


             var categoryId = Convert.ToInt32(lblCategoryId.Text);
             
             await getProducts(categoryId);

            }else
            {
                txtCategory.Text = category.CategoryText;
                swchCategoryStatus.IsToggled = category.IsActive;
                btnAddCategory.IsVisible = false;
                btnUpdateCategory.IsVisible = true;
                btnCancelCategory.IsVisible = true;
                btnDeleteCategory.IsVisible = true;
                frmItems.IsEnabled = false;
                frmItems.Opacity = 0.3;
                txtItem.Text = "";
                txtProductPrice.Text = "";
                lblCategoryId.Text = category.CategoryId.ToString();
                lblCategoryText.Text = category.CategoryText;


                var categoryId = Convert.ToInt32(lblCategoryId.Text);

                await getProducts(categoryId);
            }
          


        }

        private void DataGridProduct_SelectionChanged(object sender, Telerik.XamarinForms.DataGrid.DataGridSelectionChangedEventArgs e)
        {
            if(DataGridProduct.SelectedItems != null && DataGridProduct.SelectedItems.Count > 0 && DataGridProduct.SelectedItems[0] != null)
            {
                 var product = (Product)DataGridProduct.SelectedItems[0];
                 txtItem.Text = product.ProductText;
                 txtProductPrice.Text = product.ProductPrice.ToString();
                 btnAddItem.IsVisible = false;
                 btnCancelItem.IsVisible = true;
                 btnUpdateItem.IsVisible = true;
                 btnDeleteItem.IsVisible = true;
            }
            



        }
    }
}