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
            var fileNameLengthBuffer = new byte[4];
            networkStream.Read(fileNameLengthBuffer, 0, 4);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthBuffer, 0);
            var fileNameBuffer = new byte[fileNameLength];
            networkStream.Read(fileNameBuffer, 0, fileNameLength);
            string fileName = System.Text.Encoding.UTF8.GetString(fileNameBuffer);
            var path = Path.Combine(directoryPath, fileName);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                int len = 0;
                var buffer = new byte[65536];
                while ((len = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, len);
                }
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