using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace csharp_server
{

    public class Echo : WebSocketBehavior
    {
        string[] splitData;
        string htcpId = "";
        string userId = "";
        char chara = '|';
        bool activeHtcp = false;
        bool activeUser = false;
        static string applicationPath = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory);
        string saveFilePath = Path.Combine(applicationPath, "config.xml");
        protected override void OnMessage(MessageEventArgs e)
        {
            using (XmlReader reader = XmlReader.Create(saveFilePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "htcpId":
                                htcpId = reader.ReadString();
                                break;
                            case "userId":
                                userId = reader.ReadString();
                                break;
                            case "activeHtcp":
                                if (reader.ReadString() == "false")
                                {
                                    activeHtcp = false;
                                }
                                else
                                {
                                    activeHtcp = true;
                                }
                                break;
                            case "activeUser":
                                if (reader.ReadString() == "false")
                                {
                                    activeUser = false;
                                }
                                else
                                {
                                    activeUser = true;
                                }
                                break;
                        }
                    }
                }
            }
            Console.WriteLine("Recibiendo: " + e.Data);
            splitData = e.Data.Split(chara);
            if (activeHtcp == false)
            {
                if (splitData[0] == "#^#")
                {
                    htcpId = ID;
                    activeHtcp = true;
                    XmlTextWriter textWriter = new XmlTextWriter(saveFilePath, null);
                    // Opens the document
                    textWriter.WriteStartDocument();
                    // Write element
                    textWriter.WriteStartElement("Data");
                    textWriter.WriteStartElement("htcpId");
                    textWriter.WriteString(ID);
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("userId");
                    textWriter.WriteString(userId);
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("activeHtcp");
                    textWriter.WriteString("true");
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("activeUser");
                    if (activeUser)
                    {
                        textWriter.WriteString("true");
                    }
                    else
                    {
                        textWriter.WriteString("false");
                    }
                    textWriter.WriteEndElement();
                    textWriter.WriteEndElement();
                    // Ends the document.
                    textWriter.WriteEndDocument();
                    // close writer
                    textWriter.Close();
                }
            }
            if (activeUser == false)
            {
                if (splitData[0] == "user")
                {
                    userId = ID;
                    activeUser = true;
                    XmlTextWriter textWriter = new XmlTextWriter(saveFilePath, null);
                    // Opens the document
                    textWriter.WriteStartDocument();
                    // Write element
                    textWriter.WriteStartElement("Data");
                    textWriter.WriteStartElement("htcpId");
                    textWriter.WriteString(htcpId);
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("userId");
                    textWriter.WriteString(ID);
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("activeHtcp");
                    if (activeHtcp)
                    {
                        textWriter.WriteString("true");
                    }
                    else
                    {
                        textWriter.WriteString("false");
                    }
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("activeUser");
                    textWriter.WriteString("true");
                    textWriter.WriteEndElement();
                    textWriter.WriteEndElement();
                    // Ends the document.
                    textWriter.WriteEndDocument();
                    // close writer
                    textWriter.Close();
                }
            }
            switch (splitData[0])
            {
                case "#^#":
                    Sessions.SendTo(splitData[1], userId);
                    break;
                case "user":
                    Sessions.SendTo(splitData[1], htcpId);
                    break;
            }
            //Send(e.Data);
            //Sessions.SendTo(e.Data,ID);
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
