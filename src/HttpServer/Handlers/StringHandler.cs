namespace HttpServer.Handlers
{
    public class StringHandler : IStreamHandler
    {
        private readonly string _answer;
        public StringHandler(string answer)
        {
            _answer = answer;
        }
        public void Handle(Stream stream, Request request)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(_answer);
            }
        }
    }
}
