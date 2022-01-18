using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutomationRemote
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExecuteCommandsPage : ContentPage
    {
        private TcpClient tcpClient;
        private EventHandler tcpClientStateChangedHandler;
        private ObservableCollection<string> commandsList;
        private bool closing = false;

        public ExecuteCommandsPage(TcpClient _tcpClient)
        {
            InitializeComponent();
            commandsList = new ObservableCollection<string>();
            listViewCommands.ItemsSource = commandsList;
            tcpClient = _tcpClient;
            tcpClientStateChangedHandler = TcpClientStateChanged;
            tcpClient.TcpClientStateChanged += tcpClientStateChangedHandler;
            tcpClient.RecievedNewCommands += TcpClient_RecievedNewCommands;
            Disappearing += ExecuteCommandsPage_Disappearing;
        }

        private void ExecuteCommandsPage_Disappearing(object sender, EventArgs e)
        {
            closing = true;
            tcpClient.Disconnect();
        }

        private void TcpClient_RecievedNewCommands(object sender, string[] commands)
        {
            foreach(string command in commands)
            {
                commandsList.Add(command);
            }
        }

        private void TcpClientStateChanged(object sender, EventArgs e)
        {
            if(closing)
                return;
            
            if(tcpClient.ClientState == TcpClientState.Disconnecting || tcpClient.ClientState == TcpClientState.Disconnected)
            {
                Navigation.PopModalAsync();
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            string command = (sender as Button).BindingContext as string;

            int index = commandsList.IndexOf(command);

            if(index != -1)
            {
                tcpClient.SendCommand(index);
            }
        }
    }
}