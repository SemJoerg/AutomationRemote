using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Diagnostics;

namespace NetflixRemoteServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {   
        
        public Forms.NotifyIcon notifyIcon;

        public App() : base() { InitializeComponent(); }
        
        public App(bool sartupInisible) : base()
        {   
            InitializeComponent();
            MainWindow = new MainMenu();
            notifyIcon = new Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("n.ico");
            notifyIcon.MouseClick += NotifyIcon_Click;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, NotifyIcon_ExitClick);
            notifyIcon.Visible = true;
            
            if(sartupInisible)
            {
                notifyIcon.BalloonTipTitle = "Started NetflixRemote hidden";
                notifyIcon.ShowBalloonTip(0, "Started Netflix-Remote", " ", Forms.ToolTipIcon.Info);
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void NotifyIcon_Click(object? sender, Forms.MouseEventArgs e)
        {
            if (e.Button != Forms.MouseButtons.Left)
                return;
            
            if (MainWindow == null)
                MainWindow = new MainMenu();
            
            MainWindow.Show();
            MainWindow.Activate();
        }

        private void NotifyIcon_ExitClick(object? sender, EventArgs e)
        {
            if(Windows.Count > 0)
            {
                MainWindow.Close();
            }
            else if (Program.CheckIfSaved() == MessageBoxResult.Cancel)
            {
                return;
            }

            if(Windows.Count == 0)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
                Shutdown();
            }
        }
    }
}
