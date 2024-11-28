using Geek.Server.Core.Net;
using Geek.Server.Core.Net.Tcp;
using Geek.Server.Core.Net.Websocket;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Geek.Server.TestPressure.Logic
{
    public enum ServerErrorCode
    {
        Success = 0,
        ConfigErr = 400, //配置表错误
        ParamErr, //客户端传递参数错误
        CostNotEnough, //消耗不足

        Notice = 100000, //正常通知
        FuncNotOpen, //功能未开启，主消息屏蔽
        Other //其他
    }

    public class Client
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        long id;
        NetChannel netChannel;
        MsgWaiter msgWaiter = new();
        int msgUniId = 200;

        public Client(long id)
        {
            this.id = id;
        }

        public async void Start()
        {
            if (TestSettings.Ins.useWebSocket)
            {
                var ws = new ClientWebSocket();
                await ws.ConnectAsync(new Uri(TestSettings.Ins.webSocketServerUrl), CancellationToken.None);

                if (ws.State == WebSocketState.Open)
                {
                    Log.Info($"Connected to {TestSettings.Ins.webSocketServerUrl}");
                    netChannel = new WebSocketChannel(ws, TestSettings.Ins.webSocketServerUrl, OnRevice);
                    _ = netChannel.StartAsync();
                }
                else
                {
                    Log.Error($"连接服务器失败...");
                    return;
                }
            }
            else
            {
                var socket = new TcpClient(AddressFamily.InterNetwork);
                try
                {
                    socket.NoDelay = true;
                    await socket.ConnectAsync(TestSettings.Ins.serverIp, TestSettings.Ins.serverPort);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return;
                }

                netChannel = new ClientTcpChannel(socket, OnRevice);
                _ = netChannel.StartAsync();
            }


            await DoReqLogin();
            await Task.Delay(5000);

            for (int i = 0; i < 100; i++)
            {
                await DoReqBagInfo();
                await Task.Delay(1000);
            }
            await DoReqComposePet();
        }

        private Task<bool> DoReqLogin()
        {
            //登陆
            var req = ReqLogin.Create();
            req.SdkType = 0;
            req.SdkToken = "555";
            req.UserName = "name" + id;
            req.Device = new Random().NextInt64().ToString();
            req.Platform = "android";
            return SendMsgAndWaitBack(req);
        }

        private Task DoReqBagInfo()
        {
            return SendMsgAndWaitBack(ReqBagInfo.Create());
        }

        private Task DoReqComposePet()
        {
            var req = ReqComposePet.Create();
            req.FragmentId = 1000;
            return SendMsgAndWaitBack(req);
        }
         
        async Task<bool> SendMsgAndWaitBack(Message msg)
        {
            msg.SerialId = (int)id*10000 +  msgUniId++;
            Log.Info($"{id} {msg.GetType()} 发送消息:{JsonConvert.SerializeObject(msg)}");
            var awaiter = msgWaiter.StartWait(msg.SerialId,  msg.GetType().Name); 
            netChannel.Write(msg);
            return await awaiter;
        }



        public void OnRevice(Message msg)
        {
            Log.Info($"收到消息:{msg.TypeId} {Geek.Server.HotData.MemoryPackTypeMapping.GetType(msg.TypeId)} {JsonConvert.SerializeObject(msg)}"); 

            if (msg.TypeId == ResErrorCode.TYPE_ID)
            {
                ResErrorCode errMsg = (ResErrorCode)msg;
                switch (errMsg.ErrCode)
                {
                    case (int)ServerErrorCode.Success:
                        //do some thing
                        break;
                    case (int)ServerErrorCode.ConfigErr:
                        //do some thing
                        break;
                    //case ....
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(errMsg.Desc))
                    Log.Info("服务器提示:" + errMsg.Desc);
            }
            else
            {
                msgWaiter.EndWait(msg.SerialId);
            }

        }
    }
}
