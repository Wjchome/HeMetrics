using DG.Tweening;
using UnityEngine;

/// <summary>
/// 近战攻击行为 - 直接造成伤害
/// </summary>
public class MeleeAttackBehavior : IAttackBehavior
{
    public void ExecuteAttack(Character character, Character target)
    {
        if (target != null && !target.isDead)
        {
            target.Defend(character.attributeManager.GetFinalValue(character.attack, "Attack"));
        }
    }
    
    public void PlayAttackAnimation(Character character, Character target)
    {
        if (target == null) return;
        
        Vector2 position = character.currentCell.transform.position;
        character.transform.DOMove(
            Vector2.Lerp(position, target.currentCell.transform.position, 0.5f),
            character.attackInterval / 3)
            .OnComplete(() => { character.transform.DOMove(position, character.attackInterval / 3); });
    }
}

