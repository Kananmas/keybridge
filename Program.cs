using System.Net;
using System.Text;
using KeyBridge.Connections;

Console.WriteLine("Welcome To Keyboard bridge");

Console.WriteLine("the application launches in two modes first");
Console.WriteLine("1-Reciver: Meaning the device that is going to accept your keyboard input");
Console.WriteLine("2-Sender: Meaning the device which sends input to reciver");
Console.WriteLine("");


Console.WriteLine("Select Your desired Mode:");
var mode = Console.ReadKey();
Console.WriteLine();

switch(mode.KeyChar)
{
    case '1':
        {
            await KeyBridgeHandler.ServerHandler();
            break;
        }
    case '2':
        {
            await KeyBridgeHandler.ClientHandler();
            break;
        }
}