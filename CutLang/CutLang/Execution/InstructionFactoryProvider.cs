using CutLang.Execution.Instruction;
using System;
using System.Collections.Generic;

namespace CutLang.Execution
{
    public class InstructionFactoryProvider
    {
        private Dictionary<Type, Func<IInstruction>> _factories = new Dictionary<Type, Func<IInstruction>>();

        public InstructionFactoryProvider()
        {
        }

        public void SetFactory<T>(Func<IInstruction> creator)
            where T : IInstruction
        {
            _factories[typeof(T)] = creator;
        }

        public T CreateInstance<T>()
            where T : IInstruction
        {
            var type = typeof(T);

            if (!_factories.ContainsKey(type))
            {
                throw new ArgumentException($"No factory has been created for the specified instruction type: '{type.FullName}'");
            }

            return (T)_factories[type].Invoke();
        }
    }
}
