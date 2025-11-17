using UnityEngine;

/// <summary>
/// 技能接口 - 用于实现角色的特殊技能
/// </summary>
public interface ISkill
{
    /// <summary>
    /// 技能名称
    /// </summary>
    string SkillName { get; }
    
    /// <summary>
    /// 技能冷却时间（帧数）
    /// </summary>
    int CooldownFrames { get; }
    
    /// <summary>
    /// 执行技能
    /// </summary>
    /// <param name="character">技能释放者</param>
    /// <param name="targets">目标列表</param>
    void Execute(Character character, Character[] targets);
    
    /// <summary>
    /// 检查技能是否可以释放
    /// </summary>
    /// <param name="character">技能释放者</param>
    /// <returns>是否可以释放</returns>
    bool CanCast(Character character);
}

