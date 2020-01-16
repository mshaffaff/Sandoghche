using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Drawing.Printing;
using System.IO;
using System.Net;

namespace Sandoghche
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {

        private byte[] imageContent;
        //    (System.Drawing.Image img) {
        //using (var ms = new MemoryStream())
        //    {
        //        img.Save(ms, img.RawFormat);
        //        return ms.ToArray();
        //    }
        //}

        public SettingsPage()
        {
            InitializeComponent();
            DownloadPicAsync("https://vector.me/files/images/1/7/17513/tottenham_hotspur_fc.png");

            //imageContent = ImageContent((System.Drawing.Image))imgtest);
            DependencyService.Get<IPrint>().Print(imageContent);

        }
        private void DownloadPicAsync(string url)
        {
            using (var webClient = new WebClient())
            {

                var imageBytes = webClient.DownloadData(new Uri(url));
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imgtest.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                    imageContent = imageBytes;
                }
            }
        }
    }
}