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
    public RectTransform midHP;
    public RectTransform realHP;
    
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
        HPUIShow();
        
    }


    public void UpdateFrame()
    {
        if (currentCell is HexCell)
        {
            UpdateTarget();
            if (currentState == CharacterState.Walk)
            {
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
                    else
                    {
                        currentState = CharacterState.Attack;
                        lastAttackFrame = Core.NetMgr.serverTimer;
                    }
                }
            }
            else if (currentState == CharacterState.Attack)
            {
                if (lastAttackFrame + attackIntervalFrame < Core.NetMgr.serverTimer)
                {
                    lastAttackFrame = Core.NetMgr.serverTimer;

                    if (nearestEnemy != null &&
                        Core.HexMapMgr.GetHexDistance(nearestEnemy.currentCell as HexCell, currentCell as HexCell) == 1)
                    {
                        Attack();
                        Vector2 position = currentCell.transform.position;
                        transform.DOMove(Vector2.Lerp(position, nearestEnemy.currentCell.transform.position, 0.5f),
                                attackInterval / 3)
                            .OnComplete(() => { transform.DOMove(position, attackInterval / 3); });
                    }

                    else
                    {
                        currentState = CharacterState.Walk;
                    }
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

        HPUIShow();
    }


    public void HPUIShow()
    {
        float rate = (float)HP / MaxHP;
        realHP.sizeDelta = new Vector2(rate, realHP.sizeDelta.y);
        midHP.DOSizeDelta(new Vector2(rate, realHP.sizeDelta.y), 0.5f);
    }
}