using UnityEngine;

public class CursorManager:MonoBehaviour
{
    
    public Vector2 mousePosition; // 鼠标在游戏世界中的位置（2D坐标）
    
    public Camera cam;
    public void Update()
    {
        // 获取鼠标的屏幕坐标
        Vector3 screenPos = Input.mousePosition;
        
        screenPos.z = cam.nearClipPlane; // 或者使用固定值，如 10f
        
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        
        // 转换为2D坐标（只取x和y）
        mousePosition = new Vector2(worldPos.x, worldPos.y);
    }
}