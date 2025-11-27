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

    private Dictionary<BondType, List<int>> myActiveBonds = new Dictionary<BondType, List<int>>(); // 缓存的激活羁绊数据
    private List<BondType> myActiveBondList = new List<BondType>(); // 缓存的激活羁绊数据

    private Dictionary<BondType, List<int>> enemyActiveBonds = new Dictionary<BondType, List<int>>(); // 缓存的激活羁绊数据
    private List<BondType> enemyActiveBondList = new List<BondType>(); // 缓存的激活羁绊数据

    private BondType clickBondType;
    private List<int> activeBondList = new List<int>();

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
        List<int> ids = Core.bondMgr.GetAllIdFromBond(clickBondType);
     
            characterNameText.text = Core.dataMgr.CharacterData()[ids[index]].Name;
            
            if (activeBondList.Contains(ids[index]))
            {
                characterNameText.color = Color.yellow;
            }
            else
            {
                characterNameText.color = Color.gray;

            }
        
    }

    public void UpdateBondList(Dictionary<BondType, List<int>> myActiveBonds,
        Dictionary<BondType, List<int>> enemyActiveBonds)
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

        BondItem bondItem = item as BondItem;
        bondItem.nameText.text = bondData.Name;
        bondItem.countText.text = currentCount.ToString();
        bondItem.levelsText.text = sb.ToString();

        // 清除之前的点击事件，避免重复添加
        item.onClick.Clear();
        item.onClick.Add(() =>
        {
            openCtrl.selectedIndex = 1;
            activeBondList = myActiveBonds[bondType];
            clickBondType = bondType;
            characterList.numItems = Core.bondMgr.GetAllIdFromBond(clickBondType).Count;
        });
    }

    private void EnemyBondItemRender(int index, GObject item)
    {
        BondType bondType = enemyActiveBondList[index];
        int currentCount = enemyActiveBonds[bondType].Count;

        BondData bondData = Core.dataMgr.BondData()[bondType];
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bondData.Level.Count; i++)
        {
            var level = bondData.Level[i];
            sb.Append(level);
            if (level > currentCount)
            {
                break;
            }
            //最后一个特殊处理
            if (i != bondData.Level.Count - 1)
            {
                sb.Append("/");
            }
        }

        BondItem bondItem = item as BondItem;
        bondItem.nameText.text = bondData.Name;
        bondItem.countText.text = currentCount.ToString();
        bondItem.levelsText.text = sb.ToString();


        // 清除之前的点击事件，避免重复添加
        item.onClick.Clear();
        item.onClick.Add(() =>
        {
            openCtrl.selectedIndex = 1;
            activeBondList = enemyActiveBonds[bondType];
            clickBondType = bondType;
            characterList.numItems = Core.bondMgr.GetAllIdFromBond(clickBondType).Count;
        });
    }
}