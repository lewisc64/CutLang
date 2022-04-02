using CutLang.Execution;
using CutLang.Execution.Instruction;
using System;
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

            try
            {
                Utils.Run($"-i \"{executionContext.SeedVideo.FullName}\" -ss {Start.TotalSeconds} -to {End.TotalSeconds} -c copy \"{outputPath}\"").Wait();
            }
            finally
            {
                executionContext.SegmentStack.Push(new SegmentReference { File = new FileInfo(outputPath) });
            }
        }

        public override string ToString()
        {
            return $"FFmpeg {nameof(ExtractSegment)} ({Start.TotalSeconds}s-{End.TotalSeconds}s)";
        }
    }
}
