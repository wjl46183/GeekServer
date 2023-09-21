﻿using Geek.Server.Core.Net;
using Geek.Server.Core.Serialize;
using MessagePack;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace Geek.Server.TestPressure.Logic
{
    public class ClientTcpChannel : NetChannel
    {
        static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        protected Action<Message> onMessage;
        protected Pipe recvPipe;
        protected TcpClient socket;
        public ClientTcpChannel(TcpClient socket, Action<Message> onMessage = null)
        {
            this.socket = socket;
            this.onMessage = onMessage;
            recvPipe = new Pipe();
        }

        public override async Task StartAsync()
        {
            try
            {
                _ = recvNetData();
                var cancelToken = closeSrc.Token;
                while (!cancelToken.IsCancellationRequested)
                {
                    var result = await recvPipe.Reader.ReadAsync(cancelToken);
                    var buffer = result.Buffer;
                    if (buffer.Length > 0)
                    {
                        SequencePosition examined = buffer.Start;
                        SequencePosition consumed = examined;
                        TryParseMessage(buffer, ref consumed, ref examined);
                        recvPipe.Reader.AdvanceTo(consumed, examined);
                    }
                    else if (result.IsCanceled || result.IsCompleted)
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                LOGGER.Error(e.Message);
            }
        }

        async Task recvNetData()
        {
            byte[] readBuffer = new byte[2048];
            var dataPipeWriter = recvPipe.Writer;
            var cancelToken = closeSrc.Token;
            while (!cancelToken.IsCancellationRequested)
            { 
                var length = await socket.GetStream().ReadAsync(readBuffer, cancelToken);
                if (length > 0)
                {
                    dataPipeWriter.Write(readBuffer.AsSpan()[..length]);
                    var flushTask = dataPipeWriter.FlushAsync();
                    if (!flushTask.IsCompleted)
                    {
                        await flushTask.ConfigureAwait(false);
                    }
                }
                else
                {
                    break;
                } 
            }
        }

        protected virtual void TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined)
        {
            var reader = new SequenceReader<byte>(input);

            if (!reader.TryReadBigEndian(out int length) || reader.Remaining < length - 4)
            {
                consumed = input.End; //告诉read task，到这里为止还不满足一个消息的长度，继续等待更多数据
                return;
            }

            var payload = input.Slice(reader.Position, length - 4);
            if (payload.Length < 4)
                throw new Exception("消息长度不够");

            consumed = payload.End;
            examined = consumed;

            //消息id
            reader.TryReadBigEndian(out int msgId);
            var msgType = MsgFactory.GetType(msgId);
            if (msgType == null)
            {
                LOGGER.Error($"消息ID:{msgId} 找不到对应的Msg.");
            }
            else
            {
                var message = MessagePackSerializer.Deserialize<Message>(payload.Slice(4));
#if UNITY_EDITOR
                Debug.Log("收到消息:" + MessagePackSerializer.SerializeToJson(message));
#endif
                if (message.MsgId != msgId)
                {
                    throw new Exception($"解析消息错误，注册消息id和消息无法对应.real:{message.MsgId}, register:{msgId}");
                }

                onMessage(message);
            }
            return;
        }

        private const int Magic = 0x1234;
        int count = 0;
        public override void Write(Message msg)
        {
            if (IsClose())
                return;
            var bytes = Serializer.Serialize(msg);
            int len = 4 + 8 + 4 + 4 + bytes.Length;
            Span<byte> target = stackalloc byte[len];

            int magic = Magic + ++count;
            magic ^= Magic << 8;
            magic ^= len;

            int offset = 0;
            target.WriteInt(len, ref offset);
            target.WriteLong(DateTime.Now.Ticks, ref offset);
            target.WriteInt(magic, ref offset);
            target.WriteInt(msg.MsgId, ref offset);
            target.WriteBytesWithoutLength(bytes, ref offset);
            socket.GetStream().Write(target); 
        }
    }
}
