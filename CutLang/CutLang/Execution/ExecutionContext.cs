using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CutLang.Execution
{
    public class ExecutionContext
    {
        public Stack<SegmentReference> SegmentStack { get; set; } = new Stack<SegmentReference>();

        public FileInfo SeedVideo { get; set; }

        public string GetTempVideoPath()
        {
            return GetTempFilePath(SeedVideo.Extension[1..]);
        }

        public string GetTempFilePath(string extension)
        {
            return Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.{extension}");
        }

        public void CleanUp()
        {
            while (SegmentStack.Any())
            {
                File.Delete(SegmentStack.Pop().File.FullName);
            }
        }
    }
}
