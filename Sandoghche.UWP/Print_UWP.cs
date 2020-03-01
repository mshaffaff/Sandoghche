using Sandoghche.Models;
using System;
using System.Diagnostics;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Printing;


namespace Sandoghche.UWP
{

    public class Print_UWP
    {
        PrintManager printmgr = PrintManager.GetForCurrentView();
        PrintDocument PrintDoc;
        PrintDocument printDoc;
        PrintTask Task = null;

        StackPanel testPanel = new StackPanel();
        Grid Receipt = new Grid { Width = 300 };



        public Print_UWP()
        {
            printmgr.PrintTaskRequested += Printmgr_PrintTaskRequested;
        }

        public async void PrintUWpAsync(Order order,string receiptType)
        {
            #region Grid Print Document
            Grid Receipt = new Grid { Width = 300 };

            #region Main Grid Definition
            Receipt.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(30, GridUnitType.Pixel)
            });
            Receipt.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(30, GridUnitType.Pixel)
            });
            Receipt.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(50, GridUnitType.Pixel)
            });

            Receipt.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });
            Receipt.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(30, GridUnitType.Pixel)
            });
            Receipt.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(30, GridUnitType.Pixel)
            });

            Receipt.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(300, GridUnitType.Pixel)
            });
            #endregion

            #region Store Name
            TextBlock StoreName = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = "بابا بستنی",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18,
                FontWeight = Windows.UI.Text.FontWeights.Bold

            };

            #endregion
            Receipt.Children.Add(StoreName);
            Grid.SetRow(StoreName, 0);

            #region ReceiptType
            TextBlock ReceiptType = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = receiptType,
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            };
            #endregion
            Receipt.Children.Add(ReceiptType);
            Grid.SetRow(ReceiptType, 1);

            StackPanel row2 = new StackPanel();

            #region ReceiptNumber ,User,Date,Time
            Grid ReceiptNumber = new Grid { Background = new SolidColorBrush(Windows.UI.Colors.LightGray) };
            ReceiptNumber.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(25, GridUnitType.Pixel)
            });
            ReceiptNumber.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(25, GridUnitType.Pixel)
            });
            ReceiptNumber.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(150, GridUnitType.Pixel)
            });
            ReceiptNumber.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(150, GridUnitType.Pixel)
            });

            TextBlock User = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = "کاربر : مدیر",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            ReceiptNumber.Children.Add(User);
            Grid.SetRow(User, 0); Grid.SetColumn(User, 0);

            TextBlock ReceiptNum = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = "شماره : " + order.ReceiptNumber,
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            };
            ReceiptNumber.Children.Add(ReceiptNum);
            Grid.SetRow(ReceiptNum, 0); Grid.SetColumn(ReceiptNum, 1);

            TextBlock Time = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = "ساعت :" + Convert.ToDateTime(order.DateCreated).ToString("HH:mm:ss"),
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            ReceiptNumber.Children.Add(Time);
            Grid.SetRow(Time, 1); Grid.SetColumn(Time, 0);

            TextBlock Date = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = "تاریخ : " + Convert.ToDateTime(order.DateCreated).ToString("dd:MM:yyyy"),
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            ReceiptNumber.Children.Add(Date);
            Grid.SetRow(Date, 1); Grid.SetColumn(Date, 1);

            #endregion
            row2.Children.Add(ReceiptNumber);
            Receipt.Children.Add(row2);
            Grid.SetRow(row2, 2);


            StackPanel row3 = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right, Width = 300 };

            #region Order Detail Header
            StackPanel PanelHeader = new StackPanel { Orientation = Orientation.Horizontal };
            Grid Header = new Grid { Width = 300 };
            Header.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(60, GridUnitType.Pixel)
            });
            Header.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(60, GridUnitType.Pixel)
            });
            Header.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(40, GridUnitType.Pixel)
            });
            Header.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            Header.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(40, GridUnitType.Pixel)
            });

            TextBlock TotalPriceHeader = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = "قیمت کل",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
            };
            Header.Children.Add(TotalPriceHeader); Grid.SetColumn(TotalPriceHeader, 0);

            TextBlock PriceHeader = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = "قیمت",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
            };
            Header.Children.Add(PriceHeader); Grid.SetColumn(PriceHeader, 1);

            TextBlock NumberHeader = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = "تعداد",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
            };
            Header.Children.Add(NumberHeader); Grid.SetColumn(NumberHeader, 2);

            TextBlock ProductTextHeader = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Text = "عنوان",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
            };
            Header.Children.Add(ProductTextHeader); Grid.SetColumn(ProductTextHeader, 3);

            TextBlock RowNumberHeader = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = "ردیف",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
            };
            Header.Children.Add(RowNumberHeader); Grid.SetColumn(RowNumberHeader, 4);

            PanelHeader.Children.Add(Header);
            #endregion

            //GridView ProductGridviewContent = new GridView();
            // StackPanel OrderDetailPanel = new StackPanel { Orientation = Orientation.Vertical };
            #region Order Detail Content
            StackPanel PanelContent = new StackPanel { Orientation = Orientation.Vertical };

            Grid Contents = new Grid { Width = 300 };

            ListView lstContents = new ListView();

            Contents.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(60, GridUnitType.Pixel)
            });

            Contents.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(60, GridUnitType.Pixel)
            });
            Contents.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(40, GridUnitType.Pixel)
            });
            Contents.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            Contents.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(40, GridUnitType.Pixel)
            });

            for (int i = 0; i < order.OrderDetails.Count; i++) //Add 36 columns
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(40, GridUnitType.Pixel);
                Contents.RowDefinitions.Add(row);
            }

            for (int rowIndex = 0; rowIndex < order.OrderDetails.Count; rowIndex++)
            {

                TextBlock TotalPriceContent = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = order.OrderDetails[rowIndex].TotalPrice.ToString(),
                    FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
                };
                Contents.Children.Add(TotalPriceContent);
                Grid.SetColumn(TotalPriceContent, 0);
                Grid.SetRow(TotalPriceContent, rowIndex);

                TextBlock PriceContent = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = order.OrderDetails[rowIndex].Price.ToString(),
                    FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
                };
                Contents.Children.Add(PriceContent);
                Grid.SetColumn(PriceContent, 1);
                Grid.SetRow(PriceContent, rowIndex);

                TextBlock NumberContent = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = order.OrderDetails[rowIndex].Number.ToString(),
                    FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
                };
                Contents.Children.Add(NumberContent);
                Grid.SetColumn(NumberContent, 2);
                Grid.SetRow(NumberContent, rowIndex);

                TextBlock ProductTextContent = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Text = order.OrderDetails[rowIndex].ProductText,
                    FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
                };
                Contents.Children.Add(ProductTextContent);
                Grid.SetColumn(ProductTextContent, 3);
                Grid.SetRow(ProductTextContent, rowIndex);

                TextBlock RowNumberContent = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = order.OrderDetails[rowIndex].RowNumber.ToString(),
                    FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                    Foreground = new SolidColorBrush(Windows.UI.Colors.Black)
                };
                Contents.Children.Add(RowNumberContent);
                Grid.SetColumn(RowNumberContent, 4);
                Grid.SetRow(RowNumberContent, rowIndex);





            }
            PanelContent.Children.Add(Contents);
            #endregion

            row3.Children.Add(PanelHeader);
            row3.Children.Add(PanelContent);
            Receipt.Children.Add(row3);
            Grid.SetRow(row3, 3);



            #region Final Payment
            TextBlock FinalPayment = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = "مجموع : " + order.FinalPayment.ToString(),
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            };
            #endregion
            Receipt.Children.Add(FinalPayment);
            Grid.SetRow(FinalPayment, 4);

            #region Note
            TextBlock Note = new TextBlock
            {
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
                Text = "شیراز - خیابان عفیف آباد",
                FontFamily = new FontFamily("IRANSansMobile(FaNum).ttf#IRANSansMobile(FaNum)"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight = Windows.UI.Text.FontWeights.Bold
            };
            #endregion
            Receipt.Children.Add(Note);
            Grid.SetRow(Note, 5);

            testPanel.Children.Add(Receipt);
            #endregion

            if (PrintDoc != null)
            {
                printDoc.GetPreviewPage -= PrintDoc_GetPreviewPage;
                printDoc.Paginate -= PrintDoc_Paginate;
                printDoc.AddPages -= PrintDoc_AddPages;


            }


            this.printDoc = new PrintDocument();
            try
            {

                printDoc.GetPreviewPage += PrintDoc_GetPreviewPage;
                printDoc.Paginate += PrintDoc_Paginate;
                printDoc.AddPages += PrintDoc_AddPages;

                bool showprint = await PrintManager.ShowPrintUIAsync();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            PrintDoc = null;
            PrintManager printmgr = PrintManager.GetForCurrentView();
            printmgr.PrintTaskRequested -= Printmgr_PrintTaskRequested;
            GC.Collect();

        }

        private void Printmgr_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var deff = args.Request.GetDeferral();
            Task = args.Request.CreatePrintTask("Invoice", OnPrintTaskSourceRequested);
            deff.Complete();

        }

        async void OnPrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            var def = args.GetDeferral();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                args.SetSource(printDoc.DocumentSource);
            });
            def.Complete();


        }

        private void PrintDoc_AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(testPanel);
            printDoc.AddPagesComplete();
        }

        private void PrintDoc_Paginate(object sender, PaginateEventArgs e)
        {
            PrintTaskOptions opt = Task.Options;
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void PrintDoc_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDoc.SetPreviewPage(e.PageNumber, testPanel);

        }



        public void UnregisterForPrinting()
        {
            if (PrintDoc == null)
            {
                return;
            }

            printDoc.GetPreviewPage -= PrintDoc_GetPreviewPage;
            printDoc.Paginate -= PrintDoc_Paginate;
            printDoc.AddPages -= PrintDoc_AddPages;

            PrintManager printmgr = PrintManager.GetForCurrentView();
            printmgr.PrintTaskRequested -= Printmgr_PrintTaskRequested;
        }

    }

}
