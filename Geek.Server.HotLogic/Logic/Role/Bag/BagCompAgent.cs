using Geek.Server.Main.Common.Event;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Utils;
using Server.Logic.Common.Events;
using Server.Logic.Logic.Role.Base;
using Geek.Server.Storage.Role.Bag;
using Geek.Server.Storage.Role.Bag.Comp;

namespace Server.Logic.Logic.Role.Bag
{
    public class BagCompAgent : StateCompAgent<BagStateComp
        , BagState>
    {
        readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        public override void Active()
        {
            if (State.ItemMap.Count <= 0)
            {
                State.ItemMap.Add(101, 1);
                State.ItemMap.Add(103, 100);
            }
        }

        private ResBagInfo BuildInfoMsg()
        {
            ResBagInfo res = ResBagInfo.Create();
            // foreach (var kv in State.ItemMap)
            //     res.ItemDic[kv.Key] = kv.Value;
            // res.ItemDic[100] = 2;
            return res;
        }

        private ResBagInfo BuildInfoMsg2()
        {
            // ResBagInfo res = Message.pool.GetObject<ResBagInfo>(ResBagInfo.TYPE_ID);
            ResBagInfo res = ResBagInfo.Create();

            // res.ItemDic[100] = 1;
            return res;
        }

        public async Task GetBagInfo(ReqBagInfo reqMsg)
        {
            int a = Random.Shared.Next(100);
            if (a > 50)
            {
                var ret = BuildInfoMsg();
                await this.NotifyClient(ret, reqMsg.SerialId);
            }
            else
            {
                var ret = BuildInfoMsg2();
                await this.NotifyClient(ret, reqMsg.SerialId);
            }
        }

        /// <summary>
        /// 宠物合成
        /// </summary>
        /// <returns></returns>
        public async Task ComposePet(ReqComposePet reqMsg)
        {
            //宠物碎片合成相关逻辑
            //.....
            //.....

            //合成成功后分发一个获得宠物的事件(在PetCompAgent中监听此事件)
            this.Dispatch(EventID.GotNewPet, new OneParam<int>(1000));

            var res = ResComposePet.Create();
            res.PetId = 1000;
            await this.NotifyClient(res, reqMsg.SerialId);
        }
    }
}