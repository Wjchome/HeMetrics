using DG.Tweening;
using UnityEngine;

public class MeleeCharacter : Character
{
    protected override void UpdateState()
    {
        if (nearestEnemy == null)
            return;

        int dis = Core.HexMapMgr.GetHexDistance(currentCell as HexCell, nearestEnemy.currentCell as HexCell);
        lastState = currentState;
        if (dis <= attackRange)
        {
            currentState = CharacterState.Attack;
        }
        else
        {
            currentState = CharacterState.Move;
        }


        if (currentState == CharacterState.Move)
        {
            if (lastState == CharacterState.Attack)
            {
                lastMoveFrame = Core.NetMgr.serverTimer;
            }
            if (movePath != null)
            {
                if (movePath.Count > 2)
                {
                    if (lastMoveFrame + moveIntervalFrame < Core.NetMgr.serverTimer &&
                        movePath[1].characterOn == null)
                    {
                        lastMoveFrame = Core.NetMgr.serverTimer;
                        //
                        transform.DOMove(movePath[1].transform.position, moveInterval).SetLink(gameObject,LinkBehaviour.KillOnDestroy);
                        currentCell.characterOn = null;
                        currentCell = movePath[1];
                        currentCell.characterOn = this;
                    }
                }
            }
        }
        else if (currentState == CharacterState.Attack)
        {
            if (lastState == CharacterState.Move)
            {
                lastAttackFrame = Core.NetMgr.serverTimer;
            }
            if (lastAttackFrame + attackIntervalFrame < Core.NetMgr.serverTimer)
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
}