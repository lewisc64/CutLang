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
                Console.WriteLine($"'{outputPath}' already exists.");
                Console.ResetColor();
                return;
            }

            var lexer = new Lexer(program);
            var syntaxParser = new SyntaxParser(lexer.Tokenise());
            var tree = syntaxParser.MakeTree();

            var factoryProvider = new InstructionFactoryProvider();
            factoryProvider.SetFactory<IExtractSegment>(() => new ExtractSegment());
            factoryProvider.SetFactory<IConcatSegments>(() => new ConcatSegments());

            var executor = new Executor();
            var instructions = executor.Instructionise(factoryProvider, tree).ToArray();
            var executionResultFile = executor.Execute(instructions, inputFile);

            File.Move(executionResultFile.FullName, outputPath);
        }
    }
}
