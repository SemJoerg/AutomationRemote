using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationEngine
{
    public struct CodeExecutionResults
    {
        public readonly bool Succesful;

        public readonly string? Instruction;

        public readonly int? PositionOfInstruction;

        public CodeExecutionResults()
        {
            Succesful = true;
            Instruction = null;
            PositionOfInstruction = null;
        }

        public CodeExecutionResults(string? unsuccesfulInstruction, int? positionOfInstruction)
        {
            Succesful = false;
            Instruction = unsuccesfulInstruction;
            PositionOfInstruction = positionOfInstruction;
        }
    }
}
