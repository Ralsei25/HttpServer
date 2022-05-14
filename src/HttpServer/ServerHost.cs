using System.Net;
using System.Net.Sockets;

namespace HttpServer
{
    public class ServerHost
    {
        private readonly IStreamHandler _handler;
        public ServerHost(IStreamHandler handler)
        {
            _handler = handler;
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
                    _handler.Handle(stream);
                }
            }
        }
    }
}