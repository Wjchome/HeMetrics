using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Buff管理器 - 管理角色身上的所有Buff实例
/// 职责：添加/移除Buff，更新Buff状态，管理Buff生命周期
/// </summary>
public class BuffManager
{
    private Character character;
    private Dictionary<string, IBuff> buffs = new Dictionary<string, IBuff>(); // BuffId -> Buff实例
    
    public BuffManager(Character character)
    {
        this.character = character;
    }
    
    /// <summary>
    /// 添加Buff
    /// </summary>
    public void AddBuff(BuffData buffData, string source)
    {
        
        // 创建Buff实例
        IBuff buff = new BuffInstance(buffData, character, source);
        string buffId = buff.BuffId;
        
        // 如果已存在相同ID的Buff，先移除旧的
        if (buffs.ContainsKey(buffId))
        {
            RemoveBuff(buffId);
        }
        
        // 添加新Buff
        buffs[buffId] = buff;
        
        // 立即应用到AttributeManager（如果是激活状态）
        if (buff.IsActive(character))
        {
            buff.ApplyToAttributeManager(character);
        }
    }
    
    /// <summary>
    /// 移除Buff
    /// </summary>
    public void RemoveBuff(string buffId)
    {
        if (!buffs.TryGetValue(buffId, out IBuff buff))
        {
            return;
        }
        
        // 从AttributeManager移除
        buff.RemoveFromAttributeManager(character);
        
        buffs.Remove(buffId);
    }
    
    /// <summary>
    /// 移除指定来源的所有Buff
    /// </summary>
    public void RemoveBuffsBySource(string sourcePrefix)
    {
        var buffsToRemove = buffs.Values
            .Where(b => b.Source.StartsWith(sourcePrefix))
            .Select(b => b.BuffId)
            .ToList();
        
        foreach (var buffId in buffsToRemove)
        {
            RemoveBuff(buffId);
        }
    }
    
    /// <summary>
    /// 检查是否有指定ID的Buff
    /// </summary>
    public bool HasBuff(string buffId)
    {
        return buffs.ContainsKey(buffId) && !buffs[buffId].IsExpired();
    }
    
    /// <summary>
    /// 获取所有Buff
    /// </summary>
    public List<IBuff> GetAllBuffs()
    {
        return buffs.Values.Where(b => !b.IsExpired()).ToList();
    }
    
    /// <summary>
    /// 更新所有Buff（每帧调用）
    /// </summary>
    public void UpdateFrame()
    {
        // 检查过期的Buff
        var expiredBuffs = buffs.Values
            .Where(b => b.IsExpired())
            .Select(b => b.BuffId)
            .ToList();
        
        foreach (var buffId in expiredBuffs)
        {
            RemoveBuff(buffId);
        }
        
        // 更新所有Buff（条件Buff需要每帧检查）
        foreach (var buff in buffs.Values)
        {
            buff.Update(character);
        }
    }
    
    /// <summary>
    /// 清除所有Buff
    /// </summary>
    public void Clear()
    {
        var allBuffIds = buffs.Keys.ToList();
        foreach (var buffId in allBuffIds)
        {
            RemoveBuff(buffId);
        }
    }
}
