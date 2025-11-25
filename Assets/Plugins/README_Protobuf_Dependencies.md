# Protobuf 依赖说明

## 问题
使用 Google.Protobuf 时，需要以下依赖：
- System.Runtime.CompilerServices.Unsafe
- System.Memory (可能也需要)

## 解决方案

### 方法1：使用 NuGet for Unity（推荐）

1. 安装 NuGet for Unity：
   - 下载：https://github.com/GlitchEnzo/NuGetForUnity/releases
   - 导入到 Unity

2. 通过 NuGet 安装依赖：
   - `NuGet` → `Manage NuGet Packages`
   - 搜索并安装：
     - `System.Runtime.CompilerServices.Unsafe` (版本 4.5.3 或更高)
     - `System.Memory` (如果需要)

### 方法2：手动下载 DLL

1. 访问 NuGet 网站：
   - https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/

2. 下载 `.nupkg` 文件（实际是 ZIP）

3. 解压后找到：
   - `lib/netstandard2.0/System.Runtime.CompilerServices.Unsafe.dll`
   - 或 `lib/netstandard1.0/System.Runtime.CompilerServices.Unsafe.dll`

4. 复制到 `Assets/Plugins/` 文件夹

5. 在 Unity 中选中 DLL，在 Inspector 中：
   - 确保勾选正确的平台
   - 如果需要，取消勾选 WebGL（某些版本不支持）

### 方法3：使用 Unity Package Manager

如果项目支持，可以通过 manifest.json 添加：

```json
{
  "dependencies": {
    "com.unity.nuget.system-runtime-compiler-services-unsafe": "1.0.0"
  }
}
```

## 验证

安装后，重新编译项目，错误应该消失。



