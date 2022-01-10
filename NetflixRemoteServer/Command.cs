using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationEngine;

namespace NetflixRemoteServer
{
    public class Command
    {
        public string Name { get; set; }

        public string InstructionsString { get; set; }

        public AutomationInstruction[] InstructionsArray 
        { 
            get 
            {
                return CodeEngine.CodeToInstructionsArray(InstructionsString);
            }
        }

        public bool IsEnabled { get; set; }
        
        public Command()
        {
            IsEnabled = true;
        }

        public Command(string name) : this()
        {
            Name = name;
        }
        
        public Command(string name, string instructionsString) : this(name)
        {
            InstructionsString = instructionsString;
        }
        
        public Command(string name, string instructionsString, bool isEnabled) : this(name, instructionsString)
        {
            IsEnabled = isEnabled;
        }

        /*
        public void Execute()
        {
            foreach (string instruction in InstructionsArray)
            {
                ExecuteInstruction(GetInstructionSegments(instruction));
            }
        }

        private static string[] GetInstructionSegments(string instruction)
        {
            List<string> instructionSegments = new List<string>();
            string[] rawInstructionSegments = instruction.Split(' ');

            foreach (string segment in rawInstructionSegments)
            {
                if(!String.IsNullOrEmpty(segment))
                {
                    instructionSegments.Add(segment);
                }
            }

            return instructionSegments.ToArray();
        }
        
        private static void ExecuteInstruction(string[] instruction)
        {

            string output = "";
            for(int i = 0; i < instruction.Length; i++)
            {
                output += $"[{i}] ";
                output += instruction[i] + "\n";
            }

            System.Windows.MessageBox.Show(output);
        }
        */
    }
}
