using CVARC.V2;
using Infrastructure;

namespace Assets.Tools
{
    public class WorldCreationParams
    {
        public readonly GameSettings GameSettings;
        public readonly ControllerFactory ControllerFactory;
        public readonly WorldState WorldState;

        public WorldCreationParams(GameSettings gameSettings, ControllerFactory controllerFactory, WorldState worldState)
        {
            GameSettings = gameSettings;
            ControllerFactory = controllerFactory;
            WorldState = worldState;
        }
    }
}
