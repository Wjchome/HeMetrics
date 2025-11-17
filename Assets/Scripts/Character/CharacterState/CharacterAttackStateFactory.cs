/// <summary>
/// 攻击状态工厂 - 根据角色类型创建不同的攻击状态
/// 如果需要不同角色有不同的攻击状态逻辑，可以在这里扩展
/// </summary>
public static class CharacterAttackStateFactory
{
    public static FSMState<Character> CreateAttackState(CharacterType characterType)
    {
        // 目前所有角色使用相同的攻击状态
        // 如果将来需要不同角色有不同的攻击状态逻辑，可以在这里扩展
        // 例如：return new MeleeAttackState() / new RangedAttackState() 等
        return new CharacterAttackState();
    }
}

