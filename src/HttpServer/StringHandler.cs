using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class StringHandler : IStreamHandler
    {
        private readonly string _answer;
        public StringHandler(string answer)
        {
            _answer = answer;
        }
        public void Handle(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                for (string? line = null; line != string.Empty; line = reader.ReadLine()) 
                    ;

                writer.Write(_answer);
            }
        }
    }
}
