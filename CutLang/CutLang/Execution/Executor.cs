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
            else if (tree.Token is SpeedUpToken || tree.Token is SlowDownToken)
            {
                var modifySpeedInstruction = factoryProvider.CreateInstance<IModifySpeed>();

                foreach (var instruction in Instructionise(factoryProvider, tree.Left))
                {
                    yield return instruction;
                }

                if (tree.Token is SpeedUpToken)
                {
                    modifySpeedInstruction.Modifier = ((NumberToken)tree.Right.Token).Value;
                }
                else
                {
                    modifySpeedInstruction.Modifier = 1 / ((NumberToken)tree.Right.Token).Value;
                }

                yield return modifySpeedInstruction;
            }
            else
            {
                throw new InstructioniseException($"Unrecognised token: {tree.Token}");
            }
        }

        public FileInfo Execute(IEnumerable<IInstruction> instructions, FileInfo seedVideo, Action<int, int, string> progressCallback = null)
        {
            var context = new ExecutionContext
            {
                SeedVideo = seedVideo,
            };

            var step = 1;
            foreach (var instruction in instructions)
            {
                progressCallback?.Invoke(step++, instructions.Count(), instruction.ToString());
                try
                {
                    instruction.Execute(context);
                }
                catch (Exception)
                {
                    context.CleanUp();
                    throw;
                }
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
