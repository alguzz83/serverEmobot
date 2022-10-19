using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace csharp_server
{

    public class Echo : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Recibiendo: " + e.Data);
            Send(e.Data);
        }
    }

    public class EchoAll : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Recibiendo All: " + e.Data);
            Sessions.Broadcast(e.Data);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            WebSocketServer server = new WebSocketServer("ws://198.251.71.3:7890");

            server.AddWebSocketService<Echo>("/Echo");
            server.AddWebSocketService<EchoAll>("/EchoAll");

            server.KeepClean = false;
            server.Start();
            Console.WriteLine("Server iniciado: ws://198.251.71.3:7890/Echo");
            Console.WriteLine("Server iniciado: ws://198.251.71.3:7890/EchoAll");

            Console.ReadKey();
            //server.Stop();
        }
    }
}
