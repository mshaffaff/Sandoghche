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
       public NotePopupPage(string comment="")
        {
            InitializeComponent();
            txtComment.Text = comment;
        }

        private void btnAddNote_Clicked(object sender, EventArgs e)
        {            
            PopupNavigation.Instance.PopAsync(true);
            MessagingCenter.Send(new PopupViewModel() { CommentText = txtComment.Text }, "txtComment");

        }
    }
}