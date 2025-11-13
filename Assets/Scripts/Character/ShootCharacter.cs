using DG.Tweening;
using UnityEngine;

public class ShootCharacter : Character
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
            currentState = CharacterState.Walk;
        }


        if (currentState == CharacterState.Walk)
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

                        transform.DOMove(movePath[1].transform.position, moveInterval);
                        currentCell.characterOn = null;
                        currentCell = movePath[1];
                        currentCell.characterOn = this;
                    }
                }
            }
        }
        else if (currentState == CharacterState.Attack)
        {
            if (lastState == CharacterState.Walk)
            {
                lastAttackFrame = Core.NetMgr.serverTimer;
            }

            if (lastAttackFrame + attackIntervalFrame < Core.NetMgr.serverTimer)
            {
                lastAttackFrame = Core.NetMgr.serverTimer;
                realAttackFrame = Core.NetMgr.serverTimer + attackWindupFrame;

                Vector3 position = currentCell.transform.position;
                transform.DOMove(position + 0.2f * (position - nearestEnemy.currentCell.transform.position).normalized,
                        attackInterval / 3)
                    .OnComplete(() => { transform.DOMove(position, attackInterval / 5); }).SetLink(gameObject,LinkBehaviour.KillOnDestroy);
            }

            if (Core.NetMgr.serverTimer == realAttackFrame)
            {
                var bullet = Instantiate(Core.I.bullet1Prefab, transform.position, Quaternion.identity);
                Core.Bullet1Mgr.AddBullet1(bullet);
                bullet.Init(attack, 0.4f * dis, nearestEnemy,nearestEnemy.currentCell as HexCell);
            }
        }
    }
}