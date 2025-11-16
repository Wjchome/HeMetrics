using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using UnityEngine;

public class BondShowWin : GComponent
{
    //  private GComponent ui;
    private GList list;
    private GList list1;

    private Dictionary<BondType, int> myActiveBonds = new Dictionary<BondType, int>(); // 缓存的激活羁绊数据
    private List<BondType> myActiveBondList = new List<BondType>(); // 缓存的激活羁绊数据

    private Dictionary<BondType, int> enemyActiveBonds = new Dictionary<BondType, int>(); // 缓存的激活羁绊数据
    private List<BondType> enemyActiveBondList = new List<BondType>(); // 缓存的激活羁绊数据

    public void Init()
    {
        list = GetChild("List") as GList;
        list1 = GetChild("List1") as GList;

        list.itemRenderer = RenderMyBondItem;
        list1.itemRenderer = RenderEnemyBondItem;
    }

    public void UpdateBondList(Dictionary<BondType, int> myActiveBonds, Dictionary<BondType, int> enemyActiveBonds)
    {
        // 缓存激活的羁绊数据
        this.myActiveBonds = myActiveBonds;
        this.enemyActiveBonds = enemyActiveBonds;
        myActiveBondList = this.myActiveBonds.Keys.ToList();
        enemyActiveBondList = this.enemyActiveBonds.Keys.ToList();

        // 更新列表数量
        list.numItems = this.myActiveBonds.Count;
        list1.numItems = this.enemyActiveBonds.Count;
    }

    /// <summary>
    /// 渲染单个羁绊项
    /// </summary>
    private void RenderMyBondItem(int index, GObject item)
    {
        BondType bondType = myActiveBondList[index];
        int currentCount = myActiveBonds.TryGetValue(bondType, out int count) ? count : 0;
        int totalCount = Core.bondMgr.GetBondConfigCount(bondType);

        // 获取羁绊名称
        string bondName = GetBondName(bondType);

        GTextField nameText = item.asCom.GetChild("Txt_name") as GTextField;
        GTextField countText = item.asCom.GetChild("Txt_count") as GTextField;

        nameText.text = bondName;
        countText.text = $"{currentCount}/{totalCount}";
    }

    private void RenderEnemyBondItem(int index, GObject item)
    {
        BondType bondType = enemyActiveBondList[index];
        int currentCount = enemyActiveBonds.TryGetValue(bondType, out int count) ? count : 0;
        int totalCount = Core.bondMgr.GetBondConfigCount(bondType);

        // 获取羁绊名称
        string bondName = GetBondName(bondType);

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