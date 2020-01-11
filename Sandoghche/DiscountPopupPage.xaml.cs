using Rg.Plugins.Popup.Services;
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
    public partial class DiscountPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public DiscountPopupPage()
        {
            InitializeComponent();
            pkrDiscountType.Items.Add("درصد");
            pkrDiscountType.Items.Add("مقدار");
        }

        private void btnAddDiscount_Clicked(object sender, EventArgs e)
        {
            //اگر هنوز فاکتوری صادر نشده و فاکتور خالی است کاربر نمیتواند در ان تخفیف لحاظ کند

            if (string.IsNullOrWhiteSpace(txtDiscountAmount.Text) || pkrDiscountType.SelectedIndex == -1)
            {
                DisplayAlert("اخطار", "اطلاعات تخیخف ناقص است", "باشه");
                return;
            }
            else
            {
                double num;


                if (!double.TryParse(txtDiscountAmount.Text, out num))
                {
                    DisplayAlert("اخطار", "مقدار تخفیف را بصورت عدد وارد کنید", "باشه");
                }
                else
                {
                    double amount = Convert.ToDouble(txtDiscountAmount.Text);
                    int index = pkrDiscountType.SelectedIndex;

                    if ((index == 0) && (amount < 0 || amount > 100))
                    {
                        DisplayAlert("اخطار", "مقدار تخفیف بایستی عددی بین 0 تا 100 باشد", "باشه");
                    }
                    else if ((index == 1) && (amount < 0 || amount > 1000))
                    {
                        DisplayAlert("اخطار", "مقدار تخفیف نمیتواند از مجموع فاکتور کمتر یا  بیشتر باشد", "باشه");
                    }
                    else
                    {
                        PopupNavigation.Instance.PopAsync(true);
                        MessagingCenter.Send(new PopupViewModel() { DiscountType = Convert.ToInt32(pkrDiscountType.SelectedIndex), DiscountAmount = Convert.ToDouble(txtDiscountAmount.Text) }, "Discount");
                    }
                }


            }

        }
    }
}