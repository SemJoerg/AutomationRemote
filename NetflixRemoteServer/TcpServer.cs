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

    public delegate void TcpServerInfoEventHandler (TcpServer tcpServer, string message);

    public class TcpServer
    {
        private IPAddress ipAddress;
        public int Port { get; set; }

        public string CurrentTcpServerInfo { get; private set; }
        
        public event TcpServerInfoEventHandler TcpServerInfo;

        public event EventHandler ServerStateChanged;
        private TcpServerState _serverState;
        public TcpServerState ServerState
        {
            get { return _serverState; }
            private set 
            { 
                if(value != _serverState)
                {
                    if(_serverState == TcpServerState.Stopping && value == TcpServerState.Stopped)
                    {
                        _serverState = value;
                        FireTcpServerInfoEvent("");
                        ServerStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    else if(_serverState != TcpServerState.Stopping)
                    {
                        _serverState = value;
                        ServerStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    
                }
            }
        }

        public TcpServer(int port)
        {
            ServerState = TcpServerState.Stopped;
            ipAddress = IPAddress.Any; //new IPAddress(new byte[] { 127, 0, 0, 1 });
            Port = port;
        }
        
        public void Stop()
        {
            ServerState = TcpServerState.Stopping;
        }

        public  bool Run()
        {
            if(ServerState !=TcpServerState.Stopped)
            {
                return false;
            }
            
            ServerState = TcpServerState.Running;
            Socket listenerSocket = null;
            string currendCommand = "";
            Socket handler = null;

            while(ServerState != TcpServerState.Stopping)
            {
                FireTcpServerInfoEvent("No client connected");
                listenerSocket = InitTcpSocket();
                Task<Socket> acceptSocketTask = listenerSocket.AcceptAsync();
                while (ServerState != TcpServerState.Stopping)
                {
                    if (acceptSocketTask.IsCompleted)
                    {
                        listenerSocket.Close();
                        listenerSocket = null;
                        break;
                    }
                    Thread.Sleep(50);
                }

                if (ServerState != TcpServerState.Stopping)
                {
                    handler = acceptSocketTask.Result;
                    FireTcpServerInfoEvent($"Connected with: {handler.RemoteEndPoint}");
                    SendCommandList(handler);
                }

                while (ServerState != TcpServerState.Stopping && IsSocketConnected(handler))
                {
                    ListenForCommands(handler, ref currendCommand);
                }

                if (handler != null)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    handler = null;
                }
            }

            if(listenerSocket != null)
            {
                listenerSocket.Close();
            }
            ServerState = TcpServerState.Stopped;
            return true;
        }

        public async Task RunAsync()
        {
            Task task = new Task(() =>
            {
                Run();
            });

            task.Start();

            await task;
        }

        bool IsSocketConnected(Socket socket)
        {
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if ((part1 && part2) || !socket.Connected)
                return false;
            else
                return true;
        }

        private Socket InitTcpSocket()
        {

            Socket tcpSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(new IPEndPoint(ipAddress, Port));
            tcpSocket.Listen(10);
            return tcpSocket;
        }

        private void SendCommandList(Socket socket)
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

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());

            socket.Send(buffer);
        }

        private void ListenForCommands(Socket socket, ref string currentCommand)
        {
            if (socket.Available == 0)
            {
                Thread.Sleep(50);
                return;
            }

            try
            {
                byte[] buffer = new byte[socket.Available];
                socket.Receive(buffer);
                string rawData = Encoding.UTF8.GetString(buffer);

                foreach (char letter in rawData)
                {
                    switch (letter)
                    {
                        case '<':
                            currentCommand = "";
                            break;
                        case '>':
                            int commandIndex = Convert.ToInt32(currentCommand);
                            AutomationEngine.CodeEngine.ExecuteCode(Program.commandsList[commandIndex].InstructionsString);
                            currentCommand = "";
                            break;
                        default:
                            currentCommand += letter;
                            break;
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void FireTcpServerInfoEvent(string info)
        {
            CurrentTcpServerInfo = info;
            TcpServerInfo?.Invoke(this, info);
        }
    }
}
