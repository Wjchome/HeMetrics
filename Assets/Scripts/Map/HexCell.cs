using System;
using UnityEngine;


public class HexCell : BaseCell
{
    // 立方体坐标 (q, r, s)，满足约束：q + r + s = 0
    public int q, r, s;

    #region A*算法所需参数

    // A*算法所需参数
    public int gCost; // 起点到当前节点的实际代价
    public int hCost; // 当前节点到终点的预估代价（启发式）
    public HexCell parent; // 寻路路径中的父节点

    // 计算fCost（总代价 = g + h）
    public int fCost => gCost + hCost;

    #endregion
    public void Init(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.green;
        for (int i = 0; i < 6; i++)
        {
            var q = Core.HexMapMgr.GetCell(this, (HexDirection)i);
            if (q != null)
                q.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 122);
        }
    }

    public void OnMouseExit()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
        for (int i = 0; i < 6; i++)
        {
            var q = Core.HexMapMgr.GetCell(this, (HexDirection)i);
            if (q != null)

                q.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void OnMouseDown()
    {
        Core.HexMapMgr.Click(this);
    }
}