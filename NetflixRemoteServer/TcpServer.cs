using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace NetflixRemoteServer
{
    public enum TcpServerState
    {
        Stopped,
        Stopping,
        Running
    }
    
    public class TcpServer
    {
        private TcpListener tcpListener;

        private int _port;
        public int Port
        {
            get { return _port; }
            set 
            {
                if(_port != value)
                {
                    tcpListener = new TcpListener(value);
                    _port = value;
                }
            }
        }

        public TcpServerState ServerState { get; private set; }

        public TcpServer(int port)
        {
            _port = port;
            tcpListener = new TcpListener(port);
        }

        public void Stop()
        {
            ServerState = TcpServerState.Stopping;
        }

        public bool Run()
        {
            if(ServerState !=TcpServerState.Stopped)
            {
                return false;
            }
            
            ServerState = TcpServerState.Running;
            tcpListener.Start();

            byte[] buffer = new byte[20];
            byte[] commandListInBytes = GetCommandListInBytes();
            int bytesInBuffer;

            List<int> commandsToExectue = new List<int>();
            string currendCommand = "";

            Socket socket = null;

            while (ServerState != TcpServerState.Stopping)
            {
                if(tcpListener.Pending())
                {
                    socket = tcpListener.AcceptSocket();
                    break;
                }
                Thread.Sleep(200);
            }

            while (ServerState != TcpServerState.Stopping)
            {
                socket.Send(commandListInBytes);

                try
                {
                    bytesInBuffer = socket.Receive(buffer);
                    string rawData = Encoding.UTF8.GetString(buffer, 0, bytesInBuffer);

                    foreach (char letter in rawData)
                    {
                        switch (letter)
                        {
                            case '<':
                                currendCommand = "";
                                break;
                            case '>':
                                commandsToExectue.Add(Convert.ToInt32(currendCommand));
                                break;
                            default:
                                currendCommand += letter;
                                break;
                        }
                    }

                    foreach (int command in commandsToExectue)
                    {
                        AutomationEngine.CodeEngine.ExecuteCode(Program.commandsList[command].InstructionsString);
                    }
                }
                catch (Exception ex) { }
                
            }

            socket.Close();
            tcpListener.Stop();
            ServerState = TcpServerState.Stopped;
            return true;
        }

        private byte[] GetCommandListInBytes()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<");
            for(int i = 0; i < Program.commandsList.Count; i++)
            {
                sb.Append(Program.commandsList[i].Name);

                if(i + 1 < Program.commandsList.Count)
                {
                    sb.Append('|');
                }
            }
            sb.Append(">");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

    }
}
