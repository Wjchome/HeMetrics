using UnityEngine;

public class Object2DClickHandler : MonoBehaviour
{
    // 可选：指定检测的层级（过滤不需要的物体）
    public LayerMask targetLayers;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 将鼠标位置转换为世界坐标（2D中常用Camera.main.ScreenToWorldPoint）
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 2D射线的方向可以忽略（用点检测更合适，因为2D射线本质是线，点检测更直观）
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, targetLayers);

            if (hit)
            {
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log("点击了2D物体：" + clickedObject.name);
                // 调用物体的点击逻辑
               // clickedObject.GetComponent<Your2DScript>()?.OnClick();
            }
        }
    }
}