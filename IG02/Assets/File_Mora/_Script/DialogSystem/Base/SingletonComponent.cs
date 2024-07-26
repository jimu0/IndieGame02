using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonComponent< T >  : MonoBehaviour where T : MonoBehaviour
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
	static T m_Instance = default;
	// -----------------------------------------------
	// 函数
	// -----------------------------------------------
	/// <summary>
	/// 唤醒
	/// </summary>
	void Awake()
	{
		if( m_Instance == null )
		{
			m_Instance = this as T;
			DontDestroyOnLoad( gameObject );
		}
		else
		{
			Destroy( gameObject );
		}
	}
	// -----------------------------------------------
	// 属性
	// -----------------------------------------------
	/// <summary>
	/// 实例
	/// </summary>
	public static T Instance
	{
		get
		{
			if( m_Instance == null )
			{
				var go = new GameObject( $"SingletonComponent -> {typeof(T)}");
				m_Instance = go.AddComponent< T >();
			}
			return m_Instance;
		}
	}
}
