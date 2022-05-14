namespace HttpServer.Handlers
{
    public interface IStreamHandler
    {
        void Handle(Stream stream, Request request);
    }
}
