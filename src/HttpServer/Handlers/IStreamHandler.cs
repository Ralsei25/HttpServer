namespace HttpServer.Handlers
{
    public interface IStreamHandler
    {
        void Handle(Stream stream, Request request);
        Task HandleAsync(Stream stream, Request request);
    }
}
