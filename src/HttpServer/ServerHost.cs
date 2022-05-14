using System.Net;
using System.Net.Sockets;

namespace HttpServer
{
    public class ServerHost
    {
        private string _answer;
        public ServerHost(string answer)
        {
            _answer = answer;
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                using (var stream = client.GetStream())
                {
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(stream))
                    {
                        for (string? line = null; line != string.Empty; line = reader.ReadLine())
                        {
                            Console.WriteLine(line);
                        }

                        writer.Write(_answer);
                    }
                }
            }
        }
    }
}