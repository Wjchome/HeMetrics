/// <summary>
/// Buff接口 - 定义Buff的行为
/// </summary>
public interface IBuff
{
    /// <summary>
    /// Buff唯一标识（用于识别和移除）
    /// </summary>
    string BuffId { get; }
    
    /// <summary>
    /// Buff来源（用于UI显示和批量移除）
    /// </summary>
    string Source { get; }
    
    /// <summary>
    /// 是否激活（条件Buff需要检查条件）
    /// </summary>
    bool IsActive(Character character);
    
    /// <summary>
    /// 获取属性加成值（根据当前状态动态计算）
    /// </summary>
    /// <param name="character">角色</param>
    /// <param name="attributeName">属性名称</param>
    /// <returns>属性加成值</returns>
    int GetAttributeBonus(Character character, string attributeName);
    
    /// <summary>
    /// 更新Buff（每帧调用，用于处理时间相关的条件）
    /// </summary>
    /// <param name="character">角色</param>
    void Update(Character character);
    
    /// <summary>
    /// 是否已过期（用于临时Buff）
    /// </summary>
    bool IsExpired();
    
    /// <summary>
    /// 应用到AttributeManager
    /// </summary>
    void ApplyToAttributeManager(Character character);
    
    /// <summary>
    /// 从AttributeManager移除
    /// </summary>
    void RemoveFromAttributeManager(Character character);
}
