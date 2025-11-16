using System;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Unity.VisualScripting;
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

    public int id;
    public CharacterData data;
    
    public int MaxHP;
    public int attack;
    public int defence;
    public float moveInterval;
    public float attackInterval;
    public float attackWindup; //攻击前摇
    public int attackRange;
    
    public List<BondType> bondTypes;
    
    public int HP;

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

    public void Init(int id)
    {
        this.id = id;
        data = Core.dataMgr.CharacterData()[id];
        MaxHP = data.MaxHP;
        attack = data.Attack;
        defence = data.Defend;
        moveInterval = data.MoveInterval;
        attackInterval = data.AttackInterval;
        attackWindup = data.AttackWindup; //攻击前摇
        attackRange = data.AttackRange;
        bondTypes = data.BondList;
        
        HP = MaxHP;

        moveIntervalFrame = (int)(moveInterval * Const.ServerFrame);
        lastMoveFrame = 0;

        attackIntervalFrame = (int)(attackInterval * Const.ServerFrame);
        lastAttackFrame = 0;

        attackWindupFrame = (int)(attackWindup * Const.ServerFrame);
        realAttackFrame = -1;
        
        
        HPUIShow();
    }

    public void Init(CharacterData data)
    {
        this.data = data;
        Init(data.Id);
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

    protected virtual void UpdateState()
    {
    }


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
   // AttributeManager attributeManager;
}


// public class AttributeRecorde
// { 
//     // 属性来源
//     public string sorce;
//

//     public double value;
// }

// public class AttributeManager
// {
//     public Dictionary<string, List<AttributeRecorde>> attributeDic;
//
//     public void Add(string name, string sorce, double value)
//     {
//         
//     }
//
//     public double GetFinalValue(string name)
//     {
//         var attributeRecords = attributeDic[name];
//         double finalValue = 0;
//         for (int i = 0; i < attributeRecords.Count; i++)
//         {
//             switch (attributeRecords[i].sorce)
//             {
//                 
//             }
//             var record = attributeRecords[i];
//             finalValue += record.value;
//         }
//         return finalValue;
//     }
//
//     public void Remove(string name, string sorce)
//     {
//         
//     }
//     
//     // "负面效果"
//     public void RemoveByPrefix()
// }
