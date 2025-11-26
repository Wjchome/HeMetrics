/// <summary>
/// 羁绊数值配置 - 根据羁绊类型和等级返回实际的属性加成值
/// </summary>
public static class BondValueConfig
{
    /// <summary>
    /// 获取攻击羁绊的加成值
    /// </summary>
    public static int GetAttackBonus(int bondLevel)
    {
        // 等级1: +10攻击, 等级2: +25攻击, 等级3: +50攻击
        int[] bonuses = { 0, 10, 25, 50 };
        if (bondLevel > 0 && bondLevel < bonuses.Length)
        {
            return bonuses[bondLevel];
        }
        // 超出范围使用线性计算
        return bondLevel * 15;
    }

    /// <summary>
    /// 获取防御羁绊的加成值
    /// </summary>
    public static int GetDefenceBonus(int bondLevel)
    {
        // 等级1: +5防御, 等级2: +12防御, 等级3: +25防御
        int[] bonuses = { 0, 5, 12, 25 };
        if (bondLevel > 0 && bondLevel < bonuses.Length)
        {
            return bonuses[bondLevel];
        }
        return bondLevel * 8;
    }

    /// <summary>
    /// 获取生命值羁绊的加成值
    /// </summary>
    public static int GetHPBonus(int bondLevel)
    {
        // 等级1: +50生命, 等级2: +120生命, 等级3: +250生命
        int[] bonuses = { 0, 50, 120, 250 };
        if (bondLevel > 0 && bondLevel < bonuses.Length)
        {
            return bonuses[bondLevel];
        }
        return bondLevel * 80;
    }

    /// <summary>
    /// 获取移动速度羁绊的加成值（减少移动间隔，返回负值）
    /// </summary>
    public static int GetMoveSpeedBonus(int bondLevel)
    {
        // 等级1: -0.1秒, 等级2: -0.25秒, 等级3: -0.5秒
        // 注意：这里返回的是毫秒值（乘以1000），因为AttributeManager使用int
        int[] bonuses = { 0, -100, -250, -500 }; // 单位：毫秒
        if (bondLevel > 0 && bondLevel < bonuses.Length)
        {
            return bonuses[bondLevel];
        }
        return -(bondLevel * 150);
    }

    /// <summary>
    /// 根据羁绊类型获取对应的属性加成值
    /// </summary>
    public static int GetBondBonus(BondType bondType, int bondLevel, string attributeName)
    {
        switch (bondType)
        {
            case BondType.Attack:
                if (attributeName == "Attack")
                {
                    return GetAttackBonus(bondLevel);
                }
                break;
            case BondType.Defend:
                if (attributeName == "Defence")
                {
                    return GetDefenceBonus(bondLevel);
                }
                break;
            case BondType.HP:
                if (attributeName == "MaxHP")
                {
                    return GetHPBonus(bondLevel);
                }
                break;
            case BondType.Move:
                if (attributeName == "MoveInterval")
                {
                    return GetMoveSpeedBonus(bondLevel);
                }
                break;
        }
        return 0;
    }
}

