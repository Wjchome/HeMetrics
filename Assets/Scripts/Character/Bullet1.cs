using System;
using DG.Tweening;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{


    public int damage;
    public float hitTime;

    private int hitFrame;
    private int hitTimer;

    private Character target;

    public bool isDead = false;
    
    public AnimationCurve curve;
    
    private HexCell targetCell;

    public void Init(int damage, float hitTime, Character target, HexCell targetCell)
    {
        this.damage = damage;
        this.hitTime = hitTime;
        this.target = target;
        this.targetCell = targetCell;
        hitFrame = (int)(hitTime * Const.ServerFrame);
        hitTimer = 0;
    }

    public void UpdateFrame()
    {
        if (target != null)
        {
            hitTimer++;
            float rate = (float)hitTimer / hitFrame;
            rate = curve.Evaluate(rate);
            transform.position = Vector3.Lerp(transform.position, targetCell.transform.position, rate);
            if (rate >= 1)
            {
                ShowDead();
            }
        }
        else
        {
            ShowDead();
        }
    }

    void ShowDead()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        transform.DOScale(0.01f, 0.3f).OnComplete(() => { Destroy(gameObject); }).SetLink(gameObject,LinkBehaviour.KillOnDestroy);
    }

    public void LogicDead()
    {
        if (targetCell.characterOn == target)
        {
            target.Defend(damage);
        }
    }
}