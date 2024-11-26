using System.Collections.Concurrent;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Server.Storage.Login;

namespace Geek.Server.Main.Logic.Login
{
    [Comp(ActorType.Server)]
    public class LoginComp : StateComp<LoginState>
    {
    }
}