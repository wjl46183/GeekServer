using Geek.Server.Core.Actors;
using Geek.Server.Core.Comps;
using Geek.Server.Core.Storage;
using MemoryPack;

namespace Geek.Server.Storage.Role.Pet;

[MemoryPackable]
[SaveState(ActorType.Role)]
public partial class PetState : BaseState
{
}