using CutLang;
using CutLang.Execution;
using CutLang.Execution.Instruction;
using CutLang.Integrations.Ffmpeg.Instructions;
using CutLang.Token;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CutLang.Lexer;
using static CutLang.SyntaxParser;

namespace ICut
{
    public class Program
    {
        public const string VerticalFlag = "--vertical";

        public static readonly string[] ValidFlags = new[] { VerticalFlag };

        public static async Task Main(string[] args)
        {
            if (args.Count() < 2)
            {
                Console.WriteLine("Usage: icut VIDEO PROGRAM [flags?]");
                Console.WriteLine("Example: icut video.mp4 \"00:30-01:00\"");
                Console.WriteLine("Valid flags:");
                Console.WriteLine($" {VerticalFlag}: crops video to a 9:16 ratio");
                return;
            }

            foreach (var arg in args.Skip(2))
            {
                if (!ValidFlags.Contains(arg))
                {
                    WriteError($"Unknown flag: {arg}");
                    return;
                }
            }

            var inputFile = new FileInfo(args[0]);
            var program = args[1];
            var outputPath = $"{Path.Join(inputFile.DirectoryName, Path.GetFileNameWithoutExtension(inputFile.Name))}_cut{inputFile.Extension}";

            if (!File.Exists(inputFile.FullName))
            {
                WriteError($"{inputFile.FullName} does not exist.");
                return;
            }

            if (File.Exists(outputPath))
            {
                WriteError($"'{outputPath}' already exists.");
                return;
            }

            var lexer = new Lexer(program);
            IToken[] tokens;
            try
            {
                tokens = lexer.Tokenise().ToArray();
            }
            catch (LexerException ex)
            {
                WriteError($"Lexical error: {ex.Message}");
                return;
            }

            var syntaxParser = new SyntaxParser(tokens);
            Node tree;
            try
            {
                tree = syntaxParser.MakeTree();
            }
            catch (SyntaxParserException ex)
            {
                WriteError($"Syntax error: {ex.Message}");
                return;
            }

            var factoryProvider = new InstructionFactoryProvider();
            factoryProvider.SetFactory<IExtractSegment>(() => new ExtractSegment());
            factoryProvider.SetFactory<IConcatSegments>(() => new ConcatSegmentsWithDemuxer());
            factoryProvider.SetFactory<IModifySpeed>(() => new ModifySpeed());

            var executor = new Executor();
            var instructions = executor.Instructionise(factoryProvider, tree).ToArray();

            Action<int, int, string> progressCallback = (step, steps, instructionName) =>
            {
                Console.WriteLine($"Step {step}/{steps}: {instructionName}");
            };

            var outputFile = await executor.Execute(instructions, inputFile, progressCallback: progressCallback);

            if (args.Contains(VerticalFlag))
            {
                Console.WriteLine("Done, cropping video to 9:16...");
                await CutLang.Integrations.Ffmpeg.Utils.Run($"-i \"{outputFile.FullName}\" -vf \"crop = ih * (9 / 16):ih\" -crf 21 -c:a copy \"{outputPath}\"");
                File.Delete(outputFile.FullName);
            }
            else
            {
                File.Move(outputFile.FullName, outputPath);
            }
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }
    }
}
