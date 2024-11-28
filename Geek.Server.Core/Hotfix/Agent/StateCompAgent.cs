
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;

namespace Geek.Server.Core.Hotfix.Agent
{
    public abstract class StateCompAgent<TComp, TState> : BaseCompAgent<TComp> where TComp : StateComp<TState> where TState : BaseState, new()
    {
        public TState State => Comp.State;
    }
}
