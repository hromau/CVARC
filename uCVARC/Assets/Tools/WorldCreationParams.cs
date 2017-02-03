using CVARC.V2;
using Infrastructure;

namespace Assets.Tools
{
    public class WorldCreationParams
    {
        public readonly GameSettings GameSettings;
        public readonly ControllerFactory ControllerFactory;
        public readonly IWorldState WorldState;

        public WorldCreationParams(GameSettings gameSettings, ControllerFactory controllerFactory, IWorldState worldState)
        {
            GameSettings = gameSettings;
            ControllerFactory = controllerFactory;
            WorldState = worldState;
        }
    }
}
