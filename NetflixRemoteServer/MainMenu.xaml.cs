using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutomationEngine;

namespace NetflixRemoteServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        
        private ObservableCollection<Command> commands = Program.commandsList;
        private Thread mouseTrackerThread;
        private bool mouseTrackerThredKeepRunning = true;
        private TcpServer tcpServer;

        private bool scriptingToolsEnabled;
        private bool ScriptingToolsEnabled
        {
            get { return scriptingToolsEnabled; }
            set
            {
                scriptingToolsEnabled = value;

                btnAdd.IsEnabled = value;
                btnSave.IsEnabled = value;
                commandsControl.IsEnabled = value;
            }
        }

        public MainMenu()
        {
            InitializeComponent();
            commandsControl.ItemsSource = commands;
            mouseTrackerThread = new Thread(new ThreadStart(TrackMouse));
            mouseTrackerThread.Start();
            tcpServer = Program.TcpServer;
            tbxPort.Text = Convert.ToString(tcpServer.Port);
            tcpServer.ServerStateChanged += TcpServer_ServerStateChanged;
            tcpServer.TcpServerInfo += TcpServer_TcpServerInfo;
            TcpServer_ServerStateChanged(null, null);
            tbServerInfo.Text = tcpServer.CurrentTcpServerInfo;
        }

        private void TcpServer_TcpServerInfo(TcpServer tcpServer, string message)
        {
            Dispatcher.Invoke(() =>
            {
                tbServerInfo.Text = message;
            });
        }

        private void TcpServer_ServerStateChanged(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch (tcpServer.ServerState)
                {
                    case TcpServerState.Running:
                        tbxPort.IsEnabled = false;
                        ScriptingToolsEnabled = false;
                        btnStartStop.Content = "Stop";
                        break;
                    case TcpServerState.Stopped:
                        btnStartStop.IsEnabled = true;
                        tbxPort.IsEnabled = true;
                        ScriptingToolsEnabled = true;

                        btnStartStop.Content = "Start";
                        break;
                    case TcpServerState.Stopping:
                        btnStartStop.IsEnabled = false;
                        btnStartStop.Content = "Stopping";
                        break;
                }
            });
        }

        private void TrackMouse()
        {
            try
            {
                
                System.Drawing.Point mousePosition = new System.Drawing.Point(0, 0);
                System.Drawing.Point lastMousePosition;
                while (mouseTrackerThredKeepRunning)
                {
                    lastMousePosition = mousePosition;
                    VirtualInput.GetCursorPos(ref mousePosition);

                    if(lastMousePosition != mousePosition)
                    {
                        Dispatcher.Invoke(() => { lblMousePosition.Content = $"x: {mousePosition.X}  y: {mousePosition.Y}"; });
                    }
                    
                    Thread.Sleep(200);
                }
            }
            catch(ThreadAbortException ex)
            { }

        }

        private void BtnStartStopServerClick(object sender, RoutedEventArgs e)
        {
            if(tcpServer.ServerState == TcpServerState.Stopped)
            {
                tcpServer.Port = Convert.ToInt32(tbxPort.Text);
                tcpServer.RunAsync();
            }
            else if(tcpServer.ServerState == TcpServerState.Running)
            {
                tcpServer.Stop();
            }
        }

        private void BtnAddClick(object sender, RoutedEventArgs e)
        {
            commands.Add(new Command());
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            Program.SaveDialog();
        }

        private void BtnCommandRemoveClick(object sender, RoutedEventArgs e)
        {
            Command command = (sender as Button).DataContext as Command;
            commands.Remove(command);
        }

        private void BtnCommandMoveRightClick(object sender, RoutedEventArgs e)
        {
            Command command = (sender as Button).DataContext as Command;

            int index = commands.IndexOf(command);
            if(index + 1 < commands.Count)
            {
                commands.Move(index, index + 1);
            }
            Program.SavedCommands = false;
        }

        private void BtnCommandMoveLeftClick(object sender, RoutedEventArgs e)
        {
            Command command = (sender as Button).DataContext as Command;

            int index = commands.IndexOf(command);
            if(index > 0)
            {
                commands.Move(index, index - 1);
            }
            Program.SavedCommands = false;
        }

        private async void BtnCommandRunClick(object sender, RoutedEventArgs e)
        {
            Command command = (sender as Button).DataContext as Command;
            ScriptingToolsEnabled = false;
            btnStartStop.IsEnabled = false;

            WindowState = WindowState.Minimized;
            CodeExecutionResults results = await CodeEngine.ExecuteCodeAsync(command.InstructionsString);
            ScriptingToolsEnabled = true;
            btnStartStop.IsEnabled = true;
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                Activate();
            }
            
            if (!results.Succesful)
            {
                Program.ErrorMessage($"Code Executed Unsuccesful\n\nError in {results.PositionOfInstruction}th Instruction\n'{results.Instruction}'");
            }
        }

        private void TbxPortPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TbxCommandPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[<|>]+");

            e.Handled = regex.IsMatch(e.Text);

            if (!e.Handled)
            {
                Program.SavedCommands = false;
            }
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if(Program.CheckIfSaved() == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            mouseTrackerThredKeepRunning = false;
        }

        
    }
}
