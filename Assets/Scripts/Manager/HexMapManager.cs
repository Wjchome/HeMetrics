using System.Collections.Generic;
using UnityEngine;


public class HexMapManager : MonoBehaviour
{
    public HexCell hexCellPrefab;

    public Character characterPrefab;

    // 使用字典存储，key 是立方体坐标的元组 (q, r, s)
    private Dictionary<(int, int, int), HexCell> hexCells;
    public int width, height;
    public float cellSize;

    public void Init()
    {
        hexCells = new Dictionary<(int, int, int), HexCell>();
        // 初始化时使用偏移坐标创建网格，然后转换为立方体坐标
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 将偏移坐标转换为立方体坐标
                int q = OffsetToCubeQ(x, y);
                int r = y;
                int s = -q - r;

                HexCell hexCell = Instantiate(hexCellPrefab, CubeToWorldPosition(q, r, s), Quaternion.identity,
                    transform);
                hexCell.Init(q, r, s);
                hexCells[(q, r, s)] = hexCell;
            }
        }
    }

    /// <summary>
    /// 将立方体坐标转换为世界坐标
    /// </summary>
    Vector2 CubeToWorldPosition(int q, int r, int s)
    {
        // 立方体坐标转世界坐标的公式
        float x = cellSize * (Mathf.Sqrt(3) * q + Mathf.Sqrt(3) / 2 * r);
        float y = cellSize * (3f / 2 * r);
        return new Vector2(x, y);
    }

    // /// <summary>
    // /// 检查立方体坐标是否在有效范围内（通过转换为偏移坐标检查）
    // /// </summary>
    // public bool IsValidCubePos(int q, int r, int s)
    // {
    //     // 验证立方体坐标约束
    //     if (q + r + s != 0) return false;
    //     
    //     // 转换为偏移坐标进行范围检查
    //     int x = CubeToOffsetX(q, r); //        战斗系统  -- 「角色 羁绊 装备 海克斯 网格」
    //     int y = r;                             网络系统
    //
    //     return x >= 0 && x < width && y >= 0 && y < height;
    // }

    /// <summary>
    /// 获取指定方向的邻居单元格（使用立方体坐标）
    /// </summary>
    public HexCell GetCell(HexCell cell, HexDirection direction)
    {
        // 立方体坐标的6个方向向量
        int dq = 0, dr = 0, ds = 0;
        switch (direction)
        {
            case HexDirection.Right:
                dq = 1;
                dr = 0;
                ds = -1;
                break;
            case HexDirection.RightDown:
                dq = 1;
                dr = -1;
                ds = 0;
                break;
            case HexDirection.LeftDown:
                dq = 0;
                dr = -1;
                ds = 1;
                break;
            case HexDirection.Left:
                dq = -1;
                dr = 0;
                ds = 1;
                break;
            case HexDirection.LeftUp:
                dq = -1;
                dr = 1;
                ds = 0;
                break;
            case HexDirection.RightUp:
                dq = 0;
                dr = 1;
                ds = -1;
                break;
        }

        int newQ = cell.q + dq;
        int newR = cell.r + dr;
        int newS = cell.s + ds;

        var key = (newQ, newR, newS);
        return hexCells.GetValueOrDefault(key);
    }

    public List<HexCell> GetAstarPath(HexCell start, HexCell target, List<Character> ignoreCharacters)
    {
        List<HexCell> openSet = new List<HexCell>(); // 待检查的节点
        HashSet<HexCell> closedSet = new HashSet<HexCell>(); // 已检查的节点
        openSet.Add(start);
        start.gCost = 0;
        start.hCost = GetHexDistance(start, target); // 计算起点到终点的预估代价
        start.parent = null;
        while (openSet.Count > 0)
        {
            // 1. 从openSet中找到fCost最小的节点（当前节点）
            HexCell current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost ||
                    (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                {
                    current = openSet[i];
                }
            }

            // 2. 将当前节点从openSet移到closedSet
            openSet.Remove(current);
            closedSet.Add(current);

            // 3. 如果到达终点，回溯路径并返回
            if (current == target)
            {
                return RetracePath(start, target);
            }

            // 4. 遍历当前节点的所有邻居
            for (int i = 0; i < 6; i++)
            {
                HexCell neighbor = GetCell(current, (HexDirection)i);
                if (neighbor == null)
                {
                    continue;
                }

                // 邻居已检查或不可通行（如果有障碍物逻辑，这里添加判断）
                if (closedSet.Contains(neighbor) ||
                    (neighbor.characterOn != null && !ignoreCharacters.Contains(neighbor.characterOn)))
                    continue;

                // 计算从起点到邻居的临时gCost（当前g + 1，假设相邻单元格代价为1）
                int newGCost = current.gCost + 1; // 可根据地形设置不同代价（如山地代价更高）

                // 如果是新节点，或找到更优路径（gCost更小）
                if (newGCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // 更新邻居的代价和父节点
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetHexDistance(neighbor, target);
                    neighbor.parent = current;

                    // 如果邻居不在openSet中，添加进去
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // 如果openSet为空仍未找到终点，说明无路径
        return null;
    }

    /// <summary>
    /// 计算两个六边形单元格之间的距离（使用立方体坐标）
    /// </summary>
    public int GetHexDistance(HexCell a, HexCell b)
    {
        // 直接使用立方体坐标计算距离，非常简单
        return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r) + Mathf.Abs(a.s - b.s)) / 2;
    }

    /// <summary>
    /// 将偏移坐标转换为立方体坐标的q值（奇数行偏移）
    /// 用于初始化时创建网格
    /// </summary>
    private int OffsetToCubeQ(int x, int y)
    {
        // 统一公式，使用 FloorToInt 确保负数也正确向下取整
        int offset = y - (y & 1);
        return x - Mathf.FloorToInt(offset / 2f);
    }

    /// <summary>
    /// 将立方体坐标转换为偏移坐标的x值（奇数行偏移）
    /// 用于范围检查
    /// </summary>
    private int CubeToOffsetX(int q, int r)
    {
        return q + Mathf.FloorToInt((r - (r & 1)) / 2f);
    }

    /// <summary>
    /// 回溯路径
    /// </summary>
    private List<HexCell> RetracePath(HexCell start, HexCell target)
    {
        List<HexCell> path = new List<HexCell>();
        HexCell current = target;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Add(start);
        path.Reverse();

        return path;
    }
}

public enum HexDirection
{
    Right,
    RightDown,
    LeftDown,
    Left,
    LeftUp,
    RightUp,
}