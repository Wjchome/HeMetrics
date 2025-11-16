using FairyGUI;
using UnityEngine;

public class UIManager:MonoBehaviour
{
    public BondShowWin bondShow;
    
    // UI包的路径（相对于Resources文件夹，或者Assets路径）
    public string bondUIPackagePath = "Assets/FGUI/HeMetrics";  // 根据你的实际路径修改
    
    // UI包的名称（FairyGUI编辑器中设置的包名）
    public string bondUIPackageName = "HeMetrics";  // 根据你的实际包名修改
    
    // UI组件的名称（FairyGUI编辑器中设置的组件名）
    public string bondUIComponentName = "BondShowWin";
    
    public void Init()
    {
        // 方法1：直接使用 AddPackage + CreateObject(userClass) - 推荐方式
        CreateBondUIWithUserClass();
        
        // 方法2：先注册扩展，再创建（如果方法1不行，可以用这个）
        // CreateBondUIWithExtension();
    }
    
    /// <summary>
    /// 方法1：直接指定用户类创建（推荐）
    /// </summary>
    private void CreateBondUIWithUserClass()
    {
        // 1. 加载UI包（如果还没加载）
        UIPackage package = UIPackage.GetByName(bondUIPackageName);
        if (package == null)
        {
            // 包还没加载，需要先加载
            // 路径可以是相对于Resources的路径，或者Assets路径（仅编辑器）
            package = UIPackage.AddPackage(bondUIPackagePath);
            if (package == null)
            {
                Debug.LogError($"Failed to load UI package from path: {bondUIPackagePath}");
                return;
            }
        }
        
        // 2. 直接创建自定义类的对象（关键：传入 typeof(BondShowWin)）
        GObject obj = package.CreateObject(bondUIComponentName, typeof(BondShowWin));
        if (obj == null)
        {
            Debug.LogError($"Failed to create UI component: {bondUIComponentName}");
            return;
        }
        
        // 3. 转换为自定义类型
        bondShow = obj as BondShowWin;
        if (bondShow == null)
        {
            Debug.LogError($"Created object is not BondShowWin type. Got: {obj.GetType().Name}");
            return;
        }
        
        // 4. 初始化（必须在对象完全构造后调用）
        bondShow.Init();
        
        // 5. 添加到根节点显示
        GRoot.inst.AddChild(bondShow);
    }
    
    /// <summary>
    /// 方法2：先注册扩展，再创建（备选方案）
    /// </summary>
    private void CreateBondUIWithExtension()
    {
        // 1. 加载UI包
        UIPackage package = UIPackage.GetByName(bondUIPackageName);
        if (package == null)
        {
            package = UIPackage.AddPackage(bondUIPackagePath);
            if (package == null)
            {
                Debug.LogError($"Failed to load UI package from path: {bondUIPackagePath}");
                return;
            }
        }
        
        // 2. 注册扩展（格式: "ui://包名/组件名"）
        string url = $"ui://{bondUIPackageName}/{bondUIComponentName}";
        UIObjectFactory.SetPackageItemExtension(url, typeof(BondShowWin));
        
        // 3. 创建对象（会自动使用注册的扩展类）
        GObject obj = package.CreateObject(bondUIComponentName);
        if (obj == null)
        {
            Debug.LogError($"Failed to create UI component: {bondUIComponentName}");
            return;
        }
        
        // 4. 转换为自定义类型
        bondShow = obj as BondShowWin;
        if (bondShow == null)
        {
            Debug.LogError($"Created object is not BondShowWin type. Got: {obj.GetType().Name}");
            return;
        }
        
        // 5. 添加到根节点显示
        GRoot.inst.AddChild(bondShow);
    }
}

