using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// Unity TCP客户端 - 用于连接Go服务器
/// </summary>
public class NetManager : MonoBehaviour
{
    private TcpClient tcpClient; //tcp连接
    private NetworkStream stream; //写流
    private Thread receiveThread; //听线程
    private bool isConnected = false; //是否连接
    private uint reqIdCounter = 0; //一个消息id计数器
    private Dictionary<uint, Action<byte[]>> pendingRequests = new Dictionary<uint, Action<byte[]>>();
    private Queue<Action> actionQueue = new Queue<Action>();

    [Header("服务器配置")] public string serverHost = "127.0.0.1";
    public int serverPort = 1024;

    public int serverTimer = 0;
    public void Init()
    {
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

            Debug.Log($"已连接到服务器 {serverHost}:{serverPort}");
        }
        catch (Exception e)
        {
            Debug.LogError($"连接失败: {e.Message}");
            isConnected = false;
        }
    }

    /// <summary>
    /// 发送请求（需要响应）
    /// </summary>
    /// <param name="moduleId">模块ID</param>
    /// <param name="routerId">路由ID</param>
    /// <param name="messageBody">Protobuf序列化的消息体</param>
    /// <param name="onResponse">响应回调</param>
    public void SendRequest(uint moduleId, uint routerId, byte[] messageBody, Action<byte[]> onResponse)
    {
        if (!isConnected)
        {
            Debug.LogError("未连接到服务器");
            return;
        }

        reqIdCounter++;
        uint reqId = reqIdCounter;

        // 注册响应回调

        pendingRequests[reqId] = onResponse;


        // 构建请求包
        byte[] packet = BuildReqPacket(moduleId, routerId, reqId, false, messageBody);

        // 构建帧（添加长度字段）
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
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        isConnected = false;

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
        while (actionQueue.Count > 0)
        {
            serverTimer++;
            actionQueue.Dequeue()?.Invoke();
            Core.I.UpdateFrame();
        }
    }
}