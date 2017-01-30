using CVARC.V2;
using Pudge.Units.PudgeUnit;

namespace Pudge.Units.WADUnit
{
    public static class WADCommandBuilderExctentions
    {
        public static CommandBuilder<TRules, TCommand> GameMove<TRules, TCommand>
            (this CommandBuilder<TRules, TCommand> builder, double range)
            where TCommand : IGameCommand, new()
            where TRules : IWADRules
        {
            builder.Add(new TCommand { GameMovement  = new GameMovement
            {
                Range = range,
                Angle = 0
            }});
            return builder;
        }

        public static CommandBuilder<TRules, TCommand> Rotate<TRules, TCommand>(this CommandBuilder<TRules, TCommand> builder, 
            double angle)
            where TCommand : IGameCommand, new()
            where TRules : IWADRules
        {
            builder.Add(new TCommand
            {
                GameMovement = new GameMovement
                {
                    Angle = angle,
                    Range = 0
                }
            });
            return builder;
        }

        public static CommandBuilder<TRules, TCommand> Wait<TRules, TCommand>(
            this CommandBuilder<TRules, TCommand> builder, double waitTime = 0.1)
            where TCommand : IGameCommand, new()
            where TRules : IWADRules
        {
            builder.Add(new TCommand
            {
                GameMovement = new GameMovement
                {
                    WaitTime = waitTime
                }
            });
            return builder;
        } 
    }
}