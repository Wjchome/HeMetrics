using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 羁绊Buff处理器 - 将BondData转换为Buff实例，添加到BuffManager
/// 职责：Bond配置 -> Buff实例
/// </summary>
public static class BondBuffHandler
{
    /// <summary>
    /// 应用羁绊Buff到角色（通过BuffManager）
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
        
        if (bondLevel <= 0 || bondLevel > bondData.BuffList.Count)
        {
            return;
        }
        List<BuffData> buffList = bondData.BuffList[bondLevel - 1];
        
        // 生成来源名称
        string source = $"羁绊_{bondData.Name}_Lv{bondLevel}";
        
        // 为每个BuffData创建Buff实例并添加到BuffManager
        foreach (BuffData buffData in buffList)
        {
            if (buffData != null)
            {
                character.buffManager.AddBuff(buffData, source);
            }
        }
    }
}
