using HttpServer;

ServerHost host = new ServerHost(new StringHandler("Hello! I'm test server"));
host.Start();