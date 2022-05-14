using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class StaticFileHandler : IStreamHandler
    {
        private readonly string _path;
        public StaticFileHandler(string path)
        {
            _path = path;
        }
        public void Handle(Stream networkStream)
        {
            using (var reader = new StreamReader(networkStream))
            using (var writer = new StreamWriter(networkStream))
            {
                string firstLine = reader.ReadLine();
                for (string? line = null; line != string.Empty; line = reader.ReadLine())
                    ;

                var request = RequestParser.Parse(firstLine);
                var filePath = Path.Combine(_path, request.Path[1..]);

                if (!File.Exists(filePath))
                {
                    ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);
                }
                else
                {
                    ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        fileStream.CopyTo(networkStream);
                    }
                }
            }
        }
    }
}
