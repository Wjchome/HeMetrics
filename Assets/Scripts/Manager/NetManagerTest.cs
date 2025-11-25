using System;
using Google.Protobuf;
using UnityEngine;
using Testpb; // Protobuf 生成的命名空间

/// <summary>
/// NetManager 测试脚本 - 测试两个请求
/// </summary>
public class NetManagerTest : MonoBehaviour
{
    [Header("网络管理器")] public NetManager netManager;

    [Header("测试配置")] public bool autoTestOnStart = false;
    public float testDelay = 1f;

    private void Start()
    {
        if (autoTestOnStart)
        {
            Invoke(nameof(TestAllRequests), testDelay);
        }
    }

    /// <summary>
    /// 测试所有请求
    /// </summary>
    public void TestAllRequests()
    {
        Debug.Log("========== 开始测试网络请求 ==========");

        // 测试1: Test请求
        TestTestRequest();

        // 延迟后测试2: Echo请求
        Invoke(nameof(TestEchoRequest), 1f);
    }

    /// <summary>
    /// 测试1: TestReq/TestRsp 请求
    /// ModuleId=1, RouterId=1
    /// </summary>
    public void TestTestRequest()
    {
        Debug.Log("--- 发送 TestReq 请求 ---");

        // 创建 TestReq 消息
        // 注意：需要先编译 proto 文件生成 C# 类
        // 如果还没有生成，请参考下面的手动序列化方法

        try
        {

            var testReq = new TestReq
            {
                Message = "Hello from Unity",
                Number = 42
            };
            byte[] messageBody = testReq.ToByteArray();


            // 发送请求
            netManager.SendRequest(
                moduleId: 1, // TestModuleId
                routerId: 1, // TestRouterId
                messageBody: messageBody,
                onResponse: (responseBytes) =>
                {
                    Debug.Log("--- 收到 TestRsp 响应 ---");

                    try
                    {

                        // 使用 Protobuf 反序列化
                        var testRsp = TestRsp.Parser.ParseFrom(responseBytes);
                        Debug.Log($"✅ Test响应成功!");
                        Debug.Log($"   Result: {testRsp.Result}");
                        Debug.Log($"   Code: {testRsp.Code}");

                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"❌ 解析TestRsp失败: {e.Message}");
                    }
                }
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ 发送TestReq失败: {e.Message}");
        }
    }

    /// <summary>
    /// 测试2: EchoReq/EchoRsp 请求
    /// ModuleId=1, RouterId=2
    /// </summary>
    public void TestEchoRequest()
    {
        Debug.Log("--- 发送 EchoReq 请求 ---");

        try
        {

            var echoReq = new EchoReq
            {
                Content = "Echo Test from Unity"
            };
            byte[] messageBody = echoReq.ToByteArray();


            // 发送请求
            netManager.SendRequest(
                moduleId: 1, // TestModuleId
                routerId: 2, // EchoRouterId
                messageBody: messageBody,
                onResponse: (responseBytes) =>
                {
                    Debug.Log("--- 收到 EchoRsp 响应 ---");

                    try
                    {
                        // 使用 Protobuf 反序列化
                        var echoRsp = EchoRsp.Parser.ParseFrom(responseBytes);
                        Debug.Log($"✅ Echo响应成功!");
                        Debug.Log($"   Content: {echoRsp.Content}");

                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"❌ 解析EchoRsp失败: {e.Message}");
                    }
                }
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ 发送EchoReq失败: {e.Message}");
        }
    }





}

