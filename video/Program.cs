namespace VideoSample
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Gtk;
    using Gdk;
    using Microsoft.Psi;
    using Microsoft.Psi.Imaging;
    using Microsoft.Psi.Media;
	
    class Program
    {
        static void Main(string[] args)
        {
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
                    var webcam = new MediaCapture(pipeline, 640, 480, "/dev/video0", PixelFormatId.YUYV);
                    webcam.Out.Do(frame =>
                    {
                        var len = frame.Resource.Width * frame.Resource.Height * 3;
                        var data = new byte[len];
                        Marshal.Copy(frame.Resource.ImageData, data, 0, len);
                        var buf = new Pixbuf(data, false, 8, frame.Resource.Width, frame.Resource.Height, frame.Resource.Stride);
                        img.Pixbuf = buf;
                    });
                    pipeline.Run();
                }
            })) { IsBackground = true }.Start();

            Application.Run();
        }
    }
}
