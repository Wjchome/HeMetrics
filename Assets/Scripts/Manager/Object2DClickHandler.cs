using DG.Tweening;
using UnityEngine;

/// <summary>
/// 2D点击和拖动处理器：使用射线检测替代Unity的鼠标事件，更准确可靠
/// </summary>
public class Object2DClickHandler
{
    private RaycastHit2D[] hitResults = new RaycastHit2D[10]; // 增加容量以支持更多碰撞
    
    // 拖动相关
    private Character draggingCharacter;
    private Vector3 dragOffset;
    private Vector3 originalPos;
    private bool isDragging = false;

    /// <summary>
    /// 获取鼠标位置下的所有碰撞体
    /// </summary>
    public (int, RaycastHit2D[]) GetHit()
    {
        int hitCount = Physics2D.RaycastNonAlloc(
            Core.CursorMgr.mousePosition, 
            Vector2.zero, 
            hitResults, 
            Mathf.Infinity
        );
        return (hitCount, hitResults);
    }

    /// <summary>
    /// 更新拖动逻辑（每帧调用）
    /// </summary>
    public void Update()
    {
        // 只在 Display 状态下处理拖动
        if (Core.GameMgr.gameState != GameState.Display)
        {
            if (isDragging)
            {
                // 如果状态改变，取消拖动
                CancelDrag();
            }
            return;
        }

        // 处理鼠标按下
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }

        // 处理鼠标拖动
        if (isDragging && Input.GetMouseButton(0))
        {
            HandleMouseDrag();
        }

        // 处理鼠标抬起
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            HandleMouseUp();
        }
    }

    /// <summary>
    /// 处理鼠标按下：检测是否点击到 Character
    /// </summary>
    private void HandleMouseDown()
    {
        var (hitCount, hits) = GetHit();
        
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i])
            {
                var character = hits[i].collider.GetComponent<Character>();
                if (character != null)
                {
                    // 找到 Character，开始拖动
                    StartDrag(character);
                    Debug.Log("Clicked Character: " + character.name);
                    break; // 只拖动第一个找到的 Character
                }
            }
        }
    }

    /// <summary>
    /// 处理鼠标拖动：更新 Character 位置
    /// </summary>
    private void HandleMouseDrag()
    {
        if (draggingCharacter == null) return;

        Vector2 targetPos = Core.CursorMgr.mousePosition + (Vector2)dragOffset;
        draggingCharacter.transform.position = Vector2.Lerp(
            draggingCharacter.transform.position, 
            targetPos, 
            Time.deltaTime * 10
        );
    }

    /// <summary>
    /// 处理鼠标抬起：尝试放置 Character 到目标位置
    /// </summary>
    private void HandleMouseUp()
    {
        if (draggingCharacter == null) return;

        var (hitCount, hits) = GetHit();
        bool isOk = false;

        // 查找可以放置的 Map 单元格
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i])
            {
                var cell = hits[i].collider.gameObject;
                if (cell.CompareTag("Map"))
                {
                    var baseCell = cell.GetComponent<BaseCell>();
                    if (baseCell != null && baseCell.characterOn == null)
                    {
                        // 找到有效的放置位置
                        isOk = true;
                        PlaceCharacter(draggingCharacter, baseCell);
                        break;
                    }
                }
            }
        }

        // 如果放置失败，返回原位置
        if (!isOk)
        {
            draggingCharacter.transform.DOMove(originalPos, 0.3f);
        }

        // 结束拖动
        EndDrag();
    }

    /// <summary>
    /// 开始拖动 Character
    /// </summary>
    private void StartDrag(Character character)
    {
        draggingCharacter = character;
        originalPos = character.transform.position;
        dragOffset = (Vector2)character.transform.position - Core.CursorMgr.mousePosition;
        isDragging = true;
    }

    /// <summary>
    /// 放置 Character 到目标单元格
    /// </summary>
    private void PlaceCharacter(Character character, BaseCell targetCell)
    {
        // 在清除原位置之前保存状态
        bool wasHex = character.currentCell != null && character.currentCell is HexCell;
        
        // 清除原位置
        if (character.currentCell != null)
        {
            character.currentCell.characterOn = null;
        }
        
        // 设置新位置
        character.currentCell = targetCell;
        targetCell.characterOn = character;
        character.transform.position = targetCell.transform.position;

        // 检查是否需要切换 Character 列表
        bool isHex = targetCell is HexCell;
        if (wasHex != isHex)
        {
            Core.CharacterMgr.ChangeCharacter(character, isHex);
            Core.bondMgr.ChangeCharacter(character, isHex);
        }
    }

    /// <summary>
    /// 结束拖动
    /// </summary>
    private void EndDrag()
    {
        draggingCharacter = null;
        isDragging = false;
    }

    /// <summary>
    /// 取消拖动（状态改变时调用）
    /// </summary>
    private void CancelDrag()
    {
        if (draggingCharacter != null && originalPos != Vector3.zero)
        {
            draggingCharacter.transform.DOMove(originalPos, 0.3f);
        }
        EndDrag();
    }

    /// <summary>
    /// 检查是否正在拖动
    /// </summary>
    public bool IsDragging()
    {
        return isDragging;
    }
}