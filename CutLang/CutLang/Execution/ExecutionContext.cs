using System;
using System.Collections.Generic;
using System.IO;

namespace CutLang.Execution
{
    public class ExecutionContext
    {
        public Stack<SegmentReference> SegmentStack { get; set; } = new Stack<SegmentReference>();

        public FileInfo SeedVideo { get; set; }

        public string GetTempVideoPath()
        {
            return Path.Join(SeedVideo.DirectoryName, $"{Guid.NewGuid()}.mp4");
        }
    }
}
