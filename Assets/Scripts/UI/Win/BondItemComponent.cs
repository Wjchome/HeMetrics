using FairyGUI;
using UnityEngine;

/// <summary>
/// 羁绊项组件 - 封装GList子项的UI组件引用
/// 继承GComponent，在ConstructFromResource中初始化子组件
/// </summary>
public class BondItemComponent : GComponent
{
    public GTextField nameText;
    public GTextField countText;
    public GTextField levelsText;
    
    public override void ConstructFromResource()
    {
        base.ConstructFromResource();
        
        // 在构造时初始化所有子组件引用（只调用一次）
        nameText = GetChild("Txt_name") as GTextField;
        countText = GetChild("Txt_count") as GTextField;
        levelsText = GetChild("Txt_levels") as GTextField;
    }
    
    /// <summary>
    /// 更新显示内容
    /// </summary>
    public void UpdateContent(string name, int count, string levels)
    {
        if (nameText != null) nameText.text = name;
        if (countText != null) countText.text = count.ToString();
        if (levelsText != null) levelsText.text = levels;
    }
}