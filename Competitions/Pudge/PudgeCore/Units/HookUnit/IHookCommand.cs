using CVARC.V2;

namespace Pudge.Units.HookUnit
{
    public interface IHookCommand : ICommand
    {
        bool MakeHook{ get; set; }
    }
}