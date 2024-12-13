namespace Geek.Server.Core.Storage
{
    /// <summary>
    /// 数据库接口
    /// </summary>
    public interface IGameDB
    {
        /// <summary>
        /// 链接数据库
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dbName"></param>
        public void Open(string url, string dbName);
        
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void Close();
        
        /// <summary>
        /// 数据刷新到数据库
        /// </summary>
        /// <returns></returns>
        public Task Flush();

        /// <summary>
        /// 从数据库加载指定状态数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultGetter"></param>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public Task<TState> LoadState<TState>(long id, Func<TState> defaultGetter = null)
            where TState : BaseState, new();

        /// <summary>
        /// 存储状态数据到数据库
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public Task SaveState<TState>(TState state) where TState : BaseState;
    }
}