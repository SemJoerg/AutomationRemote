using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutomationEngine
{
    public enum AutomationCommandResults
    {
        WrongCommand,
        Succesful,
        Unsuccessful
    }

    public delegate AutomationCommandResults AutomationCommandLogic(string[] parameter);

    public class AutomationCommand
    {
        public string CommandDescription { get; set; }

        public readonly string CommandKeyWord;

        public readonly AutomationCommandLogic CommandLogic;
        public List<AutomationCommand> SubCommands { get; set; }


        public AutomationCommand(string commandKeyword, AutomationCommandLogic commandLogic)
        {
            SubCommands = new List<AutomationCommand>();
            CommandKeyWord = commandKeyword;
            CommandLogic = commandLogic;
        }

        public AutomationCommandResults TryExecute(string[] instructionSegments)
        {

            if (instructionSegments[0] != CommandKeyWord)
                return AutomationCommandResults.WrongCommand;

            foreach(AutomationCommand subCommand in SubCommands)
            {
                AutomationCommandResults results = subCommand.TryExecute(instructionSegments.GetSegment(1, instructionSegments.Length -1));

                if (results != AutomationCommandResults.WrongCommand)
                    return results;
            }

            return CommandLogic(instructionSegments.GetSegment(1, instructionSegments.Length - 1));
        }
    }
}
