/// <summary>
/// Buff使用类型枚举 - 定义UseType对应的属性类型和计算方式
/// </summary>
public enum BuffUseType
{
    // 基础属性（绝对值）
    Attack = 1,           // 攻击力（绝对值）
    Defence = 2,          // 防御力（绝对值）
    MaxHP = 3,            // 最大生命值（绝对值）
    
    // 百分比属性（Param1表示百分比，例如30表示30%）
    AttackPercent = 4,    // 攻击力百分比
    DefencePercent = 5,   // 防御力百分比
    MaxHPPercent = 6,     // 最大生命值百分比
    
    // 时间间隔（Param1表示减少的毫秒数，负数表示减少）
    AttackInterval = 7,   // 攻击间隔（减少时间，单位：毫秒）
    MoveInterval = 8,     // 移动间隔（减少时间，单位：毫秒）
    
    // 百分比时间间隔（Param1表示百分比，例如30表示减少30%）
    AttackIntervalPercent = 9,  // 攻击间隔百分比（减少）
    MoveIntervalPercent = 10,   // 移动间隔百分比（减少）
}

/// <summary>
/// Buff使用类型配置 - 根据UseType获取属性信息和计算方式
/// </summary>
public static class BuffUseTypeConfig
{
    /// <summary>
    /// 获取属性名称
    /// </summary>
    public static string GetAttributeName(BuffType useType)
    {
        return "";
        // switch (useType)
        // {
        //     case (int)BuffUseType.Attack:
        //     case (int)BuffUseType.AttackPercent:
        //         return "Attack";
        //     case (int)BuffUseType.Defence:
        //     case (int)BuffUseType.DefencePercent:
        //         return "Defence";
        //     case (int)BuffUseType.MaxHP:
        //     case (int)BuffUseType.MaxHPPercent:
        //         return "MaxHP";
        //     case (int)BuffUseType.AttackInterval:
        //     case (int)BuffUseType.AttackIntervalPercent:
        //         return "AttackInterval";
        //     case (int)BuffUseType.MoveInterval:
        //     case (int)BuffUseType.MoveIntervalPercent:
        //         return "MoveInterval";
        //     default:
        //         return "";
        // }
    }
    
    /// <summary>
    /// 判断是否是百分比类型
    /// </summary>
    public static bool IsPercentType(int useType)
    {
        return useType == (int)BuffUseType.AttackPercent ||
               useType == (int)BuffUseType.DefencePercent ||
               useType == (int)BuffUseType.MaxHPPercent ||
               useType == (int)BuffUseType.AttackIntervalPercent ||
               useType == (int)BuffUseType.MoveIntervalPercent;
    }
    
    /// <summary>
    /// 计算Buff的实际加成值
    /// </summary>
    /// <param name="useType">使用类型</param>
    /// <param name="param1">参数1（值或百分比）</param>
    /// <param name="baseValue">基础值（用于百分比计算）</param>
    /// <returns>实际加成值</returns>
    public static int CalculateBuffValue(int useType, int param1, int baseValue)
    {
        if (IsPercentType(useType))
        {
            // 百分比类型：param1是百分比值（例如30表示30%）
            // 计算：baseValue * param1 / 100
            return (baseValue * param1) / 100;
        }
        else
        {
            // 绝对值类型：param1直接是加成值
            return param1;
        }
    }
    
    /// <summary>
    /// 获取Buff显示名称
    /// </summary>
    public static string GetDisplayName(int useType)
    {
        switch (useType)
        {
            case (int)BuffUseType.Attack:
                return "攻击力";
            case (int)BuffUseType.Defence:
                return "防御力";
            case (int)BuffUseType.MaxHP:
                return "最大生命值";
            case (int)BuffUseType.AttackPercent:
                return "攻击力百分比";
            case (int)BuffUseType.DefencePercent:
                return "防御力百分比";
            case (int)BuffUseType.MaxHPPercent:
                return "最大生命值百分比";
            case (int)BuffUseType.AttackInterval:
                return "攻击间隔";
            case (int)BuffUseType.MoveInterval:
                return "移动间隔";
            case (int)BuffUseType.AttackIntervalPercent:
                return "攻击间隔百分比";
            case (int)BuffUseType.MoveIntervalPercent:
                return "移动间隔百分比";
            default:
                return $"未知类型({useType})";
        }
    }
}
