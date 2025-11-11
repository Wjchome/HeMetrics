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
                        currentState = CharacterState.Attack;
                        return;
                    }

                    if (lastMoveFrame + moveIntervalFrame < Core.NetMgr.serverTimer)
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
            }
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

            movePath = Core.HexMapMgr.GetAstarPath(currentCell as HexCell, nearestCharacter.currentCell as HexCell);
        }
    }
    
    // 记录物体与鼠标的偏移量（避免点击时物体“跳位”）
    private Vector3 dragOffset;
    // 标记物体是否正在被拖动
    private bool isDragging = false;

    // 鼠标按下时触发（检测到碰撞体时）
    void OnMouseDown()
    {
        if (Core.GameMgr.gameState != GameState.Display)
        {
            return;
        }

        dragOffset = (Vector2)transform.position - Core.CursorMgr.mousePosition;
        // 标记开始拖动
        isDragging = true;
    }

    // 鼠标拖动时触发（持续调用）
    void OnMouseDrag()
    {
        if (Core.GameMgr.gameState != GameState.Display)
        {
            return;
        }
        
        if (!isDragging) return; // 未按下时不执行
        
        transform.position = Core.CursorMgr.mousePosition + (Vector2)dragOffset;
    }

    // 鼠标抬起时触发
    void OnMouseUp()
    {
        isDragging = false; // 结束拖动
        
    }
    
    
}