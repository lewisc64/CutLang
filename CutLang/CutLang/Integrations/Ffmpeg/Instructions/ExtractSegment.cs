using CutLang.Execution;
using CutLang.Execution.Instruction;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CutLang.Integrations.Ffmpeg.Instructions
{
    public class ExtractSegment : IExtractSegment
    {
        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public async Task Execute(ExecutionContext executionContext)
        {
            var outputPath = executionContext.GetTempVideoPath();

            try
            {
                await Utils.Run($"-ss {Start.TotalSeconds} -i \"{executionContext.SeedVideo.FullName}\" -t {End.TotalSeconds - Start.TotalSeconds} -c copy -avoid_negative_ts 1 \"{outputPath}\"");
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
