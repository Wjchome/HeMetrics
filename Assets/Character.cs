using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum CharacterState
{
    Walk,
    Attack
}

public class Character : MonoBehaviour
{
    public BaseCell currentCell;
    public bool isMine;

    public CharacterState currentState;
    public int MaxHP;
    public int HP;
    public int attack;
    public int defence;
    public float moveInterval;
    public float attackInterval;

    private int moveIntervalFrame;
    private long lastMoveFrame;

    private int attackIntervalFrame;
    private long lastAttackFrame;

    private long lastChecktargetFrame = 0;

    private List<HexCell> movePath = new List<HexCell>();
    private Character nearestEnemy;

    private void Start()
    {
        moveIntervalFrame = (int)(moveInterval * Const.ServerFrame);
        lastMoveFrame = 0;

        attackIntervalFrame = (int)(attackInterval * Const.ServerFrame);
        lastAttackFrame = 0;
    }


    public void UpdateFrame()
    {
        if (currentCell is HexCell)
        {
            UpdateTarget();
            if (currentState == CharacterState.Walk)
            {
                if (movePath != null && movePath.Count > 2)
                {
                    if (currentCell == movePath[movePath.Count - 2])
                    {
                        //测试
                        currentState = CharacterState.Attack;
                        return;  
                    }

                    if (lastMoveFrame + moveIntervalFrame < Core.NetMgr.serverTimer && movePath[1].characterOn == null)
                    {
                        lastMoveFrame = Core.NetMgr.serverTimer;

                        transform.DOMove(movePath[1].transform.position, moveInterval);
                        currentCell.characterOn = null;
                        currentCell = movePath[1];
                        currentCell.characterOn = this;
                    }
                }
            }
            else if (currentState == CharacterState.Attack)
            {
                if (movePath != null && movePath.Count > 2)
                {
                    if (nearestEnemy != null && movePath[movePath.Count - 1].characterOn == nearestEnemy)
                    {
                        if (lastAttackFrame + attackIntervalFrame < Core.NetMgr.serverTimer)
                        {
                            Attack();
                            Vector2 position = transform.position;
                            transform.DOMove(movePath[1].transform.position, attackInterval / 3)
                                .OnComplete(() =>
                                {
                                    transform.DOMove(position, attackInterval / 3);
                                });
                        }
                    }
                    else
                    {
                        currentState = CharacterState.Walk;
                    }
                }
                else
                {
                    currentState = CharacterState.Walk;
                }
            }
        }
    }

    void UpdateTarget()
    {
        if (lastChecktargetFrame + Const.findTargetFrame < Core.NetMgr.serverTimer)
        {
            lastChecktargetFrame = Core.NetMgr.serverTimer;

            nearestEnemy = Core.CharacterMgr.GetNearestCharacter(this);
            if (nearestEnemy == null)
            {
                movePath = null;
                return;
            }

            movePath = Core.HexMapMgr.GetAstarPath(currentCell as HexCell, nearestEnemy.currentCell as HexCell,
                new List<Character> { this, nearestEnemy });
        }
    }

    public void Attack()
    {
        nearestEnemy.Defend(attack);
    }

    public void Defend(int damage)
    {
        damage = Mathf.Clamp(damage - defence, 0, damage);
        HP = Mathf.Clamp(HP - damage, 0, HP);
    }
}