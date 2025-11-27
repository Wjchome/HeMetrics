using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Google.Protobuf;

/// <summary>
/// Unity TCP客户端 - 用于连接Go服务器
/// </summary>
public class NetManager : MonoBehaviour
{
    public bool isLocal = false;

    private TcpClient tcpClient; //tcp连接
    private NetworkStream stream; //写流
    private Thread receiveThread; //听线程
    private Thread keepAliveThread; //心跳线程
    private bool isConnected = false; //是否连接
    private uint reqIdCounter = 0; //一个消息id计数器
    private Dictionary<uint, Action<byte[]>> pendingRequests = new Dictionary<uint, Action<byte[]>>();
    private Queue<Action> actionQueue = new Queue<Action>();


    [Header("服务器配置")] public string serverHost = "127.0.0.1";
    public int serverPort = 1024;

    [Header("心跳配置")] public float keepAliveInterval = 2f; // 心跳间隔（秒）

    // 心跳相关常量（与Go服务器保持一致）
    private const uint InnerServiceModuleId = 1u << 31; // 2147483648
    private const uint KeepAliveRouterId = 1u << 31; // 2147483648

    public int serverTimer = 0;

    public void Init()
    {
        if (!isLocal)
            Connect();
    }

    /// <summary>
    /// 连接到服务器
    /// </summary>
    private void Connect()
    {
        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(serverHost, serverPort);
            stream = tcpClient.GetStream();
            isConnected = true;

            // 启动接收线程
            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            // 启动心跳线程
            keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.IsBackground = true;
            keepAliveThread.Start();

            Debug.Log($"已连接到服务器 {serverHost}:{serverPort}");
        }
        catch (Exception e)
        {
            Debug.LogError($"连接失败: {e.Message}");
            StartCoroutine(test());
            isConnected = false;
        }
    }

    IEnumerator test()
    {
        while (true)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 发送请求（需要响应）
    /// </summary>
    /// <param name="moduleId">模块ID</param>
    /// <param name="routerId">路由ID</param>
    /// <param name="messageBody">Protobuf序列化的消息体</param>
    /// <param name="onResponse">响应回调</param>
    /// <summary>
    /// 泛型发送请求（自动序列化Protobuf，回调返回IMessage）
    /// </summary>
    /// <typeparam name="TReq">请求消息类型（实现IMessage）</typeparam>
    /// <typeparam name="TRsp">响应消息类型（实现IMessage）</typeparam>
    public void SendRequest(uint moduleId, uint routerId, byte[] messageBody, Action<byte[]> onResponse)
    {
        if (!isConnected)
        {
            Debug.LogError("未连接到服务器");
            return;
        }


        reqIdCounter++;
        uint reqId = reqIdCounter;

        // 2. 注册回调：将TRsp（IMessage）包装成Action<IMessage>存入字典
        pendingRequests[reqId] = (rspMsg) => onResponse(rspMsg);


        // 3. 后续打包、发送逻辑不变
        byte[] packet = BuildReqPacket(moduleId, routerId, reqId, false, messageBody);
        byte[] frame = BuildFrame(packet);

        try
        {
            stream.Write(frame, 0, frame.Length);
            Debug.Log($"发送请求: ModuleId={moduleId}, RouterId={routerId}, ReqId={reqId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"发送请求失败: {e.Message}");
            pendingRequests.Remove(reqId);
        }
    }

    /// <summary>
    /// 发送事件（不需要响应）
    /// </summary>
    public void SendEvent(uint moduleId, uint routerId, byte[] messageBody)
    {
        if (!isConnected)
        {
            Debug.LogError("未连接到服务器");
            return;
        }

        reqIdCounter++;
        uint reqId = reqIdCounter;

        // 构建请求包（isOneWay=true）
        byte[] packet = BuildReqPacket(moduleId, routerId, reqId, true, messageBody);

        // 构建帧
        byte[] frame = BuildFrame(packet);

        try
        {
            stream.Write(frame, 0, frame.Length);
            Debug.Log($"发送事件: ModuleId={moduleId}, RouterId={routerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"发送事件失败: {e.Message}");
        }
    }

    /// <summary>
    /// 构建请求包
    /// </summary>
    private byte[] BuildReqPacket(uint moduleId, uint routerId, uint reqId, bool isOneWay, byte[] messageBody)
    {
        int packetSize = 13 + messageBody.Length; // RouterId(4) + ModuleId(4) + ReqId(4) + IsOneWay(1) + Body
        byte[] packet = new byte[packetSize];

        // RouterId (4 bytes, LittleEndian)
        BitConverter.GetBytes(routerId).CopyTo(packet, 0);

        // ModuleId (4 bytes, LittleEndian)
        BitConverter.GetBytes(moduleId).CopyTo(packet, 4);

        // ReqId (4 bytes, LittleEndian)
        BitConverter.GetBytes(reqId).CopyTo(packet, 8);

        // IsOneWay (1 byte)
        packet[12] = isOneWay ? (byte)1 : (byte)0;

        // MessageBody
        messageBody.CopyTo(packet, 13);

        return packet;
    }

    /// <summary>
    /// 构建帧（添加长度字段）
    /// </summary>
    private byte[] BuildFrame(byte[] packet)
    {
        uint frameLength = (uint)(4 + packet.Length); // 长度字段(4) + 数据包
        byte[] frame = new byte[frameLength];

        // 长度字段 (4 bytes, LittleEndian)
        BitConverter.GetBytes(frameLength).CopyTo(frame, 0);

        // 数据包
        packet.CopyTo(frame, 4);

        return frame;
    }


    /// <summary>
    /// 接收数据线程
    /// </summary>
    private void ReceiveData()
    {
        byte[] buffer = new byte[4096];
        List<byte> dataBuffer = new List<byte>();

        while (isConnected && tcpClient.Connected)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    // 连接断开
                    break;
                }

                // 添加到缓冲区
                for (int i = 0; i < bytesRead; i++)
                {
                    dataBuffer.Add(buffer[i]);
                }

                // 尝试解析完整帧
                while (dataBuffer.Count >= 4)
                {
                    // 读取长度字段
                    int frameLength = BitConverter.ToInt32(dataBuffer.ToArray(), 0);

                    if (frameLength == 0 || frameLength > 4096)
                    {
                        Debug.LogError($"无效的帧长度: {frameLength}");
                        isConnected = false;
                        break;
                    }

                    // 检查是否有完整帧
                    if (dataBuffer.Count < frameLength)
                    {
                        break; // 等待更多数据
                    }

                    // 提取完整帧
                    byte[] frame = new byte[frameLength];
                    for (int i = 0; i < frameLength; i++)
                    {
                        frame[i] = dataBuffer[i];
                    }

                    dataBuffer.RemoveRange(0, (int)frameLength);

                    // 解析响应包（去掉长度字段）
                    byte[] packetData = new byte[frameLength - 4];
                    Array.Copy(frame, 4, packetData, 0, packetData.Length);

                    ParseResponse(packetData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"接收数据错误: {e.Message}");
                break;
            }
        }

        isConnected = false;
        Debug.Log("与服务器断开连接");
    }

    /// <summary>
    /// 解析响应包
    /// </summary>
    private void ParseResponse(byte[] packetData)
    {
        if (packetData.Length < 12)
        {
            Debug.LogError("响应包太短");
            return;
        }

        // 解析响应包头
        uint routerId = (uint)BitConverter.ToInt32(packetData, 0);
        uint moduleId = (uint)BitConverter.ToInt32(packetData, 4);
        uint reqId = (uint)BitConverter.ToInt32(packetData, 8);

        // 提取消息体
        byte[] messageBody = new byte[packetData.Length - 12];
        Array.Copy(packetData, 12, messageBody, 0, messageBody.Length);

        Debug.Log($"收到响应: ModuleId={moduleId}, RouterId={routerId}, ReqId={reqId}");

        // 查找并调用回调
        if (pendingRequests.TryGetValue(reqId, out Action<byte[]> callback))
        {
            pendingRequests.Remove(reqId);
            Enqueue(() => callback(messageBody));
        }
        else
        {
            Debug.LogWarning($"未找到ReqId={reqId}的回调");
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    /// <summary>
    /// 心跳机制 - 定期发送心跳包保持连接
    /// </summary>
    private void KeepAlive()
    {
        while (isConnected && tcpClient != null && tcpClient.Connected)
        {
            try
            {
                // 发送心跳包（空消息体，isOneWay=true，silent=true不输出日志）
                SendEvent(InnerServiceModuleId, KeepAliveRouterId, new byte[0]);

                // 等待指定间隔
                Thread.Sleep((int)(keepAliveInterval * 1000));
            }
            catch (Exception e)
            {
                if (isConnected)
                {
                    Debug.LogWarning($"发送心跳失败: {e.Message}");
                }

                break;
            }
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        isConnected = false;

        // 等待心跳线程结束
        if (keepAliveThread != null && keepAliveThread.IsAlive)
        {
            keepAliveThread.Join(1000);
        }

        // 等待接收线程结束
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(1000);
        }

        if (stream != null)
        {
            stream.Close();
        }

        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }

    public void Enqueue(Action action)
    {
        actionQueue.Enqueue(action);
    }

    public void Update()
    {
        if (isLocal)
        {
            serverTimer++;
            Core.I.UpdateFrame();
        }
        else
        {
            while (actionQueue.Count > 0)
            {
                serverTimer++;
                actionQueue.Dequeue()?.Invoke();
                Core.I.UpdateFrame();
            }
        }
    }
}