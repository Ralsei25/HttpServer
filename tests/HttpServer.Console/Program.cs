using HttpServer;

ServerHost host = new ServerHost(new StaticFileHandler(Path.Combine(Environment.CurrentDirectory, "www")));
host.Start();