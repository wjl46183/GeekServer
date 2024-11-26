using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Server.Storage.Role.Base;

namespace Geek.Server.Main.Logic.Role.Base
{
    [Comp(ActorType.Role)]
    public partial class RoleComp : StateComp<RoleState>
    {
    }
}