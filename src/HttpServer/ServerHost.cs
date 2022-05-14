using HttpServer.Handlers;
using HttpServer.Helpers;
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
                using (var client = listener.AcceptTcpClient())
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    string firstLine = reader.ReadLine();
                    for (string? line = null; line != string.Empty; line = reader.ReadLine())
                        ;

                    var request = RequestParser.Parse(firstLine);
                    _handler.Handle(stream, request);
                }
            }
        }
        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                await ProcessClientAsync(client);
            }
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            {
                string firstLine = await reader.ReadLineAsync();
                for (string? line = null; line != string.Empty; line = await reader.ReadLineAsync())
                    ;

                var request = RequestParser.Parse(firstLine);
                _handler.Handle(stream, request);
            }
        }
    }
    public record Request(string Path, HttpMethod Method);
}