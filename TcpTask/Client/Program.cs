using System.Net.Sockets;
using System.Net;
using System.Drawing;

var ip = IPAddress.Parse("192.168.1.7");
var port = 27001;

var ep = new IPEndPoint(ip, port);

try
{
    while (true)
    {
        _ = Task.Run(() =>
        {
            using (var client = new TcpClient())
            {
                client.Connect(ep);
                var networkStream = client.GetStream();
                using (var memoryStream = new MemoryStream())
                {
                    Bitmap memoryImage = new Bitmap(1920, 1080);
                    Size s = new Size(memoryImage.Width, memoryImage.Height);

                    Graphics memoryGraphics = Graphics.FromImage(memoryImage);
                    memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
                    memoryImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    var buffer = memoryStream.ToArray();
                    networkStream.Write(buffer, 0, buffer.Length);
                    networkStream.Flush();
                }
                Console.WriteLine("Sent Successfully!");
                client.Close();
            }
        });
        await Task.Delay(10000);
    }

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}