using FairyGUI;
using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // 存储实现IUIComponent接口的UI实例（类型安全）
    private Dictionary<System.Type, IUIComponent> uiInstances = new Dictionary<System.Type, IUIComponent>();

    // UI包配置（保持不变）
    public string bondUIPackagePath = "Assets/FGUI/HeMetrics";
    public string bondUIPackageName = "HeMetrics";

    // UI组件配置（简化为名称数组，或保留类型但约束为IUIComponent）
    public (string nameInFGUI, System.Type type)[] bondUIComponentName =
    {
        ("BondShowWin", typeof(BondShowWin)), 
        ("CharacterClickWin", typeof(CharacterClickWin))
    };

    public void Init()
    {
        CreateBondUIWithUserClass();
    }

    private void CreateBondUIWithUserClass()
    {
        // 加载UI包（保持不变）
        UIPackage package = UIPackage.GetByName(bondUIPackageName);
        if (package == null)
        {
            package = UIPackage.AddPackage(bondUIPackagePath);
            if (package == null)
            {
                Debug.LogError($"Failed to load UI package: {bondUIPackagePath}");
                return;
            }
        }

        foreach (var (nameInFGUI, type) in bondUIComponentName)
        {
            // 1. 创建UI组件实例
            GObject obj = package.CreateObject(nameInFGUI, type);
            if (obj == null)
            {
                Debug.LogError($"Failed to create UI component: {nameInFGUI}");
                continue;
            }

            // 2. 转换为IUIComponent接口类型（类型安全）
            if (obj is IUIComponent uiComponent)
            {
                // 3. 直接调用Init方法（无反射，高性能）
                uiComponent.Init();

                // 4. 存储实例（接口类型，可统一管理）
                uiInstances[type] = uiComponent;

                // 5. 添加到根节点显示（转换为GComponent）
                if (uiComponent is GComponent component)
                {
                    GRoot.inst.AddChild(component);
                }

                Debug.Log($"Successfully initialized UI: {nameInFGUI}");
            }
            else
            {
                Debug.LogError($"UI component {nameInFGUI} does not implement IUIComponent!");
            }
        }
    }

    // 泛型获取方法（类型安全，直接返回目标类型）
    public T GetUI<T>() where T : class, IUIComponent
    {
        return (T)uiInstances[typeof(T)];
    }
    
}