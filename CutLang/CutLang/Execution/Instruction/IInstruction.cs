namespace CutLang.Execution.Instruction
{
    public interface IInstruction
    {
        public void Execute(ExecutionContext executionContext);
    }
}
