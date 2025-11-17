using DG.Tweening;
using UnityEngine;

/// <summary>
/// 远程攻击行为 - 发射子弹
/// </summary>
public class RangedAttackBehavior : IAttackBehavior
{
    public void ExecuteAttack(Character character, Character target)
    {
        if (target == null || target.isDead) return;
        
        int distance = Core.HexMapMgr.GetHexDistance(
            character.currentCell as HexCell, 
            target.currentCell as HexCell);
        
        var bullet = Object.Instantiate(Core.I.bulletPrefab, character.transform.position, Quaternion.identity);
        Core.BulletMgr.AddBullet(bullet);
        bullet.Init(character.attack, 0.4f * distance, target);
    }
    
    public void PlayAttackAnimation(Character character, Character target)
    {
        if (target == null) return;
        
        Vector3 position = character.currentCell.transform.position;
        character.transform.DOMove(
            position + 0.2f * (position - target.currentCell.transform.position).normalized,
            character.attackInterval / 3)
            .OnComplete(() => { character.transform.DOMove(position, character.attackInterval / 5); })
            .SetLink(character.gameObject, LinkBehaviour.KillOnDestroy);
    }
}

