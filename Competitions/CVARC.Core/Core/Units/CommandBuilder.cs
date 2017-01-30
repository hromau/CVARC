using System;

namespace CVARC.V2
{
    public sealed class CommandBuilder<TRules, TCommand>
    {
        public CommandBuilder(TRules rules)
        {
            Rules = rules;
        }

        public readonly TRules Rules;
        public event Action<TCommand> CommandAdded;

        public void Add(TCommand command)
        {
            if (CommandAdded != null)
                CommandAdded.Invoke(command);
        }
    }

}
