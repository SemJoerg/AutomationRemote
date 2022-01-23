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
            Name = "";
            InstructionsString = "";
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

    }
}
