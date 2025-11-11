using UnityEngine;

public class Object2DClickHandler
{
    private RaycastHit2D[] hitResults = new RaycastHit2D[2];

    public (int, RaycastHit2D[]) GetHit()
    {
        // 2D射线的方向可以忽略（用点检测更合适，因为2D射线本质是线，点检测更直观）
        int hitCount =
            Physics2D.RaycastNonAlloc(Core.CursorMgr.mousePosition, Vector2.zero, hitResults, Mathf.Infinity);

        // 只遍历实际碰撞的数量（避免访问无效元素）
        // for (int i = 0; i < hitCount; i++)
        // {
        //     if (hitResults[i]) // 确保碰撞有效
        //     {
        //         Debug.Log("碰撞到：" + hitResults[i].collider.name);
        //     }
        // }
        return (hitCount,hitResults);
    }
}