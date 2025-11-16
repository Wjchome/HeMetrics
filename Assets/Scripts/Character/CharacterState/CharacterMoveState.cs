using DG.Tweening;

public class CharacterMoveState : FSMState<Character>
{
    private long _exitAttackFrame; 
    
    public override void OnEnter()
    {
        _exitAttackFrame = Core.NetMgr.serverTimer + Owner.moveIntervalFrame;
        
        Owner.lastMoveFrame = Core.NetMgr.serverTimer;
        Owner.transform.DOMove(Owner.movePath[1].transform.position, Owner.moveInterval)
            .SetLink(Owner.gameObject, LinkBehaviour.KillOnDestroy);
        Owner.currentCell.characterOn = null;
        Owner.currentCell = Owner.movePath[1];
        Owner.currentCell.characterOn = Owner;
    }
    public override void OnUpdate()
    {
        if (Core.NetMgr.serverTimer == _exitAttackFrame)
        {
            Owner.fsm.ChangeState(CharacterState.Idle);
        }
    }

}