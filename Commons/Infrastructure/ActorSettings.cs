namespace Infrastructure
{
    public class ActorSettings
    {
        public string ControllerId { get; set; }
        public bool IsBot { get; set; }
        public string BotName { get; set; }
        public PlayerSettings PlayerSettings { get; set; }
    }
}
