using System.Collections.Generic;
using System.Linq;
using System.Text;
using FairyGUI;
using UnityEngine;

public class CharacterClickWin: GComponent,IUIComponent
{
    //  private GComponent ui;
    private GList showList;
    private GList bondList;
    private GTextField characterTxt;
    private Character target;
    private Info  info;
    
    public class Info
    {
        public List<string> names=new List<string>();
        public List<AttributeRecord> attributes=new List<AttributeRecord>();
    }
    public void Init()
    {
        
        showList = GetChild("List_show") as GList;
        bondList = GetChild("List_bond") as GList;
        characterTxt = GetChild("Txt_character") as GTextField;
        
        showList.itemRenderer = showListRenderItem;
        bondList.itemRenderer = bondListRenderItem;
        
    }


    public void ShowContent(Character character, AttributeManager  data)
    {
        if (data == null)
        {
            return;
        }
        visible = true; 
        target = character;
        info = new Info();
        foreach (KeyValuePair<string, AttributeRecord> item in data.attributeDic)
        {
            info.names.Add(item.Key);
            info.attributes.Add(item.Value);
        }
        
        characterTxt.text = character.data.Name;
        showList.numItems = data.attributeDic.Count;
        bondList.numItems = character.data.BondList.Count;

    }

    public void HideContent()
    {
        visible = false;
    }


    private void showListRenderItem(int index, GObject item)
    {
        if(info==null || index < 0 || index >= info.names.Count)
        {
            return;
        }
        
        GTextField nameTxt = item.asCom.GetChild("Txt_name")as GTextField;
        GList _showList = item.asCom.GetChild("List_show") as GList;
        GTextField valueTxt = item.asCom.GetChild("Txt_value")as GTextField;
        
        string attributeName = info.names[index];
        AttributeRecord attributeRecord = info.attributes[index];
        
        // 显示属性名称
        nameTxt.text = GetAttributeDisplayName(attributeName);
        
        // 设置子列表的渲染器（显示所有来源和值）
        _showList.itemRenderer = (i, o) =>
        {
            if (i < 0 || i >= attributeRecord.attributeRecords.Count)
            {
                return;
            }
            
            GTextField sourceNameTxt = o.asCom.GetChild("Txt_name") as GTextField;
            GTextField sourceValueTxt = o.asCom.GetChild("Txt_value") as GTextField;
            
            var (source, value) = attributeRecord.attributeRecords[i];
            sourceNameTxt.text = source;
            
            // 根据属性类型格式化显示值
            string displayValue = FormatAttributeValue(attributeName, value);
            sourceValueTxt.text = displayValue;
        };
        
        // 获取基础值并计算最终值
        int baseValue = target.GetBaseAttributeValue(attributeName);
        int finalValue = target.attributeManager.GetFinalValue(baseValue, attributeName);
        
        // 显示最终值（根据属性类型格式化）
        valueTxt.text = FormatAttributeValue(attributeName, finalValue);
        
        // 设置子列表数量
        _showList.numItems = attributeRecord.attributeRecords.Count;
    }
    
    /// <summary>
    /// 获取属性的显示名称
    /// </summary>
    private string GetAttributeDisplayName(string attributeName)
    {
        switch (attributeName)
        {
            case "Attack":
                return "攻击力";
            case "Defence":
                return "防御力";
            case "MaxHP":
                return "最大生命值";
            case "MoveInterval":
                return "移动间隔";
            case "AttackInterval":
                return "攻击间隔";
            default:
                return attributeName;
        }
    }
    
    /// <summary>
    /// 格式化属性值显示
    /// </summary>
    private string FormatAttributeValue(string attributeName, int value)
    {
        switch (attributeName)
        {
            case "MoveInterval":
            case "AttackInterval":
                // 时间间隔显示为秒（保留2位小数）
                float seconds = value / 1000f;
                return seconds.ToString("F2") + "秒";
            default:
                return value.ToString();
        }
    }



    public bool isSame(Character character)
    {
        return character == target;
    }


    private void bondListRenderItem(int index, GObject item)
    {
        if (target==null)
        {
            return;
        }
        
        GTextField bondNameTxt = item.asCom.GetChild("Txt_bondName")as GTextField;
        bondNameTxt.text = target.data.BondList[index].ToString();
        
    }

}