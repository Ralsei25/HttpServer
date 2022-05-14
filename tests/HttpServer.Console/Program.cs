using HttpServer;
using HttpServer.Handlers;

ServerHost host = new ServerHost(new ControllersHandler(typeof(Program).Assembly));
host.Start();