using UnityEngine;

/// <summary>
/// 攻击行为接口 - 不同角色有不同的攻击方式
/// </summary>
public interface IAttackBehavior
{
    /// <summary>
    /// 执行攻击
    /// </summary>
    /// <param name="character">攻击者</param>
    /// <param name="target">目标</param>
    void ExecuteAttack(Character character, Character target);
    
    /// <summary>
    /// 播放攻击动画/特效
    /// </summary>
    /// <param name="character">攻击者</param>
    /// <param name="target">目标</param>
    void PlayAttackAnimation(Character character, Character target);
}

