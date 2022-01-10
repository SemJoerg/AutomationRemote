using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationEngine
{
    public struct AutomationInstruction
    {
        public readonly string RawInstruction;
        public readonly string[] InstructionSegments;

        public AutomationInstruction(string instruction)
        {
            RawInstruction = instruction;

            InstructionSegments = GetInstructionSegments(RawInstruction);
        }

        public static string[] GetInstructionSegments(string instruction)
        {
            List<string> instructionSegments = new List<string>();
            string[] rawInstructionSegments = instruction.Split(' ');

            foreach (string segment in rawInstructionSegments)
            {
                if (!String.IsNullOrEmpty(segment))
                {
                    instructionSegments.Add(segment);
                }
            }

            return instructionSegments.ToArray();
        }
    }
}
