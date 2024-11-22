using System.Buffers;

using MessagePack;
using Geek.Server.Core.PolymorphicType;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using MemoryPack;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Geek.Server.Core.Net.Websocket
{
    public class WebSocketChannel : NetChannel
    {
        private const int bufferSize = 1024 * 16;
        static readonly NLog.Logger LOGGER = NLog.LogManager.GetCurrentClassLogger();
        WebSocket webSocket;
        readonly Action<Message> onMessage;
        protected readonly ConcurrentQueue<Message> sendQueue = new();
        protected readonly SemaphoreSlim newSendMsgSemaphore = new(0); 

        public WebSocketChannel(WebSocket webSocket, string remoteAddress, Action<Message> onMessage = null)
        {
            this.RemoteAddress = remoteAddress;
            this.webSocket = webSocket;
            this.onMessage = onMessage;
        }

        public override async void Close()
        {
            try
            {
                base.Close();
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socketclose", CancellationToken.None);
            }
            catch
            {
            }
            finally
            {
                webSocket = null;
            }
        }

        public override async Task StartAsync()
        {
            try
            {
                _ = DoSend();
                await DoReceive();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                LOGGER.Error(e.Message);
            }
        }

        async Task DoSend()
        {
            // 使用 ArrayBufferWriter 来管理缓冲区
            var bufferWriter = new ArrayBufferWriter<byte>();
            var closeToken = closeSrc.Token;
            try
            {
                while (!closeToken.IsCancellationRequested)
                {
                    try
                    {
                        await newSendMsgSemaphore.WaitAsync(closeToken);

                        if (!sendQueue.TryDequeue(out var message))
                        {
                            continue;
                        }

                        bufferWriter.Clear();
                        // 写入 MsgId
                        var msgIdBytes = BitConverter.GetBytes(message.TypeId);
                        bufferWriter.Write(msgIdBytes);

                        // 序列化 message 到 bufferWriter
                        byte[] bytes = MemoryPackSerializer.Serialize(message.GetType(),message);
                        bufferWriter.Write(bytes);

                        // 获取已写入数据的内存片段
                        var data = bufferWriter.WrittenMemory;

#if DEBUG
                        LOGGER.Info($"发送消息长度: {data.Length}, MsgId: {message.TypeId}");
#endif

                        // 发送数据
                        await webSocket.SendAsync(data, WebSocketMessageType.Binary, true, closeToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // 操作取消异常可以忽略
                        break;
                    }
                    catch (Exception ex)
                    {
                        // 记录其他异常
                        LOGGER.Error("发送消息时出现异常: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获并记录外层循环中的异常
                LOGGER.Error("DoSend 出现异常: " + ex.Message);
            }
            finally
            {
                // 确保在退出时释放信号量
                newSendMsgSemaphore.Release();
            }
        }

        Message DeserializeMsg(ReadOnlySpan<byte> buffer)
        {
            Type type = null;
            int typeId = BitConverter.ToInt32(buffer.Slice(0,4));
            if (!PolymorphicTypeMapper.TryGet(typeId, out type))
            {
                throw new MemoryPackSerializationException($"找不到 Type Id: {typeId} 检查是否注册到： {nameof(PolymorphicTypeMapper)}");
            }

            Message msg = MemoryPackSerializer.Deserialize(type, buffer) as Message;
            return msg;
        }

        private async Task DoReceive()
        {
            byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(bufferSize); // Rent an 16KB buffer
            try
            {
                var closeToken = closeSrc.Token;
                while (!closeToken.IsCancellationRequested)
                {
                    int totalBytesReceived = 0;
                    WebSocketReceiveResult result;
                    do
                    {
                        var bufferSegment = new ArraySegment<byte>(rentedBuffer, totalBytesReceived, rentedBuffer.Length - totalBytesReceived);
                        result = await webSocket.ReceiveAsync(bufferSegment, closeToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                            return;
                        }

                        totalBytesReceived += result.Count;

                        // Ensure we do not exceed the buffer length
                        if (totalBytesReceived >= rentedBuffer.Length)
                        {
                            throw new InvalidOperationException("Buffer overflow. The message is too large to fit in the buffer.");
                        }
                    } while (!result.EndOfMessage);

                    var message = DeserializeMsg(rentedBuffer.AsSpan(0, totalBytesReceived));

#if DEBUG
                    LOGGER.Info("收到消息: " + message.GetType().Name + "  " + JsonConvert.SerializeObject(message));
#endif
                    onMessage(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rentedBuffer); // Ensure buffer is returned
            }
        }

        public override void Write(Message msg)
        {
            sendQueue.Enqueue(msg);
            newSendMsgSemaphore.Release();
        }
    }
}
