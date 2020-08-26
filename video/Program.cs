using System;
using Gtk;
using Gdk;
using System.Runtime.InteropServices;
using Microsoft.Psi;
using Microsoft.Psi.Media;
using System.Threading;
using Microsoft.Psi.Imaging;

namespace XamarinCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Application.Init();
            var bld = new Builder();
            bld.AddFromString(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<interface>" +
                    "<requires lib=\"gtk+\" version=\"3.12\"/>" +
                    "<object class=\"GtkWindow\" id=\"window\">" +
                        "<child>" + 
                            "<object class=\"GtkImage\" id=\"image\">" + 
                                "<property name=\"width_request\">256</property>" + 
                                "<property name=\"height_request\">256</property>" + 
                                "<property name=\"visible\">True</property>" + 
                                "<property name=\"can_focus\">False</property>" + 
                                "<property name=\"stock\">gtk-missing-image</property>" + 
                            "</object>" + 
                        "</child>" + 
                    "</object>" + 
                "</interface>");
            var win = (Gtk.Window)bld.GetObject("window");
            win.DeleteEvent += (_, __) => Application.Quit();
            var img = (Gtk.Image)bld.GetObject("image");
            win.Show();

            new Thread(new ThreadStart(() =>
            {
                using (var pipeline = Pipeline.Create())
                {
                    ////var webcam = new MediaCapture(pipeline, 1920, 1080, 30);
                    var webcam = new MediaCapture(pipeline, 640, 480, 30);
                    webcam.Video.Do(frame =>
                    {
                        Console.Write(".");
                        var len = frame.Resource.Width * frame.Resource.Height * 3;
                        var data = new byte[len];
                        Marshal.Copy(frame.Resource.ImageData, data, 0, len);
                        var buf = new Pixbuf(data, false, 8, frame.Resource.Width, frame.Resource.Height, frame.Resource.Stride);
                        img.Pixbuf = buf;
                    });

                    pipeline.Run();
                }
            })).Start();


            ////var images = webcam.Out.EncodeJpeg(90, DeliveryPolicy.LatestMessage).Out;

            ////int dataLen = img.Resource.Width * img.Resource.Height * 3;
            ////byte[] imageData = new byte[dataLen];
            ////System.Runtime.InteropServices.Marshal.Copy(img.Resource.ImageData, imageData, 0, dataLen);
            ////var pixbuf = new Gdk.Pixbuf(imageData, false, 8, img.Resource.Width, img.Resource.Height, img.Resource.Stride);
            ////imgCtrl.Pixbuf = pixbuf.ScaleSimple(900, 640, Gdk.InterpType.Bilinear);

            Application.Run();
        }
    }
}
