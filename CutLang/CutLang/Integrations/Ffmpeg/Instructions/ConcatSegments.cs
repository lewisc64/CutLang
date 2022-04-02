using CutLang.Execution;
using CutLang.Execution.Instruction;
using System;
using System.Diagnostics;
using System.IO;

namespace CutLang.Integrations.Ffmpeg.Instructions
{
    public class ConcatSegments : IConcatSegments
    {
        public void Execute(ExecutionContext executionContext)
        {
            var segmentTwo = executionContext.SegmentStack.Pop();
            var segmentOne = executionContext.SegmentStack.Pop();

            var tempListFile = $"{Guid.NewGuid()}.txt";
            var outputPath = executionContext.GetTempVideoPath();

            try
            {
                File.WriteAllText(tempListFile, $"file '{segmentOne.File.FullName.Replace("\\", "/")}'\nfile '{segmentTwo.File.FullName.Replace("\\", "/")}'");
                Process.Start("ffmpeg", $"-f concat -safe 0 -i \"{tempListFile}\" -c copy \"{outputPath}\"")
                    .WaitForExit();
            }
            finally
            {
                File.Delete(tempListFile);
                File.Delete(segmentOne.File.FullName);
                File.Delete(segmentTwo.File.FullName);
            }

            executionContext.SegmentStack.Push(new SegmentReference { File = new FileInfo(outputPath) });
        }
    }
}