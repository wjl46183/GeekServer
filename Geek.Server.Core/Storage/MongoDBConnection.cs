using MongoDB.Driver;
using NLog;

namespace Geek.Server.Core.Storage
{
    /// <summary>
    /// MongoDB数据库接口
    /// </summary>
    public class MongoDBConnection : IGameDB
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// MongoDB客户端
        /// </summary>
        public MongoClient Client { get; private set; }

        /// <summary>
        /// 当前连接的数据库
        /// </summary>
        public IMongoDatabase CurDB { get; private set; }

        /// <summary>
        /// 替换选项，如果不存在则插入
        /// </summary>
        public static readonly ReplaceOptions REPLACE_OPTIONS = new() { IsUpsert = true };

        /// <summary>
        /// 批量写入选项，不保证顺序
        /// </summary>
        public static readonly BulkWriteOptions BULK_WRITE_OPTIONS = new() { IsOrdered = false };

        /// <summary>
        /// 初始化MongoDB服务，链接到MongoDB服务
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dbName"></param>
        public void Open(string url, string dbName)
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(url);
                Client = new MongoClient(settings);
                CurDB = Client.GetDatabase(dbName);
                Log.Info($"初始化MongoDB服务完成 Url:{url} DbName:{dbName}");
            }
            catch (Exception)
            {
                Log.Error($"初始化MongoDB服务失败 Url:{url} DbName:{dbName}");
                throw;
            }
        }

        /// <summary>
        /// 从MongoDB加载数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="defaultGetter"></param>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public async Task<TState> LoadState<TState>(long id, Func<TState> defaultGetter = null)
            where TState : BaseState, new()
        {
            var filter = Builders<TState>.Filter.Eq(BaseState.UniqueId, id);
            var stateName = typeof(TState).FullName;
            var col = CurDB.GetCollection<TState>(stateName);
            using var cursor = await col.FindAsync(filter);
            var state = await cursor.FirstOrDefaultAsync();
            state?.AfterLoadFromDB();
            state ??= defaultGetter?.Invoke();
            state ??= new TState { Id = id };
            Log.Info($"[DB] 读取表：{stateName} id:{id}");
            return state;
        }

        /// <summary>
        /// 保存数据到MongoDB
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="TState"></typeparam>
        public async Task SaveState<TState>(TState state) where TState : BaseState
        {
            var filter = Builders<TState>.Filter.Eq(BaseState.UniqueId, state.Id);
            var stateName = typeof(TState).FullName;
            var col = CurDB.GetCollection<TState>(stateName);
            var result = await col.ReplaceOneAsync(filter, state, REPLACE_OPTIONS);
            if (result.IsAcknowledged)
            {
                state.AfterSaveToDB();
            }
        }

        /// <summary>
        /// 关闭MongoDB服务
        /// </summary>
        public void Close()
        {
            Client.Cluster.Dispose();
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public Task Flush()
        {
            return Task.CompletedTask;
        }
    }
}