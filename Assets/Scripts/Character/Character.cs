using System;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public enum CharacterState
{
    Idle, //没有寻路到敌人并且没有走动时
    Move, //找到敌人并且
    Attack //
}

public class Character : MonoBehaviour
{
    public RectTransform midHP;
    public RectTransform realHP;

    public BaseCell currentCell;
    public bool isMine;

    public CharacterState lastState;
    public CharacterState currentState;
    public FSMStateMgr<Character, CharacterState> fsm;
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

    // 攻击行为组件（根据角色类型动态创建）
    public IAttackBehavior attackBehavior;

    public int moveIntervalFrame;
    public long lastMoveFrame;

    public int attackIntervalFrame;
    public long lastAttackFrame;

    public int attackWindupFrame;
    public long realAttackFrame;

    public long lastChecktargetFrame = 0;

    public List<HexCell> movePath = new List<HexCell>();
    public Character nearestEnemy;

    public bool isDead = false;

    public AttributeManager attributeManager;

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

        // 根据角色类型创建对应的攻击行为
        attackBehavior = AttackBehaviorFactory.CreateAttackBehavior(data.CharacterType);

        // 初始化FSM
        fsm = new FSMStateMgr<Character, CharacterState>(this);
        // 注册状态（使用工厂创建，方便将来扩展）
        fsm.RegisterState(CharacterState.Idle, new CharacterIdleState());
        fsm.RegisterState(CharacterState.Move, new CharacterMoveState());
        fsm.RegisterState(CharacterState.Attack, CharacterAttackStateFactory.CreateAttackState(data.CharacterType));
        fsm.ChangeState(CharacterState.Idle);

        HPUIShow();
    }

    public void Init(CharacterData data)
    {
        this.data = data;
        Init(data.Id);
    }

    public void UpdateFrame()
    {
        fsm.Update();
    }

    public void UpdateTarget()
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


public class AttributeRecord
{
    public List<(string, int)> attributeRecords = new List<(string, int)>();
    public bool isDirty = true;
}

public class AttributeManager
{
    public Dictionary<string, AttributeRecord> attributeDic;

    public void Add(string name, string sorce, int value)
    {
        if (!attributeDic.TryGetValue(name, out AttributeRecord list))
        {
            list = new AttributeRecord();
            attributeDic.Add(name, list);
        }

        list.attributeRecords.Add((sorce, value));
    }

    public int GetFinalValue(int baseValue, string name)
    {
        var attributeRecords = attributeDic[name];
        int finalValue = baseValue;
        foreach (var (source,value) in attributeRecords.attributeRecords)
        {
            finalValue += value;
        }
        return finalValue;
    }

    public void Remove(string name, string sorce)
    {
        var attributeRecords = attributeDic[name];
        for (int i = 0; i < attributeRecords.attributeRecords.Count; i++)
        {
            var record = attributeRecords.attributeRecords[i];
            if (record.Item1 == sorce)
            {
                attributeRecords.attributeRecords.RemoveAt(i);
                return;
            }
        }
    }
    
}