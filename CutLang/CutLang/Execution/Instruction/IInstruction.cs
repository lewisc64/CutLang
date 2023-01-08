using System.Threading.Tasks;

namespace CutLang.Execution.Instruction
{
    public interface IInstruction
    {
        Task Execute(ExecutionContext executionContext);
    }
}
