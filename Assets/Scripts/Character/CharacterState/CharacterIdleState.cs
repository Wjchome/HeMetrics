

/// <summary>
/// 有目标
///     在攻击范围内 -> attack
///     不在        -> move
/// </summary>
public class CharacterIdleState : FSMState<Character>
{
    public override void OnUpdate()
    {
        Owner.UpdateTarget();
        if (Owner.nearestEnemy != null)
        {
            int dis = Core.HexMapMgr.GetHexDistance(Owner.currentCell as HexCell, Owner.nearestEnemy.currentCell as HexCell);
            if (dis <= Owner.attackRange)
            {
                Owner.fsm.ChangeState(CharacterState.Attack);
            }
            else
            {
                Owner.fsm.ChangeState(CharacterState.Move);
            }
        }
    }

    public override void OnEnter()
    {
        
    }
}