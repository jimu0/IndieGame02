using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ScriptableBase< T,Data > : ScriptableObject 
	where T : ScriptableBase< T, Data > 
	where Data : ScriptableData< Data >, new()
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
	public List< Data > List = new List< Data >();
	public string pathname;
	public static string assetName;
	// -----------------------------------------------
	// 函数
	// -----------------------------------------------
	/// <summary>
	/// 尝试获取数据
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public bool TryGetData( int id, out Data data )
    {
		data = null;
		for ( var i = 0; i < List.Count; ++i )
        {
			var check = List[ i ];
			if( check.Id != id ) continue;
			data = check;
			break;
		}

		return data != null;
    }

#if UNITY_EDITOR
	/// <summary>
	/// 实例获取,编辑器专用
	/// </summary>
	/// <returns></returns>
	public static T ResourceLoad(string filename)
    {
		var assetPath = GetAssetPath(filename);
		var instance = AssetDatabase.LoadAssetAtPath<T>( assetPath );
		if( instance == null )
		{
			instance = ScriptableObject.CreateInstance<T>();
			instance.name = $"{typeof(T)}";
			EditorUtility.SetDirty( instance );
			AssetDatabase.CreateAsset( instance, assetPath );
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		return instance;
    }

	/// <summary>
	/// 拷贝
	/// </summary>
	/// <param name="source"></param>
	public void CopyFrom( T source )
    {
		List.Clear();
		for( var i = 0; i < source.List.Count; ++i )
        {
			var data = source.List[ i ];
			var newData = new Data();
			newData.CopyFrom( data );
			List.Add( newData );
		}
	}

	/// <summary>
	/// 资源路径获取
	/// </summary>
	/// <returns></returns>
	public static string GetAssetPath(string filename)
    {
		return $"Assets/SourceRosources/Master/{filename}.asset";
    }
#endif
	// -----------------------------------------------
	// 属性
	// -----------------------------------------------
}
