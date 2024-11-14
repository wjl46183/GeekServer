using MemoryPack;

//外部message定义，不要修改此类 
[MemoryPackable]
public partial class Message
{
    /// <summary>
    /// 消息唯一id
    /// </summary>
    public int UniId { get; set; }
    [MemoryPackIgnore]
    public virtual int MsgId { get; }
}
