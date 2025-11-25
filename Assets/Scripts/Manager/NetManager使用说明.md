# NetManager 使用说明

## 快速开始

### 1. 设置 NetManager

1. 在 Unity 场景中创建一个 GameObject
2. 添加 `NetManager` 组件
3. 在 Inspector 中设置：
   - `Server Host`: 127.0.0.1
   - `Server Port`: 1024

### 2. 初始化连接

```csharp
NetManager netManager = GetComponent<NetManager>();
netManager.Init(); // 这会自动连接服务器
```

### 3. 发送测试请求

#### 测试1: TestReq/TestRsp

```csharp
// 创建 TestReq 消息（需要 Protobuf 生成的类）
var testReq = new TestReq
{
    Message = "Hello from Unity",
    Number = 42
};

// 序列化
byte[] messageBody = testReq.ToByteArray();

// 发送请求
netManager.SendRequest(
    moduleId: 1,      // TestModuleId
    routerId: 1,      // TestRouterId
    messageBody: messageBody,
    onResponse: (responseBytes) =>
    {
        // 反序列化响应
        var testRsp = TestRsp.Parser.ParseFrom(responseBytes);
        Debug.Log($"Result: {testRsp.Result}, Code: {testRsp.Code}");
    }
);
```

#### 测试2: EchoReq/EchoRsp

```csharp
// 创建 EchoReq 消息
var echoReq = new EchoReq
{
    Content = "Echo Test"
};

// 序列化
byte[] messageBody = echoReq.ToByteArray();

// 发送请求
netManager.SendRequest(
    moduleId: 1,      // TestModuleId
    routerId: 2,      // EchoRouterId
    messageBody: messageBody,
    onResponse: (responseBytes) =>
    {
        // 反序列化响应
        var echoRsp = EchoRsp.Parser.ParseFrom(responseBytes);
        Debug.Log($"Echo: {echoRsp.Content}");
    }
);
```

## 使用 NetManagerTest 脚本

### 方法1: 自动测试

1. 在场景中创建一个 GameObject
2. 添加 `NetManagerTest` 组件
3. 在 Inspector 中：
   - 拖拽 `NetManager` 组件到 `Net Manager` 字段
   - 勾选 `Auto Test On Start`
   - 设置 `Test Delay`（延迟时间，秒）

运行游戏后会自动发送两个测试请求。

### 方法2: 手动调用

```csharp
NetManagerTest test = GetComponent<NetManagerTest>();

// 测试 Test 请求
test.TestTestRequest();

// 测试 Echo 请求
test.TestEchoRequest();

// 或者测试所有请求
test.TestAllRequests();
```

## 路由ID说明

根据 Go 服务器定义：

| 模块 | 路由 | ModuleId | RouterId | 消息类型 |
|------|------|----------|----------|----------|
| 测试模块 | Test | 1 | 1 | TestReq/TestRsp |
| 测试模块 | Echo | 1 | 2 | EchoReq/EchoRsp |

## 注意事项

1. **Protobuf 序列化**：需要先编译 `.proto` 文件生成 C# 类
   - 如果还没有生成，`NetManagerTest` 提供了手动序列化的临时方案
   - 建议尽快使用 Protobuf 生成的类，更可靠

2. **线程安全**：响应回调会在 Unity 主线程执行（通过 `Enqueue`）

3. **连接状态**：确保在发送请求前已连接服务器

4. **错误处理**：所有异常都会在 Debug.LogError 中输出

## 生成 Protobuf C# 代码

如果还没有生成 Protobuf 代码，执行：

```bash
# 从项目根目录执行
protoc --csharp_out=./Assets/Scripts/Proto/ --proto_path=./cmd/proto/ ./cmd/proto/test.proto
```

这会生成 `Test.cs`，包含 `TestReq`、`TestRsp`、`EchoReq`、`EchoRsp` 类。



