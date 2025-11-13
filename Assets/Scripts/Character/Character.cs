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

    public CharacterState lastState;
    public CharacterState currentState;
    
    public int MaxHP;
    public int HP;
    public int attack;
    public int defence;
    public float moveInterval;
    public float attackInterval;
    public float attackWindup;//攻击前摇
    public int attackRange;
    
    protected int moveIntervalFrame;
    protected long lastMoveFrame;

    protected int attackIntervalFrame;
    protected long lastAttackFrame;

    protected int attackWindupFrame;
    protected long realAttackFrame;
    
    protected long lastChecktargetFrame = 0;

    protected List<HexCell> movePath = new List<HexCell>();
    protected Character nearestEnemy;

    public bool isDead = false;

    public void Start()
    {
        
        moveIntervalFrame = (int)(moveInterval * Const.ServerFrame);
        lastMoveFrame = 0;

        attackIntervalFrame = (int)(attackInterval * Const.ServerFrame);
        lastAttackFrame = 0;

        attackWindupFrame = (int)(attackWindup * Const.ServerFrame);
        realAttackFrame = -1;
        HPUIShow();
    }


    public void UpdateFrame()
    {
        UpdateTarget();
        UpdateState();
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

    protected virtual void UpdateState(){}
        

    protected void Attack()
    {
        nearestEnemy.Defend(attack);
    }

    public void Defend(int damage)
    {
        if (isDead)
            return;
        damage = Mathf.Clamp(damage - defence, 0, damage);
        HP = Mathf.Clamp(HP - damage, 0, HP);

        HPUIShow();

        if (HP <= 0)
        {
            ShowDead();
        }
    }


    public void HPUIShow()
    {
        float rate = (float)HP / MaxHP;
        realHP.sizeDelta = new Vector2(rate, realHP.sizeDelta.y);
        midHP.DOSizeDelta(new Vector2(rate, realHP.sizeDelta.y), 1);
    }

    void ShowDead()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        transform.DOScaleY(0.1f, 0.5f).OnComplete(() => { Destroy(gameObject); });
    }

    public void LogicDead()
    {
        currentCell.characterOn = null;
    }
}