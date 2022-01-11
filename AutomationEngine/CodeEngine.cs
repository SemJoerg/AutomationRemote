namespace AutomationEngine
{   
    public static class CodeEngine
    {

        static readonly List<AutomationCommand> automationCommands;
        
        static CodeEngine()
        {
            //Command: mouse
            AutomationCommandLogic mouseLogic = (args) => {return AutomationCommandResults.Unsuccessful;};
            AutomationCommand mouseCommand = new AutomationCommand("mouse", mouseLogic);

            //SubCommand: mouse move
            AutomationCommandLogic mouseMoveLogic = (args) =>
            {       
                try
                {
                    if (args.Length == 2)
                    {
                        int x = Convert.ToInt32(args[0]);
                        int y = Convert.ToInt32(args[1]);

                        VirtualInput.SetCursorPos(x, y);
                        return AutomationCommandResults.Succesful;
                    }
                }
                catch (Exception ex){}
                    

                return AutomationCommandResults.Unsuccessful;
            };
            AutomationCommand mouseMoveCommand = new AutomationCommand("move", mouseMoveLogic);
            mouseCommand.SubCommands.Add(mouseMoveCommand);

            //SubCommand: mouse click
            AutomationCommandLogic mouseClickCommandLogic = (args) => 
            {
                try
                {
                    if (args.Length == 1)
                    {
                        switch (args[0])
                        {
                            case "left":
                                VirtualInput.LeftClick();
                                break;
                            case "right":
                                VirtualInput.RightClick();
                                break;
                            case "middle":
                                VirtualInput.MiddleClick();
                                break;
                            default:
                                return AutomationCommandResults.Unsuccessful;
                                break;
                        }

                        return AutomationCommandResults.Succesful;
                    }
                }
                catch (Exception ex) { }
                return AutomationCommandResults.Unsuccessful;
            };
            AutomationCommand mouseClickCommand = new AutomationCommand("click", mouseClickCommandLogic);
            mouseCommand.SubCommands.Add(mouseClickCommand);

            //Command: delay
            AutomationCommandLogic delayLogic = (args) =>
            {
                try
                {
                    if(args.Length == 1)
                    {
                        Thread.Sleep(Convert.ToInt32(args[0]));
                        return AutomationCommandResults.Succesful;
                    }
                }
                catch(Exception ex){}
                return AutomationCommandResults.Unsuccessful;
            };
            AutomationCommand delayCommand = new AutomationCommand("delay", delayLogic);


            automationCommands = new List<AutomationCommand> { mouseCommand, delayCommand };
        }
        
        static public CodeExecutionResults ExecuteCode(string code)
        {
            AutomationInstruction[] instructions = CodeToInstructionsArray(code);

            for (int i = 0; i < instructions.Length; i++)
            {
                AutomationInstruction instruction = instructions[i];
                AutomationCommandResults result = ExecuteInstruction(instruction);

                if (result != AutomationCommandResults.Succesful)
                {
                    return new CodeExecutionResults(instruction.RawInstruction, i + 1);
                }
            }

            return new CodeExecutionResults();
        }

        static public async Task<CodeExecutionResults> ExecuteCodeAsync(string code)
        {
            Task<CodeExecutionResults> executeCodeTask = Task.Run(() =>
            {
                return ExecuteCode(code);
            });

            return await executeCodeTask;
        }

        static public AutomationInstruction[] CodeToInstructionsArray(string code)
        {
            string cleanedCode = code.ReplaceLineEndings("\n");
            cleanedCode = cleanedCode.Replace("\n", null);
            if (cleanedCode[cleanedCode.Length - 1] == ';')
                cleanedCode = cleanedCode.Substring(0, cleanedCode.Length - 1);

            string[] rawInstructions = cleanedCode.Split(';');
            AutomationInstruction[] instructions = new AutomationInstruction[rawInstructions.Length];

            for (int i = 0; i <  rawInstructions.Length; i++)
            {
                instructions[i] = new AutomationInstruction(rawInstructions[i]);
            }

            return instructions;
        }

        static public AutomationCommandResults ExecuteInstruction(AutomationInstruction instruction)
        {
            foreach(AutomationCommand command in automationCommands)
            {
                AutomationCommandResults results = command.TryExecute(instruction.InstructionSegments);

                if (results != AutomationCommandResults.WrongCommand)
                    return results;
            }

            return AutomationCommandResults.WrongCommand;
        }
    }
}