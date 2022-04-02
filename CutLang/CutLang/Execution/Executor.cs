using CutLang.Execution.Instruction;
using CutLang.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CutLang.Execution
{
    public class Executor
    {
        public IEnumerable<IInstruction> Instructionise(InstructionFactoryProvider factoryProvider, SyntaxParser.Node tree)
        {
            if (tree.Token is SpanToken)
            {
                var instruction = factoryProvider.CreateInstance<IExtractSegment>();
                instruction.Start = ((TimestampToken)tree.Left.Token).AsTimeSpan;
                if (tree.Right.Token is EndOfVideoToken)
                {
                    instruction.End = TimeSpan.MaxValue;
                }
                else
                {
                    instruction.End = ((TimestampToken)tree.Right.Token).AsTimeSpan;
                }
                yield return instruction;
            }
            else if (tree.Token is ConcatToken)
            {
                foreach (var instruction in Instructionise(factoryProvider, tree.Left))
                {
                    yield return instruction;
                }
                foreach (var instruction in Instructionise(factoryProvider, tree.Right))
                {
                    yield return instruction;
                }
                yield return factoryProvider.CreateInstance<IConcatSegments>();
            }
            else
            {
                throw new InstructioniseException($"Unrecognised token: {tree.Token}");
            }
        }

        public FileInfo Execute(IEnumerable<IInstruction> instructions, FileInfo seedVideo)
        {
            var context = new ExecutionContext
            {
                SeedVideo = seedVideo,
            };

            foreach (var instruction in instructions)
            {
                instruction.Execute(context);
            }

            return context.SegmentStack.Single().File;
        }

        public class InstructioniseException : Exception
        {
            public InstructioniseException(string message)
                : base(message)
            {
            }
        }
    }
}
