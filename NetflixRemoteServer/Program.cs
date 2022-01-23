using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NetflixRemoteServer
{
    static class Program
    {
        static public App app;

        static public string AppStatus
        {
            get { return app.notifyIcon.Text; }
            set { app.notifyIcon.Text = value; }
        }

        static public bool SavedCommands { get; set; }

        static public string CommandsPath { get { return Environment.CurrentDirectory + @"\commands.ini"; } }

        static public ObservableCollection<Command> commandsList;

        static public TcpServer TcpServer { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            bool hideApp = false;
            bool startTcpServer = false;
            int port = 9000;

            if(args.Length > 0)
            {
                for(int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];
                    switch (arg)
                    {
                        case "--hidden":
                            hideApp = true;
                            break;
                        case "--start":
                            int tempPort = 0;
                            
                            if (args.Length >= i + 2)
                            {
                                
                                bool changePort = Int32.TryParse(args[i + 1], out tempPort);
                                if (!changePort)
                                {
                                    MessageBox.Show("Unable to convert port");
                                    return;
                                }

                                port = tempPort;
                            }

                            startTcpServer = true;
                            break;
                    }
                }
            }

            using(Mutex mutex = new Mutex(false, @"AutomationServer"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Instance already running");
                    return;
                }

                commandsList = new ObservableCollection<Command>();
                LoadCommands(ref commandsList);
                SavedCommands = true;
                TcpServer = new TcpServer(port);
                if(startTcpServer)
                {
                    TcpServer.RunAsync();
                }
                app = new App(hideApp);
                app.Run();
            }
        }

        public static void ErrorMessage(object message)
        {
            MessageBox.Show(message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        static public void SaveDialog()
        {
            if (SaveCommands(commandsList))
            {
                SavedCommands = true;
                MessageBox.Show("Saved", "Saving Dialog");
            }
        }

        static public MessageBoxResult? CheckIfSaved()
        {
            if (!SavedCommands)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save?", "Saving Dialog", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);

                if (result == MessageBoxResult.Yes)
                {
                    SaveDialog();
                }

                return result;
            }
            return null;
        }

        static public bool SaveCommands(IEnumerable<Command> commands, string path = null)
        {
            try
            {
                if (path == null)
                {
                    path = CommandsPath;
                }

                string output = "";
                foreach (Command command in commands)
                {
                    output += $"<{command.Name}|{command.IsEnabled}|\n";
                    output += $"{command.InstructionsString.Trim(new char[] { '\n', '\r', ' ' })}\n>\n";
                }

                if(!File.Exists(CommandsPath))
                {
                    File.Create(CommandsPath).Close();
                }

                using (StreamWriter sw = new StreamWriter(CommandsPath, false))
                {
                    sw.Write(output);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
                return false;
            }

            return true;
        }

        static public bool LoadCommands(ref ObservableCollection<Command> commands, string path = null)
        {
            try
            {
                if (path == null)
                {
                    path = CommandsPath;
                }

                if (!File.Exists(path))
                {
                    return true;
                }

                using (StreamReader sr = new StreamReader(path))
                {
                    StringBuilder sb = new StringBuilder();
                    while (sr.Peek() >= 0)
                    {
                        char letter = (char)sr.Read();
                        switch (letter)
                        {
                            case '<':
                                sb.Clear();
                                break;

                            case '>':
                                commands.Add(GetCommandFromString(sb.ToString()));
                                break;

                            default:
                                sb.Append(letter);
                                break;

                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message);
                return false;
            }
            return true;
        }


        static private Command GetCommandFromString(string commandString)
        {
            string[] commandArray = commandString.Split('|');
            commandArray[2] = commandArray[2].Trim(new char[]{ '\n', '\r', ' '});
            return new Command(commandArray[0], commandArray[2], Convert.ToBoolean(commandArray[1]));
        }
    }
}
