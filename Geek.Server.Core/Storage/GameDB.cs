using Geek.Server.Core.Utils;

namespace Geek.Server.Core.Storage
{
    public class GameDB
    {
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();

        private static IGameDB dbImpler;


        public static void Init(IGameDB impler)
        {
            dbImpler = impler;
        }

        public static async Task Flush()
        {
            await dbImpler.Flush();
        }

        public static T As<T>() where T : IGameDB
        {
            return (T)dbImpler;
        }

        public static void Open(string url, string dbName)
        {
            dbImpler.Open(url, dbName);
        }

        public static void Close()
        {
            dbImpler.Close();
        }

        public static Task<TState> LoadState<TState>(long id, Func<TState> defaultGetter = null)
            where TState : BaseState, new()
        {
            return dbImpler.LoadState(id, defaultGetter);
        }

        public static async Task SaveState<TState>(TState state) where TState : BaseState
        {
            await dbImpler.SaveState(state);
        }
    }
}