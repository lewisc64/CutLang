using CutLang.Execution;
using CutLang.Execution.Instruction;
using System;
using System.Diagnostics;
using System.IO;

namespace CutLang.Integrations.Ffmpeg.Instructions
{
    public class ExtractSegment : IExtractSegment
    {
        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public void Execute(ExecutionContext executionContext)
        {
            var outputPath = executionContext.GetTempVideoPath();
            Process.Start("ffmpeg", $"-i \"{executionContext.SeedVideo.FullName}\" -ss {Start.TotalSeconds} -to {End.TotalSeconds} -c copy \"{outputPath}\"")
                .WaitForExit();

            executionContext.SegmentStack.Push(new SegmentReference { File = new FileInfo(outputPath) });
        }
    }
}
