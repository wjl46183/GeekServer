using Quartz;
using SharpCompress.Writers;
using System.Runtime.CompilerServices;

namespace Geek.Server.TestPressure.Logic
{
    public class MsgWaiter
    {
        public class Awaiter : INotifyCompletion
        {
            private Action _callback = () => { };
            private bool result = false;
            private Timer timer;
            public bool IsCompleted => cmp;
            public bool GetResult() => result;
            public Awaiter GetAwaiter() => this;
            long _serialId;
            string msg;
            bool cmp = false;
            public Awaiter(long serialId,string msg)
            {
                this._serialId = serialId;
                this.msg = msg;
                timer = new Timer(TimeOut, null, 10000, -1);
            }

            public void OnCompleted(Action continuation)
            {
                if (IsCompleted)
                {  
                    continuation();
                }else
                {
                    _callback += continuation;
                }
            }
 

            public void Complete(bool result)
            { 
                if(!cmp)
                {
                    cmp = true;
                    this.result = result;
                    timer.Dispose();
                    _callback();
                }
            }

            void TimeOut(object state)
            {
                Log.Error($"等待消息超时:{_serialId} {msg} {cmp}");
                Complete(false); 
            }
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<long, Awaiter> waitDic = new();

        //long id;
        //public MsgWaiter(long id)
        //{
        //    this.id = id;
        //}

        public void Clear()
        {
            foreach (var kv in waitDic)
                kv.Value.Complete(false);
            waitDic.Clear();
        }

        public Awaiter StartWait(long serialId,string msg)
        {
            Awaiter waiter = null;
            lock (waitDic)
            {
                if (!waitDic.ContainsKey(serialId))
                {
                    waiter = new Awaiter(serialId,msg);
                    waitDic.Add(serialId, waiter);
                }
                else
                {
                    Log.Error("发现重复消息id：" + serialId);
                }
                return waiter;
            }
        }

        public void EndWait(long serialId, bool result = true)
        {
            if (!result) Log.Error("await失败：" + serialId);
            Awaiter waiter = null;
            lock (waitDic)
            {
                if (waitDic.ContainsKey(serialId))
                {
                    waiter = waitDic[serialId];
                    waitDic.Remove(serialId); 
                }
                else
                {
                    if (serialId > 0)
                        Log.Error("找不到EndWait：" + serialId + ">size：" + waitDic.Count);
                }
                waiter?.Complete(result); 
            }
        }
    }
}
