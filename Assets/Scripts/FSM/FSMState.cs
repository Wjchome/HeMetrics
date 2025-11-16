using System;
using System.Collections.Generic;

/// <summary>
/// 状态基类（所有状态需继承此类）
/// </summary>
/// <typeparam name="T">状态持有者类型（如角色、游戏进程管理器）</typeparam>
public abstract class FSMState<T>
{
    /// <summary>状态持有者（如角色实例、游戏管理器实例）</summary>
    protected T Owner { get; private set; }

    /// <summary>初始化状态（持有物赋值）</summary>
    public void Init(T owner)
    {
        Owner = owner;
        OnInitialized();
    }

    /// <summary>状态进入时调用（可传递参数）</summary>
    public virtual void OnEnter() { }

    /// <summary>状态更新时调用（每帧）</summary>
    public virtual void OnUpdate() { }

    /// <summary>状态退出时调用</summary>
    public virtual void OnExit() { }

    /// <summary>初始化完成后调用（可重写做额外初始化）</summary>
    protected virtual void OnInitialized() { }
}

/// <summary>
/// 有限状态机管理器（泛型通用版）
/// </summary>
/// <typeparam name="T">状态持有者类型</typeparam>
/// <typeparam name="TStateType">状态类型枚举（区分不同状态）</typeparam>
public class FSMStateMgr<T, TStateType> where TStateType : Enum
{
    /// <summary>状态持有者（如角色、游戏进程）</summary>
    private readonly T _owner;

    /// <summary>所有注册的状态</summary>
    private readonly Dictionary<TStateType, FSMState<T>> _states = new Dictionary<TStateType, FSMState<T>>();

    /// <summary>当前激活的状态</summary>
    public FSMState<T> CurrentState { get; private set; }

    /// <summary>当前激活的状态类型</summary>
    public TStateType CurrentStateType { get; private set; }

    /// <summary>
    /// 初始化FSM
    /// </summary>
    /// <param name="owner">状态持有者（如角色实例）</param>
    public FSMStateMgr(T owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// 注册状态
    /// </summary>
    /// <param name="stateType">状态类型（枚举）</param>
    /// <param name="state">状态实例</param>
    public void RegisterState(TStateType stateType, FSMState<T> state)
    {
        if (!_states.ContainsKey(stateType))
        {
            state.Init(_owner); // 初始化状态（绑定持有者）
            _states.Add(stateType, state);
        }
    }

    /// <summary>
    /// 切换到目标状态
    /// </summary>
    /// <param name="targetStateType">目标状态类型</param>
    /// <param name="args">传递给目标状态的参数</param>
    public void ChangeState(TStateType targetStateType)
    {
        if (!_states.TryGetValue(targetStateType, out var targetState))
        {
            throw new Exception($"FSM中未注册状态：{targetStateType}");
        }

        // 退出当前状态
        CurrentState?.OnExit();

        // 切换状态
        CurrentState = targetState;
        CurrentStateType = targetStateType;

        // 进入新状态（传递参数）
        CurrentState.OnEnter();
    }

    /// <summary>
    /// 每帧更新当前状态
    /// </summary>
    public void Update()
    {
        CurrentState?.OnUpdate();
    }
}