using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 简易状态FSM状态管理
/// </summary>
/// <typeparam name="T"></typeparam>
public class StateContext< T > where T : Enum
{
	// -----------------------------------------------
	// 内部类
	// -----------------------------------------------
	// -----------------------------------------------
	// 枚举
	// -----------------------------------------------
	// -----------------------------------------------
	// 定义值
	// -----------------------------------------------
	// -----------------------------------------------
	// 公共变量
	// -----------------------------------------------
	// -----------------------------------------------
	// 成员变量
	// -----------------------------------------------
	Dictionary< T , ( Action enter, Action update, Action exit ) > m_ActionMaps
		= new Dictionary<T, (Action enter, Action update, Action exit)>();

	T m_PrevState;
	T m_CurrentState;
	// -----------------------------------------------
	// 函数
	// -----------------------------------------------
	/// <summary>
	/// 注册回调,登录函数可置空
	/// </summary>
	/// <param name="type"></param>
	/// <param name="enter"></param>
	/// <param name="update"></param>
	/// <param name="exit"></param>
	public void Add( T type, Action enter = null, Action update = null, Action exit = null )
    {
		if( m_ActionMaps.TryGetValue( type, out var actions ) )
        {
			Debug.LogWarning( $"StateContext -> 已经存在的枚举{type},添加回调失败" );
			return;
        }

		actions.enter = enter;
		actions.update = update;
		actions.exit = exit;

		m_ActionMaps.Add( type, actions );
	}

	/// <summary>
	/// 更新
	/// </summary>
	public void Update()
    {
		if( !m_ActionMaps.TryGetValue( m_CurrentState, out var currentActions ) )
        {
			Debug.LogWarning( $"StateContext -> 不存在当前状态{m_CurrentState}标记的回调群,无法执行更新处理" );
			return;
        }

		currentActions.update?.Invoke();
	}

	/// <summary>
	/// 状态设定
	/// </summary>
	/// <param name="type"></param>
	public void SetState( T state )
    {
		if( !m_ActionMaps.TryGetValue( state, out var toActions ) )
        {
			Debug.LogWarning( $"StateContext -> 不存在{state}标记的回调群,设定失败" );
			return;
        }

		if( m_ActionMaps.TryGetValue( m_CurrentState, out var fromActions ) )
        {
			fromActions.exit?.Invoke();
        }

		toActions.enter?.Invoke();
		m_PrevState = m_CurrentState;
		m_CurrentState = state;
    }
	// -----------------------------------------------
	// 属性
	// -----------------------------------------------
	/// <summary>
	/// 当前状态
	/// </summary>
	public T CurrentState => m_CurrentState;

	/// <summary>
	/// 过去状态
	/// </summary>
	public T PrevState => m_PrevState;
}
