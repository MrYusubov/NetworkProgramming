using System.Net;
using System.Net.Sockets;

var ip = IPAddress.Parse("192.168.1.7");
var port = 27001;
var ep = new IPEndPoint(ip, port);
var listener = new TcpListener(ep);
try
{
    listener.Start();
    while (true)
    {
        var client = listener.AcceptTcpClient();
        _ = Task.Run(() =>
        {
            var networkStream = client.GetStream();
            var remoteEp = client.Client.RemoteEndPoint as IPEndPoint;
            var directoryPath = Path.Combine(Environment.CurrentDirectory, remoteEp!.Address.ToString()!);
            if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
            var path = Path.Combine(directoryPath, $"{DateTime.Now.ToString("yyyy/MM/dd/HH/mm/ss")}.png");
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                int len = 0;
                var buffer = new byte[1024];
                while ((len = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, len);
                }
                fs.Close();
            }
            Console.WriteLine("File Received!");
            client.Close();
        });
    }
   


}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}