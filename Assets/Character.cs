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
    public HexCell currentCell;
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

    private void Start()
    {
        moveIntervalFrame = (int)(moveInterval * Const.ServerFrame);
        lastMoveFrame = 0;

        attackIntervalFrame = (int)(attackInterval * Const.ServerFrame);
        lastAttackFrame = 0;
    }


    public void UpdateFrame()
    {
        UpdateTarget();
        if (currentState == CharacterState.Walk)
        {
            if (movePath != null && movePath.Count > 2)
            {
                if (currentCell == movePath[movePath.Count - 2])
                {
                    currentState = CharacterState.Attack;
                    return;
                }
                if (lastMoveFrame + moveIntervalFrame < Core.NetMgr.serverTimer)
                {
                    lastMoveFrame = Core.NetMgr.serverTimer;
                
                    transform.DOMove(movePath[1].transform.position, moveInterval);
                    currentCell  = movePath[1];
                }
            }

           
        }
        else if (currentState == CharacterState.Attack)
        {
        }
    }

    void UpdateTarget()
    {
        if (lastChecktargetFrame + Const.findTargetFrame > Core.NetMgr.serverTimer)
        {
            lastChecktargetFrame = Core.NetMgr.serverTimer;
            
            Character nearestCharacter = Core.CharacterMgr.GetNearestCharacter(this);
            if (nearestCharacter == null)
            {
                movePath = null;
                return;
            }
            movePath = Core.HexMapMgr.GetAstarPath(currentCell, nearestCharacter.currentCell);
        }
    }
}