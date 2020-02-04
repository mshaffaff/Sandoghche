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
    public partial class EditPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public EditPopupPage()
        {

            InitializeComponent();

        }
        private void btnShowReceipt_Clicked(object sender, EventArgs e)
        {

        }
    }
}