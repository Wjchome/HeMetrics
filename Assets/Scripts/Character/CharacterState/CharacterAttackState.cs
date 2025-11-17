using UnityEngine;

/// <summary>
/// 攻击状态 - 使用攻击行为组件执行攻击
/// </summary>
public class CharacterAttackState : FSMState<Character>
{
    private long _realAttackFrame; // 实际攻击判定帧（仅攻击状态内使用）
    private long _exitAttackFrame; 

    public override void OnEnter()
    {
        Owner.lastAttackFrame = Core.NetMgr.serverTimer;
        _realAttackFrame = Core.NetMgr.serverTimer + Owner.attackWindupFrame;
        _exitAttackFrame = Core.NetMgr.serverTimer + Owner.attackIntervalFrame;

        // 播放攻击动画（使用攻击行为）
        if (Owner.nearestEnemy != null && Owner.attackBehavior != null)
        {
            Owner.attackBehavior.PlayAttackAnimation(Owner, Owner.nearestEnemy);
        }
    }

    public override void OnUpdate()
    {
        if (Core.NetMgr.serverTimer == _realAttackFrame)
        {
            // 执行攻击（使用攻击行为）
            if (Owner.nearestEnemy != null && Owner.attackBehavior != null)
            {
                Owner.attackBehavior.ExecuteAttack(Owner, Owner.nearestEnemy);
            }
        }

        if (Core.NetMgr.serverTimer == _exitAttackFrame)
        {
            Owner.fsm.ChangeState(CharacterState.Idle);
        }
    }
}