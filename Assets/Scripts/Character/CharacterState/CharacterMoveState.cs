public class CharacterMoveState : FSMState<Character>
{
    public override void OnUpdate()
    {
        Owner.UpdateTarget();
    }
}