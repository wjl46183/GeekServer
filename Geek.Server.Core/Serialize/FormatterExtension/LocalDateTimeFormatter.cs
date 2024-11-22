using MemoryPack;
using System;
namespace Geek.Server.Core.Serialize
{
    // 注册自定义格式化器
    //emoryPackFormatterProvider.Register<DateTime, DateTimeFormatter>();
    public class DateTimeFormatter : MemoryPackFormatter<DateTime>
    {
        public override void Serialize<TBufferWriter>(ref MemoryPackWriter<TBufferWriter> writer,
            scoped ref DateTime value)
        {
            // 将 DateTime 转换为 ticks 和 kind (Ticks 是 long 类型)
            writer.WriteVarInt(value.Ticks);
            writer.WriteVarInt((int)value.Kind);
        }

        public override void Deserialize(ref MemoryPackReader reader, scoped ref DateTime value)
        {
            // 从 ticks 和 kind 读取 DateTime
            long ticks = reader.ReadVarIntInt64();
            DateTimeKind kind = (DateTimeKind)reader.ReadVarIntInt32();
            value = new DateTime(ticks, kind);
        }
    }
}