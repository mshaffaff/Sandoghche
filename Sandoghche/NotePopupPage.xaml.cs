using Rg.Plugins.Popup.Services;
using Sandoghche.ModelView;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotePopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public NotePopupPage()
        {
            InitializeComponent();
        }

        private void btnAddNote_Clicked(object sender, EventArgs e)
        {
            //اگر هنوز فاکتوری صادر نشده و فاکتور خالی است کاربر نمیتواند در ان توضیحات لحاظ کند
            PopupNavigation.Instance.PopAsync(true);

            MessagingCenter.Send(new PopupViewModel() { CommentText = txtComment.Text }, "txtComment");

        }
    }
}