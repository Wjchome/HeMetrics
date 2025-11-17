/// <summary>
/// 攻击行为工厂 - 根据角色类型创建对应的攻击行为
/// </summary>
public static class AttackBehaviorFactory
{
    public static IAttackBehavior CreateAttackBehavior(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.Melee:
                return new MeleeAttackBehavior();
            case CharacterType.Ranged:
                return new RangedAttackBehavior();
            case CharacterType.Shoot:
                return new ShootAttackBehavior();
            default:
                return new MeleeAttackBehavior(); // 默认近战
        }
    }
}

