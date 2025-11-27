using UnityEngine;

/// <summary>
/// Buff实例 - 单个Buff的行为实现
/// 负责：根据BuffData计算属性加成，处理条件判断，管理生命周期
/// </summary>
public class BuffInstance : IBuff
{
    private BuffData buffData;
    private Character character;
    private string buffId;
    private string source;
    private long startTime; // 开始时间（服务器帧数）
    private bool isActive; // 当前是否激活
    
    public string BuffId => buffId;
    public string Source => source;
    
    public BuffInstance(BuffData buffData, Character character, string source)
    {
        this.buffData = buffData;
        this.character = character;
        this.source = source;
        this.startTime = Core.NetMgr.serverTimer;
        this.isActive = false;
        
        // 生成唯一ID：来源_UseType_Param1
        this.buffId = $"{source}_{buffData.UseType}_{buffData.Param1}";
    }
    
    public bool IsActive(Character character)
    {
        if (character == null || character.isDead || IsExpired())
        {
            return false;
        }
        
        // 静态Buff始终激活
        // 条件Buff需要检查条件（这里暂时简化，后续可以扩展）
        // TODO: 如果需要条件Buff，可以在这里添加条件判断逻辑
        
        return true;
    }
    
    public int GetAttributeBonus(Character character, string attributeName)
    {
        if (buffData == null || character == null)
        {
            return 0;
        }
        
        // 获取该Buff影响的属性名称
        string buffAttributeName = BuffUseTypeConfig.GetAttributeName(buffData.UseType);
        if (buffAttributeName != attributeName)
        {
            return 0; // 这个Buff不影响该属性
        }
        
        // 检查是否激活
        if (!IsActive(character))
        {
            return 0;
        }
        
        // 获取基础值（用于百分比计算）
        int baseValue = GetBaseValue(character, (int)buffData.UseType);
        
        // 计算实际加成值
        int buffValue = BuffUseTypeConfig.CalculateBuffValue((int)buffData.UseType, buffData.Param1, baseValue);
        
        return buffValue;
    }
    
    public void Update(Character character)
    {
        if (buffData == null || character == null)
        {
            return;
        }
        
        // 检查状态变化（用于条件Buff）
        bool wasActive = isActive;
        isActive = IsActive(character);
        
        // 如果状态改变，更新AttributeManager
        if (wasActive != isActive)
        {
            if (wasActive)
            {
                RemoveFromAttributeManager(character);
            }
            else
            {
                ApplyToAttributeManager(character);
            }
        }
    }
    
    public bool IsExpired()
    {
        if (buffData == null)
        {
            return true;
        }
        
        // Param2 > 0 表示有持续时间（毫秒）
        if (buffData.Param2 > 0)
        {
            long currentTime = Core.NetMgr.serverTimer;
            long elapsedFrames = currentTime - startTime;
            long durationFrames = (long)(buffData.Param2 / 1000f * Const.ServerFrame);
            
            return elapsedFrames >= durationFrames;
        }
        
        // 永久Buff
        return false;
    }
    
    public void ApplyToAttributeManager(Character character)
    {
        if (character?.attributeManager == null || buffData == null)
        {
            return;
        }
        
        string attributeName = BuffUseTypeConfig.GetAttributeName(buffData.UseType);
        if (string.IsNullOrEmpty(attributeName))
        {
            return;
        }
        
        int bonus = GetAttributeBonus(character, attributeName);
        if (bonus != 0)
        {
            character.attributeManager.Add(attributeName, source, bonus);
        }
    }
    
    public void RemoveFromAttributeManager(Character character)
    {
        if (character?.attributeManager == null || buffData == null)
        {
            return;
        }
        
        string attributeName = BuffUseTypeConfig.GetAttributeName(buffData.UseType);
        if (!string.IsNullOrEmpty(attributeName))
        {
            character.attributeManager.Remove(attributeName, source);
        }
    }
    
    /// <summary>
    /// 获取基础值（用于百分比计算）
    /// </summary>
    private int GetBaseValue(Character character, int useType)
    {
        if (character?.data == null)
        {
            return 0;
        }
        
        switch (useType)
        {
            case (int)BuffUseType.Attack:
            case (int)BuffUseType.AttackPercent:
                return character.data.Attack;
            case (int)BuffUseType.Defence:
            case (int)BuffUseType.DefencePercent:
                return character.data.Defend;
            case (int)BuffUseType.MaxHP:
            case (int)BuffUseType.MaxHPPercent:
                return character.data.MaxHP;
            case (int)BuffUseType.AttackInterval:
            case (int)BuffUseType.AttackIntervalPercent:
                return (int)(character.data.AttackInterval * 1000); // 转换为毫秒
            case (int)BuffUseType.MoveInterval:
            case (int)BuffUseType.MoveIntervalPercent:
                return (int)(character.data.MoveInterval * 1000); // 转换为毫秒
            default:
                return 0;
        }
    }
}
