using UnityEngine;

public class CharacterAttackState : FSMState<Character>
{
    public int lock_timer;

    public override void OnEnter()
    {
        lock_timer = 0;
    }
    public override void OnUpdate()
    {
        Owner.UpdateTarget();
        
        if (Owner.lastAttackFrame + Owner.attackIntervalFrame < Core.NetMgr.serverTimer)
        {
            lastAttackFrame = Core.NetMgr.serverTimer;
            realAttackFrame = Core.NetMgr.serverTimer + attackWindupFrame;
                
            Vector2 position = currentCell.transform.position;
            transform.DOMove(Vector2.Lerp(position, nearestEnemy.currentCell.transform.position, 0.5f),
                    attackInterval / 3)
                .OnComplete(() => { transform.DOMove(position, attackInterval / 3); });
        }

        if (Core.NetMgr.serverTimer == realAttackFrame)
        {
            Attack();
        }
    }
}