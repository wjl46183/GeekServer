using System.Collections.Concurrent;

namespace Geek.Server.Core.Net
{
    public abstract class NetChannel
    {
        /// <summary>
        /// 链接ID（可用于用户ID）
        /// </summary>
        public int sessionId = 0;
        
        /// <summary>
        /// 是否绑定终端
        /// 绑定后才可以业务层的通信
        /// </summary>
        public bool IsBind = false;
        
        protected CancellationTokenSource closeSrc = new();
        public virtual void Write(Message msg) { }
        public virtual void Close() { closeSrc.Cancel(); }
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
    }
}
