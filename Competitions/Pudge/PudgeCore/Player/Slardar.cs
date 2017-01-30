using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using Pudge.Units.HookUnit;
using Pudge.Units.WADUnit;
using Pudge.World;

namespace Pudge.Player
{
    public class Slardar : Robot<PudgeWorld, object, SlardarCommand, SlardarRules>,
        IWADRobot
    {
        public delegate void OnDieMethod();
        public event OnDieMethod OnDie;
        public double HasteFactor
        {
            get { return 1; }
        }

        bool stun;

        public void RunAnimation(Animation animation)
        {
            if (stun && animation == Animation.Idle) return;

            stun = animation == Animation.Stun;

            World.GetEngine<IPudgeWorldEngine>().PlayAnimation(ObjectId, animation);
        }
        
        private double respawnTime;
        private WADUnit wadUnit;
        private DeathUnit deathUnit;

        public override void AdditionalInitialization()
        {
            base.AdditionalInitialization();
            World.GetEngine<IPudgeWorldEngine>().CreateSlardarBody(ObjectId, ControllerId, false);
            wadUnit =new WADUnit(this);
            deathUnit = new DeathUnit(() => respawnTime - World.Clocks.CurrentTime);
        }

        public void Die()
        {
            if (OnDie != null)
                OnDie();
            var timeUntilRespawn = PudgeRules.Current.SlardarRespawnTime -
                World.Clocks.CurrentTime % PudgeRules.Current.SlardarRespawnTime;
            this.Die(timeUntilRespawn);
            respawnTime = timeUntilRespawn + World.Clocks.CurrentTime;
        }

        public override IEnumerable<IUnit> Units
        {
            get
            {
                yield return wadUnit;
                yield return deathUnit;
            }
        }
    }
}
