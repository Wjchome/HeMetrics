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

    public long lastChecktargetFrame = 0;

    public List<HexCell> movePath = new List<HexCell>();
    public Character nearestEnemy;

    public bool isDead = false;

    public AttributeManager attributeManager;
    public BuffManager buffManager;

    public SpriteRenderer spriteRenderer;
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

        // 初始化AttributeManager和BuffManager
        attributeManager = new AttributeManager();
        buffManager = new BuffManager(this);

        // 根据角色类型创建对应的攻击行为
        attackBehavior = AttackBehaviorFactory.CreateAttackBehavior(data.CharacterType);

        // 初始化FSM
        fsm = new FSMStateMgr<Character, CharacterState>(this);
        // 注册状态（使用工厂创建，方便将来扩展）
        fsm.RegisterState(CharacterState.Idle, new CharacterIdleState());
        fsm.RegisterState(CharacterState.Move, new CharacterMoveState());
        fsm.RegisterState(CharacterState.Attack, CharacterAttackStateFactory.CreateAttackState(data.CharacterType));
        fsm.ChangeState(CharacterState.Idle);

        spriteRenderer.sprite = Resources.Load<Sprite>("Img/" + data.IconUrl);
        
        // 更新属性（应用所有buff）
        UpdateAttributes();

        HPUIShow();
    }


    public void UpdateFrame()
    {
        buffManager.UpdateFrame();
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

    /// <summary>
    /// 更新角色属性（根据AttributeManager计算最终值）
    /// </summary>
    public void UpdateAttributes()
    {
        attack = attributeManager.GetFinalValue(data.Attack, "Attack");
        defence = attributeManager.GetFinalValue(data.Defend, "Defence");
        MaxHP = attributeManager.GetFinalValue(data.MaxHP, "MaxHP");

        // 更新移动间隔（如果有加成）
        if (attributeManager.HasAttribute("MoveInterval"))
        {
            // MoveInterval的单位是毫秒（int），需要转换为秒（float）
            int moveBonusMs = attributeManager.GetFinalValue(0, "MoveInterval");
            moveInterval = data.MoveInterval + (moveBonusMs / 1000f);
            moveIntervalFrame = (int)(moveInterval * Const.ServerFrame);
        }
        else
        {
            moveInterval = data.MoveInterval;
            moveIntervalFrame = (int)(moveInterval * Const.ServerFrame);
        }

        // 更新攻击间隔（如果有加成）
        if (attributeManager.HasAttribute("AttackInterval"))
        {
            int attackBonusMs = attributeManager.GetFinalValue(0, "AttackInterval");
            attackInterval = data.AttackInterval + (attackBonusMs / 1000f);
            attackIntervalFrame = (int)(attackInterval * Const.ServerFrame);
        }
        else
        {
            attackInterval = data.AttackInterval;
            attackIntervalFrame = (int)(attackInterval * Const.ServerFrame);
        }

        // 如果最大生命值变化，按比例调整当前生命值
        if (MaxHP != data.MaxHP)
        {
            float hpRatio = data.MaxHP > 0 ? (float)HP / data.MaxHP : 1f;
            HP = Mathf.RoundToInt(MaxHP * hpRatio);
            HP = Mathf.Clamp(HP, 0, MaxHP);
        }
    }

    /// <summary>
    /// 获取属性的基础值（用于UI显示）
    /// </summary>
    public int GetBaseAttributeValue(string attributeName)
    {
        if (data == null)
        {
            return 0;
        }

        switch (attributeName)
        {
            case "Attack":
                return data.Attack;
            case "Defence":
                return data.Defend;
            case "MaxHP":
                return data.MaxHP;
            case "MoveInterval":
                return (int)(data.MoveInterval * 1000); // 转换为毫秒
            case "AttackInterval":
                return (int)(data.AttackInterval * 1000); // 转换为毫秒
            default:
                return 0;
        }
    }
}