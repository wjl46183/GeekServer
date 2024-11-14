using System.Buffers;

using MemoryPack;
using MongoDB.Bson.Serialization.Attributes;
using NLog;

namespace Geek.Server.Core.Storage
{
    [MemoryPackable]
    [BsonIgnoreExtraElements(true, Inherited = true)]
    public partial class CacheState
    {
        public const string UniqueId = nameof(Id);

        public long Id { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}[Id={Id}]";
        }

        #region hash

        private StateHash stateHash;

        public void AfterLoadFromDB()
        {
            stateHash ??= new StateHash(this, true);
        }

        public bool IsChanged()
        {
            stateHash ??= new StateHash(this, false);
            return stateHash.IsChanged();
        }

        public void AfterSaveToDB()
        {
            stateHash.AfterSaveToDB();
        }

        #endregion
    }


    public class StateHash
    {
        private static readonly AsyncLocal<ArrayBufferWriter<byte>> _bufferWriter =
            new AsyncLocal<ArrayBufferWriter<byte>>();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private CacheState State { get; }
        private UInt128 CurrentHash { get; set; }
        private UInt128 DBHash { get; set; }

        public StateHash(CacheState state, bool loadFromDB = false)
        {
            State = state;
            CurrentHash = GetHash();
            if (loadFromDB)
            {
                DBHash = CurrentHash;
            }
        }

        public bool IsChanged()
        {
            if (DBHash != CurrentHash)
                return true;
            CurrentHash = GetHash();
            return DBHash != CurrentHash || CurrentHash == 0;
        }

        public void AfterSaveToDB()
        {
            DBHash = CurrentHash;
        }

        private UInt128 GetHash()
        {
            if (State == null)
                return 0;

            // 尝试获取或创建 ArrayBufferWriter<byte> 实例
            var bufferWriter = _bufferWriter.Value;
            if (bufferWriter == null)
            {
                bufferWriter = new ArrayBufferWriter<byte>();
                _bufferWriter.Value = bufferWriter;
            }
            else
            {
                bufferWriter.Clear();
            }

            MemoryPackSerializer.Serialize(bufferWriter, State);
            var buffer = bufferWriter.WrittenSpan;

            ulong hash = 3074457345618258791ul;
            foreach (var b in buffer)
            {
                hash += b;
                hash *= 3074457345618258799ul;
            }

            return new UInt128(hash, (ulong)buffer.Length);
        }
    }
}