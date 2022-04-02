using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CutLang.Integrations.Ffmpeg
{
    public static class Utils
    {
        public static async Task Run(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"FFmpeg returned a non-zero exit code.");
            }
        }
    }
}
