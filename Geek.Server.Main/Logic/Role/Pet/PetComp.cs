using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Server.Storage.Role.Pet;

namespace Geek.Server.Main.Logic.Role.Pet;

[Comp(ActorType.Role)]
public class PetComp : StateComp<PetState>
{
}