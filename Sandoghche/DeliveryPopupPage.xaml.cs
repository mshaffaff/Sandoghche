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
    public partial class DeliveryPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public DeliveryPopupPage(double deliveryFee = 0.0,bool forHistory = false)
        {
            InitializeComponent();
            txtDeliveryFee.Text = deliveryFee.ToString();
            if (forHistory)
                btnAddDelivery.IsEnabled = false;
            else
                btnAddDelivery.IsEnabled = true;
        }

        private void btnAddDelivery_Clicked(object sender, EventArgs e)
        {
            //اگر هنوز فاکتوری صادر نشده و فاکتور خالی است کاربر نمیتواند در ان هزینه پیک لحاظ کند
            double num;

            if (!double.TryParse(txtDeliveryFee.Text, out num))
            {
                DisplayAlert("اخطار", "هزینه پیک را بصورت عدد وارد کنید", "باشه");
                return;
            }

            PopupNavigation.Instance.PopAsync(true);

            MessagingCenter.Send(new PopupViewModel() { DeliveryFee =Convert.ToDouble(txtDeliveryFee.Text) }, "txtDeliveryFee");

        }
    }
}