using CutLang.Execution;
using CutLang.Execution.Instruction;
using System.IO;
using System.Threading.Tasks;

namespace CutLang.Integrations.Ffmpeg.Instructions
{
    public class ConcatSegmentsWithDemuxer : IConcatSegments
    {
        public async Task Execute(ExecutionContext executionContext)
        {
            var segmentTwo = executionContext.SegmentStack.Pop();
            var segmentOne = executionContext.SegmentStack.Pop();

            var inputListFilePath = executionContext.GetTempFilePath("txt");
            File.WriteAllLines(
                inputListFilePath,
                new[]
                {
                    $"file '{segmentOne.File.FullName}'",
                    $"duration {(await Utils.GetDuration(segmentOne.File.FullName)).TotalSeconds}",
                    $"file '{segmentTwo.File.FullName}'",
                    $"duration {(await Utils.GetDuration(segmentTwo.File.FullName)).TotalSeconds}",
                });

            var outputPath = executionContext.GetTempVideoPath();

            try
            {
                await Utils.Run($"-f concat -safe 0 -i {inputListFilePath} -c copy {outputPath}");
            }
            finally
            {
                File.Delete(inputListFilePath);
                File.Delete(segmentOne.File.FullName);
                File.Delete(segmentTwo.File.FullName);
                executionContext.SegmentStack.Push(new SegmentReference { File = new FileInfo(outputPath) });
            }
        }

        public override string ToString()
        {
            return $"FFmpeg {nameof(ConcatSegmentsWithDemuxer)}";
        }
    }
}