using HttpServer;
using HttpServer.Handlers;

ServerHost host = new ServerHost(new StaticFileHandler(Path.Combine(Environment.CurrentDirectory, "www")));
host.Start();