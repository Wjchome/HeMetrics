using DG.Tweening;
using UnityEngine;

public class CharacterAttackState : FSMState<Character>
{
    private long _realAttackFrame; // 实际攻击判定帧（仅攻击状态内使用）
    private long _exitAttackFrame; 

    public override void OnEnter()
    {
        Owner.lastAttackFrame = Core.NetMgr.serverTimer;
        _realAttackFrame = Core.NetMgr.serverTimer + Owner.attackWindupFrame;
        _exitAttackFrame = Core.NetMgr.serverTimer + Owner.attackIntervalFrame;

        Vector2 position = Owner.currentCell.transform.position;
        Owner.transform.DOMove(Vector2.Lerp(position, Owner.nearestEnemy.currentCell.transform.position, 0.5f),
                Owner.attackInterval / 3)
            .OnComplete(() => { Owner.transform.DOMove(position, Owner.attackInterval / 3); });
    }

    public override void OnUpdate()
    {
        if (Core.NetMgr.serverTimer == _realAttackFrame)
        {
            Owner.Attack();
        }

        if (Core.NetMgr.serverTimer == _exitAttackFrame)
        {
            Owner.fsm.ChangeState(CharacterState.Idle);
        }
    }
}