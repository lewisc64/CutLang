namespace CutLang.Execution.Instruction
{
    public interface IModifySpeed : IInstruction
    {
        public double Modifier { get; set; }
    }
}
