using CVARC.V2;

namespace Pudge.Player
{
    static class ActorExtensions
    {
        public static void Die<TWorld, TCommand, TRules>
            (this Actor<TWorld, TCommand, TRules> actor, double deathDuration)
                where TWorld : IWorld
                where TCommand : ICommand
                where TRules : IRules
        {
            var corpseId = actor.ObjectId + "'s Corpse";
            actor.World.GetEngine<IPudgeWorldEngine>().TurnIntoCorpse(actor.ObjectId, corpseId);

            actor.World.Clocks.AddTrigger(new OneTimeTrigger(
                actor.World.Clocks.CurrentTime + deathDuration / 2,
                () => actor.World.GetEngine<ICommonEngine>().DeleteObject(corpseId)
            ));

            actor.World.Clocks.AddTrigger(new OneTimeTrigger(
                actor.World.Clocks.CurrentTime + deathDuration,
                () => actor.World.GetEngine<IPudgeWorldEngine>().CreateActorBody(actor, actor.World.DebugMode)
            ));
        }
    }
}
