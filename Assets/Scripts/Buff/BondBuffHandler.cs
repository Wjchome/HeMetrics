using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 羁绊Buff处理器 - 处理BondData中的BuffList，应用到角色
/// </summary>
public static class BondBuffHandler
{
    /// <summary>
    /// 应用羁绊Buff到角色
    /// </summary>
    /// <param name="character">角色</param>
    /// <param name="bondData">羁绊数据</param>
    /// <param name="bondLevel">羁绊等级（1-based）</param>
    public static void ApplyBondBuffs(Character character, BondData bondData, int bondLevel)
    {
        if (character == null || character.isDead || bondData == null)
        {
            return;
        }

        // 获取该等级对应的Buff列表（等级从1开始，索引从0开始）
        if (bondLevel <= 0 || bondLevel > bondData.BuffList.Count)
        {
            return;
        }
        
        var buffList = bondData.BuffList[bondLevel - 1];
        if (buffList == null || buffList.Count == 0)
        {
            return;
        }
        
        // 获取角色基础属性值（用于百分比计算）
        int baseAttack = character.data.Attack;
        int baseDefence = character.data.Defend;
        int baseMaxHP = character.data.MaxHP;
        int baseAttackInterval = (int)(character.data.AttackInterval * 1000); // 转换为毫秒
        int baseMoveInterval = (int)(character.data.MoveInterval * 1000); // 转换为毫秒
        
        // 应用每个Buff
        foreach (var buffData in buffList)
        {
            ApplySingleBuff(character, buffData, bondData.Name, bondLevel, 
                baseAttack, baseDefence, baseMaxHP, baseAttackInterval, baseMoveInterval);
        }
    }
    
    /// <summary>
    /// 移除角色的羁绊Buff
    /// </summary>
    public static void RemoveBondBuffs(Character character, BondData bondData)
    {
        if (character == null || character.attributeManager == null || bondData == null)
        {
            return;
        }
        
        // 移除所有该羁绊来源的Buff
        string sourcePrefix = $"羁绊_{bondData.Name}";
        character.attributeManager.RemoveBySourcePrefix(sourcePrefix);
    }
    
    /// <summary>
    /// 应用单个Buff
    /// </summary>
    private static void ApplySingleBuff(Character character, BuffData buffData, 
        string bondName, int bondLevel, 
        int baseAttack, int baseDefence, int baseMaxHP, 
        int baseAttackInterval, int baseMoveInterval)
    {
        if (buffData == null)
        {
            return;
        }
        
        // 获取属性名称
        string attributeName = BuffUseTypeConfig.GetAttributeName(buffData.UseType);
        if (string.IsNullOrEmpty(attributeName))
        {
            Debug.LogWarning($"未知的UseType: {buffData.UseType}");
            return;
        }
        
        // 获取基础值（用于百分比计算）
        int baseValue = GetBaseValue(buffData.UseType, baseAttack, baseDefence, baseMaxHP, 
            baseAttackInterval, baseMoveInterval);
        
        // 计算实际加成值
        int buffValue = BuffUseTypeConfig.CalculateBuffValue(buffData.UseType, buffData.Param1, baseValue);
        
        // 生成来源名称
        string sourceName = GenerateSourceName(bondName, bondLevel, buffData);
        
        // 添加到AttributeManager
        character.attributeManager.Add(attributeName, sourceName, buffValue);
        
        // 如果Param2是持续时间，可以在这里处理（暂时忽略，因为AttributeManager不支持临时Buff）
        // TODO: 如果需要临时Buff，可以在BuffManager中处理
    }
    
    /// <summary>
    /// 获取基础值（用于百分比计算）
    /// </summary>
    private static int GetBaseValue(int useType, int baseAttack, int baseDefence, int baseMaxHP, 
        int baseAttackInterval, int baseMoveInterval)
    {
        switch (useType)
        {
            case (int)BuffUseType.Attack:
            case (int)BuffUseType.AttackPercent:
                return baseAttack;
            case (int)BuffUseType.Defence:
            case (int)BuffUseType.DefencePercent:
                return baseDefence;
            case (int)BuffUseType.MaxHP:
            case (int)BuffUseType.MaxHPPercent:
                return baseMaxHP;
            case (int)BuffUseType.AttackInterval:
            case (int)BuffUseType.AttackIntervalPercent:
                return baseAttackInterval;
            case (int)BuffUseType.MoveInterval:
            case (int)BuffUseType.MoveIntervalPercent:
                return baseMoveInterval;
            default:
                return 0;
        }
    }
    
    /// <summary>
    /// 生成来源名称（用于UI显示）
    /// </summary>
    private static string GenerateSourceName(string bondName, int bondLevel, BuffData buffData)
    {
        string useTypeName = BuffUseTypeConfig.GetDisplayName(buffData.UseType);
        string valueStr = BuffUseTypeConfig.IsPercentType(buffData.UseType) 
            ? $"{buffData.Param1}%" 
            : buffData.Param1.ToString();
        
        return $"羁绊_{bondName}_Lv{bondLevel}_{useTypeName}({valueStr})";
    }
}
