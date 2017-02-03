using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{
    public abstract class ControllerFactory
    {
        public IWorld World;
        
        public virtual void Initialize(IWorld world)
        {
            this.World = world;
        }

        protected ActorSettings GetSettings(string controllerId)
        {
            var result = World.Configuration.ActorSettings.SingleOrDefault(z => z.ControllerId == controllerId);
            if (result == null)
                throw new Exception("The controller '" + controllerId + "' is not defined in settings");
            return result;
        }

        protected IController CreateBot(string controllerId)
        {
            var sets = GetSettings(controllerId);
            if (!sets.IsBot)
                throw new Exception("Internal error: trying to create bot for '" + controllerId + "', but settings define it is not bot");
            var botName=sets.BotName;
            if (!World.Competitions.Logic.Bots.ContainsKey(botName))
                throw new Exception("Bot '"+botName+"' is not defined");
            return World.Competitions.Logic.Bots[botName]();
        }


        public abstract IController Create(string controllerId, IActor actor);

		public virtual void Exit() { }
    }

}
