using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

        public MainMenu()
        {
            InitializeComponent();
            commandsControl.ItemsSource = commands;
            mouseTrackerThread = new Thread(new ThreadStart(TrackMouse));
            mouseTrackerThread.Start();
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
                    
                    Thread.Sleep(50);
                }
            }
            catch(ThreadAbortException ex)
            { }

        }

        private void BtnStartStopServerClick(object sender, RoutedEventArgs e)
        {
            CodeExecutionResults results = CodeEngine.ExecuteCode(commands[0].InstructionsString);

            if(results.Succesful)
            {
                MessageBox.Show("Code Executed Succesful");
            }
            else
            {
                Program.ErrorMessage($"Code Executed Unsuccesful\n\nError in {results.PositionOfInstruction}th Instruction\n'{results.Instruction}'");
            }

            return;

            foreach (AutomationInstruction instruction in commands[0].InstructionsArray)
            {
                string output = "";
                for (int i = 0; i < instruction.InstructionSegments.Length; i++)
                {
                    output += $"[{i}] ";
                    output += instruction.InstructionSegments[i] + "\n";
                }
                MessageBox.Show(output);
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

        private void TBCommandChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            foreach(TextChange change in e.Changes)
            {
                if(change.AddedLength > 0)
                {
                    foreach(char letter in "<|>")
                    {
                        if(letter == tb.Text[change.Offset])
                        {
                            tb.Text = tb.Text.Remove(change.Offset, 1);
                            tb.SelectionStart = change.Offset;
                            tb.SelectionLength = 0;
                            break;
                        }
                    }
                }
            }

            Program.SavedCommands = false;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Program.CheckIfSaved() == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            mouseTrackerThredKeepRunning = false;
        }
    }
}
