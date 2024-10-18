using System.Net.Sockets;
using System.Net;

var ip = IPAddress.Parse("192.168.1.7");
var port = 27001;

var ep = new IPEndPoint(ip, port);

var client = new TcpClient();
try
{
    Console.WriteLine("Enter the path: ");
    var path = Console.ReadLine();
    client.Connect(ep);
    if (client.Connected)
    {
        var fileName = Path.GetFileName(path!);
        var fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
        var fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);

        var networkStream = client.GetStream();

        networkStream.Write(fileNameLength, 0, fileNameLength.Length);
        networkStream.Write(fileNameBytes, 0, fileNameBytes.Length);

        using (var fs = new FileStream(path!, FileMode.Open, FileAccess.Read))
        {
            var buffer = new byte[65536];
            int len;
            while ((len = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                networkStream.Write(buffer, 0, len);
            }
        }

        networkStream.Close();
        client.Close();
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
