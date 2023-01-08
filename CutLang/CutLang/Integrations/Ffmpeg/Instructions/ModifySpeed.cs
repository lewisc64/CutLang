using CutLang.Execution;
using CutLang.Execution.Instruction;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CutLang.Integrations.Ffmpeg.Instructions
{
    public class ModifySpeed : IModifySpeed
    {
        public double Modifier { get; set; }

        public async Task Execute(ExecutionContext executionContext)
        {
            var segment = executionContext.SegmentStack.Pop();
            var outputPath = executionContext.GetTempVideoPath();

            var seedVideoTimebase = await Utils.GetTimebase(executionContext.SeedVideo.FullName);

            try
            {
                var audioTempos = new List<double>();
                var target = Modifier;
                var m = 1D;
                while (m != target)
                {
                    if (m <= target / 2)
                    {
                        m *= 2;
                        audioTempos.Add(2);
                    }
                    else if (m >= target * 2)
                    {
                        m /= 2;
                        audioTempos.Add(0.5);
                    }
                    else
                    {
                        audioTempos.Add(target / m);
                        break;
                    }
                }
                await Utils.Run($"-i \"{segment.File.FullName}\" -filter:v \"setpts = {1 / Modifier} * PTS\" -filter:a \"{string.Join(", ", audioTempos.Select(x => $"atempo={x}"))}\" -video_track_timescale {seedVideoTimebase} \"{outputPath}\"");
            }
            finally
            {
                File.Delete(segment.File.FullName);
                executionContext.SegmentStack.Push(new SegmentReference { File = new FileInfo(outputPath) });
            }
        }

        public override string ToString()
        {
            return $"FFmpeg {nameof(ModifySpeed)} (x{Modifier})";
        }
    }
}
