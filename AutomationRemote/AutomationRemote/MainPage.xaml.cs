using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using Xamarin.Forms;

namespace AutomationRemote
{
    public partial class MainPage : ContentPage
    {
        TcpClient tcpClient;
        
        public MainPage()
        {
            InitializeComponent();
            tcpClient = new TcpClient();
            tcpClient.TcpClientStateChanged += TcpClientStateChanged;
        }

        private void TcpClientStateChanged(object sender, EventArgs e)
        {
            switch(tcpClient.ClientState)
            {
                case TcpClientState.Connecting:
                    btnConnect.IsEnabled = false;
                    SendToast("Connecting");
                    break;
                case TcpClientState.Connected:
                    Navigation.PushModalAsync(new ExecuteCommandsPage(tcpClient), true);
                    break;
                case TcpClientState.Disconnected:
                    btnConnect.IsEnabled = true;
                    SendToast("Disconnected");
                    
                    break;
            }
        }

        private void SendToast(string msg)
        {
            DependencyService.Get<Toast>().ShowShort(msg);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            IPEndPoint endPoint = null;

            try
            {
                IPAddress iPAddress;
                bool CorrectIp = IPAddress.TryParse(tbxIp.Text, out iPAddress);

                if(!CorrectIp)
                {
                    SendToast("Invalid IP-Address");
                    return;
                }

                endPoint = new IPEndPoint(iPAddress, Convert.ToInt32(tbxPort.Text));
            }
            catch (Exception ex)
            {
                SendToast("Invalid IP-Address or Port");
                return;
            }

            tcpClient.RunAsync(endPoint);
            
        }
        private void tbxIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = sender as Entry;
            Regex regex = new Regex("[^0-9.]+");

            if (e.NewTextValue != null && regex.IsMatch(e.NewTextValue))
            {
                entry.Text = e.OldTextValue;
            }
        }

        private void tbxPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = sender as Entry;
            Regex regex = new Regex("[^0-9]+");

            if (e.NewTextValue != null && regex.IsMatch(e.NewTextValue))
            {
                entry.Text = e.OldTextValue;
            }
        }
    }
}
