using System.Collections.Generic;
using System.Linq;
using System.Text;
using FairyGUI;
using UnityEngine;

public class BondShowWin : GComponent, IUIComponent
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
        BondData bondData = Core.dataMgr.BondData()[bondType];
        StringBuilder sb = new StringBuilder();
        foreach (var level in bondData.Level)
        {
            sb.Append(level);
            if (level > currentCount)
            {
                currentCount = level;
                break;
            }

            sb.Append("/");
        }


        // 获取羁绊名称
        string bondName = Core.dataMgr.BondData()[bondType].Name;

        GTextField nameText = item.asCom.GetChild("Txt_name") as GTextField;
        GTextField countText = item.asCom.GetChild("Txt_count") as GTextField;
        GTextField levelsText = item.asCom.GetChild("Txt_levels") as GTextField;

        nameText.text = bondName;
        countText.text = currentCount.ToString();
        levelsText.text = sb.ToString();
    }

    private void RenderEnemyBondItem(int index, GObject item)
    {
        BondType bondType = enemyActiveBondList[index];
        int currentCount = enemyActiveBonds.TryGetValue(bondType, out int count) ? count : 0;

        BondData bondData = Core.dataMgr.BondData()[bondType];
        StringBuilder sb = new StringBuilder();
        foreach (var level in bondData.Level)
        {
            sb.Append(level);
            if (level > currentCount)
            {
                currentCount = level;
                break;
            }

            sb.Append("/");
        }

        GTextField nameText = item.asCom.GetChild("Txt_name") as GTextField;
        GTextField countText = item.asCom.GetChild("Txt_count") as GTextField;
        GTextField levelsText = item.asCom.GetChild("Txt_levels") as GTextField;


        nameText.text = bondData.Name;
        countText.text = currentCount.ToString();
        levelsText.text = sb.ToString();
    }


}