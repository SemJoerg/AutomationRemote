using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Xamarin.Forms;

namespace AutomationRemote
{
    public enum TcpClientState
    {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected
    }


    public delegate void RecievedNewCommandsEventHandler(object sender, string[] commands);
    
    public class TcpClient
    {
        public event RecievedNewCommandsEventHandler RecievedNewCommands;

        public event EventHandler TcpClientStateChanged;

        private TcpClientState clientState;
        public TcpClientState ClientState 
        {
            get { return clientState; }
            private set 
            {
                if(value != clientState)
                {
                    if(clientState == TcpClientState.Disconnecting && value != TcpClientState.Disconnected)
                    {
                        return;
                    }

                    clientState = value;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TcpClientStateChanged?.Invoke(this, null);
                    });
                    
                }
            }
        }

        private Queue<byte[]> commandsQueue;

        public TcpClient()
        {
            commandsQueue = new Queue<byte[]>();
            ClientState = TcpClientState.Disconnected;
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

        private void Run(EndPoint endPoint)
        {
            ClientState = TcpClientState.Connecting;
            Socket client = null;
            try
            {
                client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.SendTimeout = 2000;
                client.ReceiveTimeout = 2000;
                client.Connect(endPoint);
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DependencyService.Get<Toast>().ShowLong(ex.Message);
                });
                ClientState = TcpClientState.Disconnecting;
            }

            if(ClientState != TcpClientState.Disconnecting)
                ClientState = TcpClientState.Connected;

            string rawData = "";
            int firstIndex, secondIndex;
            byte[] buffer;
            

            try
            {
                while (ClientState != TcpClientState.Disconnecting && IsSocketConnected(client))
                {
                    if (client.Available == 0)
                    {
                        Thread.Sleep(50);
                        continue;
                    }

                    buffer = new byte[client.Available];
                    client.Receive(buffer);
                    rawData += Encoding.UTF8.GetString(buffer);

                    firstIndex = rawData.IndexOf('<');
                    secondIndex = rawData.IndexOf('>');

                    if (firstIndex != -1 && secondIndex != -1)
                    {
                        string[] commands = rawData.Substring(firstIndex + 1, secondIndex - 1).Split('|');
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            RecievedNewCommands?.Invoke(this, commands);
                        });
                        break;
                    }
                }

                while (ClientState != TcpClientState.Disconnecting && IsSocketConnected(client))
                {
                    if (commandsQueue.Count == 0)
                    {
                        Thread.Sleep(20);
                        continue;
                    }

                    while(commandsQueue.Count > 0)
                    {
                        client.Send(commandsQueue.Dequeue());
                    }
                }
            }
            catch(Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DependencyService.Get<Toast>().ShowLong(ex.Message);
                });
            }



            if (client != null)
            {
                if(client.Connected)
                    client.Shutdown(SocketShutdown.Both);

                client.Close();
            }
            ClientState = TcpClientState.Disconnected;
        }

        public async Task RunAsync(EndPoint endPoint)
        {
            Task task = Task.Run(() =>
            {
                Run(endPoint);
            });

            await task;
        }

        public void Disconnect()
        {
            ClientState = TcpClientState.Disconnecting;
        }

        public void SendCommand(int commandIndex)
        {
            commandsQueue.Enqueue(Encoding.UTF8.GetBytes($"<{commandIndex}>"));
        }

    }
}
