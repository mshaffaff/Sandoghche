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
using Sandoghche.Models;
using Sandoghche.Components;
using SQLiteNetExtensionsAsync.Extensions;
namespace Sandoghche
{


    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        //private static Order order = new Order();

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

            getOrder();

            //var test = SandoghcheController._connection.Table<Order>().Where(c => c.OrderId == 5).to;
            //foreach (var item in test)
            //{
            //    order=
            //}
            //DependencyService.Get<IPrint>().Print(order);

        }
        async Task getOrder(string Searchtext = null)
        {

            //var order = Task.Run(async () => await SandoghcheController.GetConnection().Table<Order>().FirstOrDefaultAsync(c => c.OrderId == 5)).Result;

            //var orderDetail = db.GetWithChildren<Event>(event1.Id);
            
            var order = await SandoghcheController.GetConnection().GetWithChildrenAsync<Order>(5);
         
           // DependencyService.Get<IPrint>().Print(order);
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