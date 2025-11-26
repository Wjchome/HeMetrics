using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 属性记录 - 存储某个属性的所有来源和值
/// </summary>
public class AttributeRecord
{
    /// <summary>
    /// 属性来源列表 (来源名称, 属性值)
    /// </summary>
    public List<(string, int)> attributeRecords = new List<(string, int)>();
    
    /// <summary>
    /// 是否需要重新计算
    /// </summary>
    public bool isDirty = true;
    
    /// <summary>
    /// 缓存的最终值
    /// </summary>
    public int finalValue;
}

/// <summary>
/// 属性管理器 - 管理角色的所有属性加成
/// 支持显示属性的中间计算过程
/// </summary>
public class AttributeManager
{
    /// <summary>
    /// 属性字典 - 属性名称 -> 属性记录
    /// </summary>
    public Dictionary<string, AttributeRecord> attributeDic = new Dictionary<string, AttributeRecord>();

    /// <summary>
    /// 添加属性加成
    /// </summary>
    /// <param name="name">属性名称（如 "Attack", "Defence", "MaxHP"）</param>
    /// <param name="source">属性来源（如 "羁绊", "装备", "技能"）</param>
    /// <param name="value">属性值</param>
    public void Add(string name, string source, int value)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(source))
        {
            return;
        }

        if (!attributeDic.TryGetValue(name, out AttributeRecord attributeRecord))
        {
            attributeRecord = new AttributeRecord();
            attributeDic.Add(name, attributeRecord);
        }

        attributeRecord.attributeRecords.Add((source, value));
        attributeRecord.isDirty = true;
    }

    /// <summary>
    /// 获取最终属性值（带缓存）
    /// </summary>
    /// <param name="baseValue">基础值</param>
    /// <param name="name">属性名称</param>
    /// <returns>最终属性值</returns>
    public int GetFinalValue(int baseValue, string name)
    {
        if (!attributeDic.TryGetValue(name, out AttributeRecord attributeRecord))
        {
            return baseValue;
        }

        if (attributeRecord.isDirty)
        {
            int finalValue = baseValue;
            foreach (var (source, value) in attributeRecord.attributeRecords)
            {
                finalValue += value;
            }

            attributeRecord.finalValue = finalValue;
            attributeRecord.isDirty = false;
        }

        return attributeRecord.finalValue;
    }

    /// <summary>
    /// 移除指定来源的属性加成
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="source">属性来源</param>
    public void Remove(string name, string source)
    {
        if (!attributeDic.TryGetValue(name, out AttributeRecord attributeRecord))
        {
            return;
        }

        // 移除所有匹配的来源
        attributeRecord.attributeRecords.RemoveAll(r => r.Item1 == source);
        attributeRecord.isDirty = true;

        // 如果记录为空，可以移除（可选）
        if (attributeRecord.attributeRecords.Count == 0)
        {
            attributeDic.Remove(name);
        }
    }

    /// <summary>
    /// 移除指定前缀的所有属性加成（用于批量移除，如移除所有"羁绊"来源）
    /// </summary>
    /// <param name="sourcePrefix">来源前缀</param>
    public void RemoveBySourcePrefix(string sourcePrefix)
    {
        if (string.IsNullOrEmpty(sourcePrefix))
        {
            return;
        }

        var keysToRemove = new List<string>();
        foreach (var kvp in attributeDic)
        {
            var record = kvp.Value;
            record.attributeRecords.RemoveAll(r => r.Item1.StartsWith(sourcePrefix));
            record.isDirty = true;

            if (record.attributeRecords.Count == 0)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            attributeDic.Remove(key);
        }
    }

    /// <summary>
    /// 清除所有属性加成
    /// </summary>
    public void Clear()
    {
        attributeDic.Clear();
    }

    /// <summary>
    /// 检查是否有指定属性的记录
    /// </summary>
    public bool HasAttribute(string name)
    {
        return attributeDic.ContainsKey(name) && attributeDic[name].attributeRecords.Count > 0;
    }

    /// <summary>
    /// 获取属性记录（用于UI显示）
    /// </summary>
    public AttributeRecord GetAttributeRecord(string name)
    {
        attributeDic.TryGetValue(name, out AttributeRecord record);
        return record;
    }
}