using CVARC.V2;
using Pudge.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pudge.Units.HookUnit
{
    public static class HookCommandBuilderExtensions
    {
        public static CommandBuilder<TRules, TCommand> Hook<TRules, TCommand>
            (this CommandBuilder<TRules, TCommand> builder)
            where TCommand : IHookCommand, new()
            where TRules : IHookRules
        {
            builder.Add(new TCommand { MakeHook = true });
            return builder;
        }
    }
}
