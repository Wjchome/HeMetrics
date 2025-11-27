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
    private GList characterList;

    private Controller openCtrl;

    private Dictionary<BondType, List<Character>> myActiveBonds = new Dictionary<BondType, List<Character>>(); // 缓存的激活羁绊数据
    private List<BondType> myActiveBondList = new List<BondType>(); // 缓存的激活羁绊数据

    private Dictionary<BondType, List<Character>> enemyActiveBonds = new Dictionary<BondType, List<Character>>(); // 缓存的激活羁绊数据
    private List<BondType> enemyActiveBondList = new List<BondType>(); // 缓存的激活羁绊数据

    private List<Character> activeBondList = new List<Character>();
    
    public void Init()
    {
        list = GetChild("List") as GList;
        list1 = GetChild("List1") as GList;
        characterList = GetChild("List_character") as GList;
        openCtrl = GetController("Ctrl_open");

        list.itemRenderer = MyBondItemRender;
        list1.itemRenderer = EnemyBondItemRender;
        
        characterList.itemRenderer = CharacterItemRenderer;
    }

    private void CharacterItemRenderer(int index, GObject item)
    {
        GTextField characterNameText = item.asCom.GetChild("Txt_characterName") as GTextField;
        characterNameText.text = activeBondList[index].data.Name;
    }

    public void UpdateBondList(Dictionary<BondType, List<Character>> myActiveBonds, Dictionary<BondType, List<Character>> enemyActiveBonds)
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

    private void MyBondItemRender(int index, GObject item)
    {
        BondType bondType = myActiveBondList[index];
        int currentCount = myActiveBonds[bondType].Count;
        BondData bondData = Core.dataMgr.BondData()[bondType];
        StringBuilder sb = new StringBuilder();
        foreach (var level in bondData.Level)
        {
            sb.Append(level);
            if (level > currentCount)
            {
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
        
        
        item.onClick.Add(() =>
        {
            openCtrl.selectedIndex = 1;
            activeBondList = myActiveBonds[bondType];
            characterList.numItems = activeBondList.Count;
        });
    }

    private void EnemyBondItemRender(int index, GObject item)
    {
        BondType bondType = enemyActiveBondList[index];
        int currentCount = enemyActiveBonds[bondType].Count;

        BondData bondData = Core.dataMgr.BondData()[bondType];
        StringBuilder sb = new StringBuilder();
        foreach (var level in bondData.Level)
        {
            sb.Append(level);
            if (level > currentCount)
            {
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
        
        item.onClick.Add(() =>
        {
            openCtrl.selectedIndex = 1;
            activeBondList = enemyActiveBonds[bondType];
            characterList.numItems = activeBondList.Count;
        });
    }


}