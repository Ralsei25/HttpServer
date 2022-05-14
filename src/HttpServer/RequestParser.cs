namespace HttpServer
{
    public record Request(string Path, HttpMethod Method);
    internal static class RequestParser
    {
        public static Request Parse(string header)
        {
            var split = header.Split(" ");
            return new Request(split[1], GetMethod(split[0]));
        }
        public static HttpMethod GetMethod(string method)
        {
            switch (method)
            {
                case "GET":
                    return HttpMethod.Get;
                case "POST":
                    return HttpMethod.Post;
                case "PUT":
                    return HttpMethod.Put;
                case "DELETE":
                    return HttpMethod.Delete;
                case "HEAD":
                    return HttpMethod.Head;
                case "PATCH":
                    return HttpMethod.Patch;
                case "OPTIONS":
                    return HttpMethod.Options;
                case "TRACE":
                    return HttpMethod.Trace;
                default:
                    throw new ArgumentException($"{method} is unknown");
            }
        }
    }
}
