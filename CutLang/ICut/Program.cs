using CutLang;
using CutLang.Execution;
using CutLang.Execution.Instruction;
using CutLang.Integrations.Ffmpeg.Instructions;
using CutLang.Token;
using System;
using System.IO;
using System.Linq;
using static CutLang.Lexer;
using static CutLang.SyntaxParser;

namespace ICut
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Count() != 2)
            {
                Console.WriteLine("Usage: icut VIDEO PROGRAM");
                Console.WriteLine("Example: icut video.mp4 \"00:30-01:00\"");
                return;
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
            factoryProvider.SetFactory<IConcatSegments>(() => new ConcatSegments());
            factoryProvider.SetFactory<IModifySpeed>(() => new ModifySpeed());

            var executor = new Executor();
            var instructions = executor.Instructionise(factoryProvider, tree).ToArray();

            Action<int, int, string> progressCallback = (step, steps, instructionName) =>
            {
                Console.WriteLine($"Step {step}/{steps}: {instructionName}");
            };

            File.Move(executor.Execute(instructions, inputFile, progressCallback: progressCallback).FullName, outputPath);
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }
    }
}
