using CutLang.Execution;
using CutLang.Execution.Instruction;
using System.IO;

namespace CutLang.Integrations.Ffmpeg.Instructions
{
    public class ConcatSegments : IConcatSegments
    {
        public void Execute(ExecutionContext executionContext)
        {
            var segmentTwo = executionContext.SegmentStack.Pop();
            var segmentOne = executionContext.SegmentStack.Pop();

            var outputPath = executionContext.GetTempVideoPath();

            try
            {
                Utils.Run($"-i \"{segmentOne.File.FullName}\" -i \"{segmentTwo.File.FullName}\" -filter_complex \"[0:v] [0:a] [1:v] [1:a] concat=n=2:v=1:a=1 [v] [a]\" -map \"[v]\" -map \"[a]\" \"{outputPath}\"").Wait();
            }
            finally
            {
                File.Delete(segmentOne.File.FullName);
                File.Delete(segmentTwo.File.FullName);
                executionContext.SegmentStack.Push(new SegmentReference { File = new FileInfo(outputPath) });
            }
        }

        public override string ToString()
        {
            return $"FFmpeg {nameof(ConcatSegments)}";
        }
    }
}