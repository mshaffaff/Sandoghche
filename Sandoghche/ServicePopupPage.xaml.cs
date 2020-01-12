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
    public partial class ServicePopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public ServicePopupPage(string totalPrice = null)
        {
            InitializeComponent();
            lblTotalPrice.Text = totalPrice;
            pkrServiceType.Items.Add("درصد");
            pkrServiceType.Items.Add("مقدار");
        }

        private void btnAddService_Clicked(object sender, EventArgs e)
        {
            //اگر هنوز فاکتوری صادر نشده و فاکتور خالی است کاربر نمیتواند در ان تخفیف لحاظ کند

            if (string.IsNullOrWhiteSpace(txtServiceAmount.Text) || pkrServiceType.SelectedIndex == -1)
            {
                DisplayAlert("اخطار", "اطلاعات هزینه سرویس ناقص است", "باشه");
                return;
            }
            else
            {
                double num;


                if (!double.TryParse(txtServiceAmount.Text, out num))
                {
                    DisplayAlert("اخطار", "مقدار سرویس را بصورت عدد وارد کنید", "باشه");
                }
                else
                {
                    double amount = Convert.ToDouble(txtServiceAmount.Text);
                    int index = pkrServiceType.SelectedIndex;

                    if ((index == 0) && (amount < 0 || amount > 100))
                    {
                        DisplayAlert("اخطار", "مقدار سرویس بایستی عددی بین 0 تا 100 باشد", "باشه");
                    }
                    else if ((index == 1) && (amount < 0 || amount > Convert.ToDouble(lblTotalPrice.Text)))
                    {
                        DisplayAlert("اخطار", "مقدار سرویس نمیتواند از مجموع فاکتور کمتر یا  بیشتر باشد", "باشه");
                    }
                    else
                    {
                        PopupNavigation.Instance.PopAsync(true);
                        MessagingCenter.Send(new PopupViewModel() { ServiceType = Convert.ToInt32(pkrServiceType.SelectedIndex), ServiceAmount = Convert.ToDouble(txtServiceAmount.Text) }, "Service");
                    }
                }


            }
        }
    }
}