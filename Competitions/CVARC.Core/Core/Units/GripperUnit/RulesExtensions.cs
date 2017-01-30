using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CVARC.V2
{
	public static partial class RulesExtensions
	{
		public static TCommand Grip<TCommand>(this IGripperRules<TCommand> factory)
			where TCommand : IGripperCommand, new()
		{
			return new TCommand { GripperCommand = GripperAction.Grip };
		}

        public static CommandBuilder<TRules, TCommand> Grip<TRules, TCommand, TBack>(this  CommandBuilder<TRules, TCommand> builder)
            where TCommand : IGripperCommand, new()
            where TRules : IGripperRules<TCommand>
        {
            builder.Add(Grip(builder.Rules));
            return builder;
        }

		public static TCommand Release<TCommand>(this IGripperRules<TCommand> factory)
			where TCommand : IGripperCommand, new()
		{
			return new TCommand { GripperCommand = GripperAction.Release };
		}

        public static CommandBuilder<TRules, TCommand> Release<TRules, TCommand, TBack>(this  CommandBuilder<TRules, TCommand> builder)
            where TCommand : IGripperCommand, new()
            where TRules : IGripperRules<TCommand>       
        {
            builder.Add(Release(builder.Rules));
            return builder;
        }

		public static void AddGripKeys<TCommand>(this IGripperRules<TCommand> rules, KeyboardController<TCommand> pool, string controllerId)
			where TCommand : IGripperCommand, new()
		{
			if (controllerId == TwoPlayersId.Left)
			{
				pool.Add(Keys.R, () => new TCommand { GripperCommand = GripperAction.Grip });
				pool.Add(Keys.F, () => new TCommand { GripperCommand = GripperAction.Release });
			}
			if (controllerId == TwoPlayersId.Right)
			{
				pool.Add(Keys.P, () => new TCommand { GripperCommand = GripperAction.Grip });
				pool.Add(Keys.OemSemicolon, () => new TCommand { GripperCommand = GripperAction.Release });
			}
		}

	}
}
