using CutLang;
using CutLang.Execution;
using CutLang.Execution.Instruction;
using CutLang.Integrations.Ffmpeg.Instructions;
using System;
using System.IO;
using System.Linq;

namespace ICut
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = new FileInfo(args[0]);
            var program = args[1];
            var outputPath = $"{Path.Join(inputFile.DirectoryName, Path.GetFileNameWithoutExtension(inputFile.Name))}_cut{inputFile.Extension}";

            if (File.Exists(outputPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"'{outputPath}' already exists.");
                Console.ResetColor();
                return;
            }

            var lexer = new Lexer(program);
            var tokens = lexer.Tokenise().ToArray();

            var syntaxParser = new SyntaxParser(tokens);
            var tree = syntaxParser.MakeTree();

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
    }
}
