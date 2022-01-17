using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AutomationRemote
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<Toast>().ShowLong("Connected Clicked");
            Navigation.PushModalAsync(new ExecuteCommandsPage(), true);
        }
        private void tbxIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = sender as Entry;
            Regex regex = new Regex("[^1-9.]+");

            if (e.NewTextValue != null && regex.IsMatch(e.NewTextValue))
            {
                entry.Text = e.OldTextValue;
            }
        }

        private void tbxPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = sender as Entry;
            Regex regex = new Regex("[^1-9]+");

            if (e.NewTextValue != null && regex.IsMatch(e.NewTextValue))
            {
                entry.Text = e.OldTextValue;
            }
        }
    }
}
