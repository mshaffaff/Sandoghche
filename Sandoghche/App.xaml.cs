using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sandoghche
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzM3NEAzMTM3MmUzNDJlMzBPRm41TTBEL2hiZ0pjbG93dDZPQ0VocmRCWkJHSXlzWFgrUkxrZVlDaUpzPQ==");

            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
