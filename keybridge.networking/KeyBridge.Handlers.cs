using System.Net;
using System.Text;
using KeyBridge.Connections;
using KeyBridge.InputReading;
using SharpHook;

public static class KeyBridgeHandler
{

    static SimpleGlobalHook hook = new();

    static SocketClient? client;

    static void Init()
    {
        hook.KeyPressed += async (_ , e) =>
        {
            var keyChar = e.Data.KeyChar;
            Console.WriteLine(keyChar);
            var keyPress = new KeyPress
            {
                KeyChar = keyChar,
            };

            if(client != null)
            {
                await client.Send(keyPress.ToJson());
            }
        };
    }
    

    public static async Task ServerHandler()
    {
        Console.WriteLine("Please Enter Your Server IP:(Press Enter To Pass)");
        string serverIPStr = Console.ReadLine();
        if (string.IsNullOrEmpty(serverIPStr))
        {
            serverIPStr = IPAddress.Any.ToString();
        }
        IPAddress serverIP = IPAddress.Parse(serverIPStr);
        Console.WriteLine("Please Enter Desired Port:(Press Enter To Pass)");
        string port = Console.ReadLine();
        if (string.IsNullOrEmpty(port))
        {
            port = "3000";
        }
        var server = new SocketServer(int.Parse(port), serverIP);
        var client = await server.Host();
        while (true)
        {
            try
            {
                if (client != null)
                {
                    var stream = client.GetStream();
                    var bytes = new byte[2048];
                    int byteRead;

                    while ((byteRead = await stream.ReadAsync(bytes)) > 0)
                    {
                        string msg = Encoding.UTF8.GetString(bytes);
                        var press = KeyPress.Parse(msg);
                        InputEmitter.Emit(press);

                        Console.WriteLine("Recived Key Event:" + msg);
                    }
                }
            }
            catch (Exception E)
            {

                Console.WriteLine(E.Message);
                break;
            }
        }

        server.Stop();
        client.Dispose();
    }



    public static async Task ClientHandler()
    {
        var client = new SocketClient();

        Console.WriteLine("Enter the server ip address");
        var ipAddress = Console.ReadLine();
        if (string.IsNullOrEmpty(ipAddress)) ipAddress = IPAddress.Any.ToString();
        client.Server = ipAddress;

        Console.WriteLine("Enter the server port");
        var port = Console.ReadLine();
        if (string.IsNullOrEmpty(port)) port = "3000";
        client.Port = int.Parse(port);

        await client.Connect();

        KeyBridgeHandler.client = client;
        Init();
        hook.Run();

        while(hook.IsRunning)
        {
        }
        
        hook.Stop();
        hook.Dispose();
        await client.Disconnect();
    }
}