using Geek.Server.Core.Actors;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net.BaseHandler;
using Geek.Server.HotData.Proto;
using Geek.Server.Storage.Comp;

namespace Geek.Server.HotLogic.Logic.Account;

public class AccountCompAgent : BaseCompAgent<AccountStateComp>
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 检查账号是否存在
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        [Api]
        public virtual async Task<bool> CheckAccount(string token)
        {
            return true;
        }

    }