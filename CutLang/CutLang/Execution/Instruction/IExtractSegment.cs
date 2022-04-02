using System;

namespace CutLang.Execution.Instruction
{
    public interface IExtractSegment : IInstruction
    {
        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }
    }
}
