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
            srchCategory.Text = "";
            frmItems.IsEnabled = false;
            frmItems.Opacity = .3;
            lblCategoryText.Text = "انتخاب نشده";
            lblCategoryId.Text = "";

                                 
            await getCategories();


        }

        async private void btnDeleteCategory_Clicked(object sender, EventArgs e)
        {
            var category = (Category)lstCategory.SelectedItem;

            var action = await DisplayAlert("اخطار", "آیا میخواید این طبقه بندی حذف شود", "بله", "خیر");

            if(action)
            {
                category.IsDeleted = true;
                await SandoghcheController._connection.UpdateAsync(category);

                ////عملیات حذف محصولات هم بایستی انجام شود

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

            if (String.IsNullOrWhiteSpace(txtCategory.Text))
                await DisplayAlert("خطا", "طبقه بندی نمی تواند خالی باشد", "باشه");
            else
            {
                category.CategoryText = txtCategory.Text;
                category.IsActive = swchCategoryStatus.IsToggled;

                await SandoghcheController._connection.UpdateAsync(category);
                await DisplayAlert("ثبت", "بروز رسانی با موفقیت انجام شد", "باشه");

                txtCategory.Text = "";
                swchCategoryStatus.IsToggled = true;
                btnAddCategory.IsVisible = true;
                btnUpdateCategory.IsVisible = false;
                btnCancelCategory.IsVisible = false;
                btnDeleteCategory.IsVisible = false;
                srchCategory.Text = "";


                await getCategories();
            }

        }


       async private void btnAddItem_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtItem.Text) || String.IsNullOrWhiteSpace(txtProductPrice.Text))
                await DisplayAlert("خطا", " نام محصول یا قیمت خالی است", "باشه");
            else
            {
                var product = new Product();

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




        private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var category = (Category)e.SelectedItem;
            txtCategory.Text = category.CategoryText;
            swchCategoryStatus.IsToggled = category.IsActive;
            btnAddCategory.IsVisible = false;
            btnUpdateCategory.IsVisible = true;
            btnCancelCategory.IsVisible = true;
            btnDeleteCategory.IsVisible = true;
            frmItems.IsEnabled = true;
            frmItems.Opacity = 1;
            lblCategoryText.Text = category.CategoryText;




        }

    }
}