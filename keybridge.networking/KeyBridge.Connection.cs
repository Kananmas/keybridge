using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KeyBridge.Connections
{
    public class SocketServer
    {
        public IPAddress Address = IPAddress.Any;

        public int Port = 3000;

        public SocketServer() {}

        public SocketServer(int port , IPAddress address)
        {
            this.Port = port;
            if(Address != null)
            {
                this.Address = IPAddress.Any;
            }
        }


        TcpListener? listener;

        public async Task<TcpClient> Host()
        {
            listener = new TcpListener(Address , Port);
            Console.WriteLine($"Application Started on {Address.ToString()}:{Port}");

            listener.Start();

            return await listener.AcceptTcpClientAsync();
        }


        public void Stop()
        {
            if(listener == null) return;
            listener.Stop();
        }
    }


    public class SocketClient()
    {
        public string Server="192.168.1.100";
        public int Port = 4000;


        TcpClient? tcpClient;



        public async Task Connect()
        {
            tcpClient = new();
            await tcpClient.ConnectAsync(Server , Port);
        }


        public async Task Send(string data)
        {
            if(string.IsNullOrEmpty(data) || tcpClient == null) return;

            var bytes = Encoding.UTF8.GetBytes(data);
            var stream = tcpClient.GetStream();

            await stream.WriteAsync(bytes);
        }

        public async Task Disconnect()
        {
            if(tcpClient == null) return;
            tcpClient?.Dispose();
        }
    }
}