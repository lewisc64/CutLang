using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CutLang.Integrations.Ffmpeg
{
    public static class Utils
    {
        public static async Task Run(string args)
        {
            await RunProcess("ffmpeg", args);
        }

        public static async Task<TimeSpan> GetDuration(string path)
        {
            var args = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 {path}";
            return TimeSpan.FromSeconds(double.Parse(await RunProcess("ffprobe", args)));
        }

        public static async Task<int> GetTimebase(string path)
        {
            var args = $"-v 0 -of compact=p=0:nk=1 -show_entries stream=time_base -select_streams v:0 {path}";
            return int.Parse((await RunProcess("ffprobe", args)).Split("/").Last());
        }

        private static async Task<string> RunProcess(string command, string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"'{command}' returned a non-zero exit code ({process.ExitCode})");
            }

            return process.StandardOutput.ReadToEnd();
        }
    }
}
