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
            NavigationPage.SetHasBackButton(this, false);

            lblPersianDate.Text = SandoghcheController.GetPersianDate(null);
            lblUser.Text = Application.Current.Properties["userRollName"].ToString() + " : " + Application.Current.Properties["FullName"].ToString();
        }

        async protected override void OnAppearing()
        {
            await getCategories();
            base.OnAppearing();
        }

        async Task getCategories(string Searchtext = null)
        {
            var categories = await SandoghcheController.GetConnection().Table<Category>().Where(c => c.IsDeleted != true).ToListAsync();

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

                txtCategory.Text = string.Empty;
                srchCategory.Text = string.Empty;

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

            txtItem.Text = "";
            txtProductPrice.Text = "";
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
                DataGridProduct.ItemsSource = null;
                lblCategoryId.Text = "";
                lblCategoryText.Text = "انتخاب نشده";
                frmItems.IsEnabled = false;
                frmItems.Opacity = 0.3;


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

        async Task getProducts(int categoryId, string Searchtext = null)
        {

            var products = await SandoghcheController._connection.Table<Product>().Where(p => p.IsDeleted != true && p.CategoryId == categoryId).ToListAsync();

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
            if (
                String.IsNullOrWhiteSpace(txtItem.Text) ||
                String.IsNullOrWhiteSpace(txtProductPrice.Text) ||
                !double.TryParse(txtProductPrice.Text, out num) ||
                !double.TryParse(txtProductAmount.Text, out num) ||
                Convert.ToDouble(txtProductAmount.Text) < 0 || Convert.ToDouble(txtProductPrice.Text) <= 0)
                await DisplayAlert("خطا", "ورود مقادیر اشتباه است", "باشه");
            else if (String.IsNullOrWhiteSpace(lblCategoryId.Text))
            {
                await DisplayAlert("خطا", " طبقه را مشخص کنید ", "باشه");
            }
            else
            {
                var product = new Product();
                product.ProductText = txtItem.Text;
                product.ProductPrice = Convert.ToDouble(txtProductPrice.Text);
                product.ProductAmount = Convert.ToDouble(txtProductAmount.Text);
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
            txtItem.Text = "";
            txtProductPrice.Text = "";
            swchItemStatus.IsToggled = true;
            btnAddItem.IsVisible = true;
            btnUpdateItem.IsVisible = false;
            btnDeleteItem.IsVisible = false;
            btnCancelItem.IsVisible = false;
            srchProduct.Text = "";



        }

        async private void btnDeleteItem_Clicked(object sender, EventArgs e)
        {
            var product = (Product)DataGridProduct.SelectedItems[0];
            bool action = await DisplayAlert("اخطار", "آیا از حذف این محصول اطمینان دارید ؟", "بله", "خیر");
            if (action)
            {
                product.IsDeleted = true;
                await SandoghcheController._connection.UpdateAsync(product);

                await getProducts(Convert.ToInt32(lblCategoryId.Text));

                swchItemStatus.IsToggled = true;
                btnAddItem.IsVisible = true;
                btnCancelItem.IsVisible = false;
                btnUpdateItem.IsVisible = false;
                btnDeleteItem.IsVisible = false;
                txtItem.Text = "";
                txtProductPrice.Text = "";
                txtProductAmount.Text = "0";
                srchProduct.Text = "";
            }


        }

        async private void btnUpdateItem_Clicked(object sender, EventArgs e)
        {
            var product = (Product)DataGridProduct.SelectedItems[0];
            double num;
            if (String.IsNullOrWhiteSpace(lblCategoryId.Text) || String.IsNullOrWhiteSpace(lblCategoryText.Text))
                await DisplayAlert("اخطار", "طبقه بندی نامشخص است", "باشه");
            else if (String.IsNullOrWhiteSpace(txtItem.Text) || String.IsNullOrWhiteSpace(txtProductPrice.Text))
                await DisplayAlert("اخطار", "نام محصول یا قیمت محصول خالی است", "باشه");
            else if (!double.TryParse(txtProductPrice.Text, out num) || !double.TryParse(txtProductAmount.Text, out num))
                await DisplayAlert("اخطار", "مقادیر را به عدد وارد کنید", "باشه");
            else if (Convert.ToDouble(txtProductPrice.Text) <= 0 || Convert.ToDouble(txtProductAmount.Text) < 0)
                await DisplayAlert("اخطار", "مقادیر نمیتواند منفی باشد", "باشه");
            else
            {
                product.ProductText = txtItem.Text;
                product.ProductPrice = Convert.ToDouble(txtProductPrice.Text);
                product.ProductAmount = Convert.ToDouble(txtProductAmount.Text);
                product.IsActive = swchItemStatus.IsToggled;
                await SandoghcheController._connection.UpdateAsync(product);
                txtItem.Text = "";
                txtProductPrice.Text = "";
                txtProductAmount.Text = "0";
                srchProduct.Text = "";
                btnAddItem.IsVisible = true;
                btnCancelItem.IsVisible = false;
                btnUpdateItem.IsVisible = false;
                btnDeleteItem.IsVisible = false;
                await getProducts(Convert.ToInt32(lblCategoryId.Text));


            }

        }

        async private void srchCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataGridProduct.ItemsSource = null;
            lblCategoryId.Text = null;
            lblCategoryText.Text = "انتخاب نشده";
            txtItem.Text = "";
            txtProductPrice.Text = "";
            srchProduct.Text = "";
            btnAddItem.IsVisible = true;
            btnCancelItem.IsVisible = false;
            btnUpdateItem.IsVisible = false;
            btnDeleteItem.IsVisible = false;
            frmItems.IsEnabled = false;
            frmItems.Opacity = 0.3;


            await getCategories(e.NewTextValue);
        }
                     
        async private void lstCategory_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var category = (Category)e.SelectedItem;
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
                frmItems.IsEnabled = true;
                frmItems.Opacity = 1;
                txtItem.Text = "";
                txtProductPrice.Text = "";
                swchItemStatus.IsToggled = true;
                lblCategoryId.Text = category.CategoryId.ToString();
                lblCategoryText.Text = category.CategoryText;


                var categoryId = Convert.ToInt32(lblCategoryId.Text);

                await getProducts(categoryId);

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
            if (DataGridProduct.SelectedItems != null && DataGridProduct.SelectedItems.Count > 0 && DataGridProduct.SelectedItems[0] != null)
            {
                var product = (Product)DataGridProduct.SelectedItems[0];
                txtItem.Text = product.ProductText;
                txtProductPrice.Text = product.ProductPrice.ToString();
                txtProductAmount.Text = product.ProductAmount.ToString();
                swchItemStatus.IsToggled = product.IsActive;
                btnAddItem.IsVisible = false;
                btnCancelItem.IsVisible = true;
                btnUpdateItem.IsVisible = true;
                btnDeleteItem.IsVisible = true;
            }
        }

        async private void srchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtItem.Text = "";
            txtProductPrice.Text = "";
            btnAddItem.IsVisible = true;
            btnCancelItem.IsVisible = false;
            btnUpdateItem.IsVisible = false;
            btnDeleteItem.IsVisible = false;
            await getProducts(Convert.ToInt32(lblCategoryId.Text), e.NewTextValue);


        }

        private void btnHome_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

    }
}