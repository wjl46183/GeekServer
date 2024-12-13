using Geek.Server.Core.Actors;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Utils;
using Geek.Server.HotLogic.Logic.Server;
using Geek.Server.Storage.Comp;

namespace Geek.Server.HotLogic.Logic.Role.Pet
{
    public class PetCompAgent : BaseCompAgent<PetStateComp>
    {

        readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        private async Task OnGotNewPet(OneParam<int> param)
        {
            var serverComp = await ActorMgr.GetCompAgent<ServerCompAgent>();
            var level = await serverComp.GetWorldLevel();
            LOGGER.Debug($"PetCompAgent.OnGotNewPet监听到了获得宠物的事件,宠物ID:{param.value}当前世界等级:{level}");
        }

    }
}
