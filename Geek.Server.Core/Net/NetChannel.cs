using System.Collections.Concurrent;
using Geek.Server.Core.Events.Datas;

namespace Geek.Server.Core.Net
{
    public abstract class NetChannel
    {
        /// <summary>
        /// 链接ID，服务器进程唯一，0是默认的，表示没有被绑定
        /// </summary>
        public long actorId = 0;

        /// <summary>
        /// 唯一ID，与Session保持一致
        /// </summary>
        public long SerialId = 0;

        protected CancellationTokenSource closeSrc = new();

        public virtual void Write(BaseEvent msg)
        {
        }

        public virtual void Close()
        {
            closeSrc.Cancel();
        }

        public virtual bool IsClose()
        {
            return closeSrc.IsCancellationRequested;
        }

        public virtual string RemoteAddress { get; set; } = "";

        public virtual Task StartAsync()
        {
            return Task.CompletedTask;
        }


        readonly ConcurrentDictionary<string, object> datas = new();

        public T GetData<T>(string key)
        {
            if (datas.TryGetValue(key, out var v))
            {
                return (T)v;
            }

            return default;
        }

        public void RemoveData(string key)
        {
            datas.Remove(key, out _);
        }

        public void SetData(string key, object v)
        {
            datas[key] = v;
        }
        
        public void Write(BaseEvent msg, long serialId)
        {
            if (msg != null)
            {
               
            }
            msg.SerialId = serialId;
            Write(msg);
        }

        public void Write(long serialId, int code, string desc)
        {
            ResErrorCode res = ResErrorCode.Create();
            res.SerialId = serialId;
            res.ErrCode = (int)code;
            res.Desc = desc;
            Write(res);
        }
    }
}