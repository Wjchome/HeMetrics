using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class BondUI:GComponent
{
  //  private GComponent ui;
    private GList list;
    private List<BondType> bondTypeList = new List<BondType>(); // 当前显示的羁绊类型列表
    private Dictionary<BondType, int> cachedActiveBonds = new Dictionary<BondType, int>(); // 缓存的激活羁绊数据


    
    public BondUI()
    {
        Init();
    }

    public void Init()
    {
        list =GetChild("List") as GList;
        if (list != null)
        {
            list.itemRenderer = RenderBondItem;
        }
    }

    public void UpdateBondList(Dictionary<BondType, int> activeBonds)
    {
        // 缓存激活的羁绊数据
        cachedActiveBonds.Clear();
        foreach (var kvp in activeBonds)
        {
            cachedActiveBonds[kvp.Key] = kvp.Value;
        }

        bondTypeList.Clear();
        bondTypeList.AddRange(activeBonds.Keys);

        // 更新列表数量
        list.numItems = bondTypeList.Count;
    }

    /// <summary>
    /// 渲染单个羁绊项
    /// </summary>
    private void RenderBondItem(int index, GObject item)
    {
        if (index >= bondTypeList.Count)
            return;

        BondType bondType = bondTypeList[index];
        int currentCount = cachedActiveBonds.TryGetValue(bondType, out int count) ? count : 0;
        int totalCount = Core.bondMgr.GetBondConfigCount(bondType);

        // 获取羁绊名称
        string bondName = GetBondName(bondType);

        // 假设UI项有 "Name" 和 "Count" 两个子组件
        // 根据实际UI结构调整，如果UI组件名称不同，请修改这里的名称
        GTextField nameText = item.asCom.GetChild("Txt_name") as GTextField;
        GTextField countText = item.asCom.GetChild("Txt_count") as GTextField;


        nameText.text = bondName;

        countText.text = $"{currentCount}/{totalCount}";
    }

    /// <summary>
    /// 获取羁绊名称
    /// </summary>
    private string GetBondName(BondType bondType)
    {
        switch (bondType)
        {
            case BondType.Attack:
                return "攻击";
            case BondType.Defend:
                return "防御";
            case BondType.Move:
                return "移动";
            case BondType.HP:
                return "生命";
            default:
                return bondType.ToString();
        }
    }
}