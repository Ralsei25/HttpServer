using HttpServer;
using HttpServer.Handlers;

ServerHost host = new ServerHost(new ControllersHandler(typeof(Program).Assembly));
//ServerHost host = new ServerHost(new StringHandler("Hi"));
await host.StartAsync();