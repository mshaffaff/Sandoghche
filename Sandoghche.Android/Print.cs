using Android.Content;
using Android.OS;
using Android.Print;
using Java.IO;
using Sandoghche.Droid;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(Print))]

namespace Sandoghche.Droid
{
    public class Print : IPrintPdf
    {
        [Obsolete]
        void IPrintPdf.PrintPdf(byte[] content, int OrderId)
        {
            Stream inputStream = new MemoryStream(content);
            string fileName = OrderId.ToString()+".pdf";
            if (inputStream.CanSeek)
                //Reset the position of PDF document stream to be printed
                inputStream.Position = 0;
            //Create a new file in the Personal folder with the given name
            string createdFilePath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), fileName);
            //Save the stream to the created file
            using (var dest = System.IO.File.OpenWrite(createdFilePath))
                inputStream.CopyTo(dest);
            string filePath = createdFilePath;
            PrintManager printManager = (PrintManager)Forms.Context.GetSystemService(Context.PrintService);

            PrintDocumentAdapter pda = new CustomPrintDocumentAdapter(filePath);
            printManager.Print("test", pda, null);
        }
    }
}
internal class CustomPrintDocumentAdapter : PrintDocumentAdapter
{
    private string filePath;
    public CustomPrintDocumentAdapter(string filePath)
    {
        this.filePath = filePath;
    }
    public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
    {
        if (cancellationSignal.IsCanceled)
        {
            callback.OnLayoutCancelled();
            return;
        }
        callback.OnLayoutFinished(new PrintDocumentInfo.Builder(filePath)
        .SetContentType(PrintContentType.Document)
        .Build(), true);
    }

    public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
    {
        try
        {
            using (InputStream input = new FileInputStream(filePath))
            {
                using (OutputStream output = new FileOutputStream(destination.FileDescriptor))
                {
                    var buf = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = input.Read(buf)) > 0)
                    {
                        output.Write(buf, 0, bytesRead);
                    }
                }
            }
            callback.OnWriteFinished(new[] { PageRange.AllPages });
        }
        catch (System.IO.FileNotFoundException fileNotFoundException)
        {
            System.Diagnostics.Debug.WriteLine(fileNotFoundException);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
        }
    }
}