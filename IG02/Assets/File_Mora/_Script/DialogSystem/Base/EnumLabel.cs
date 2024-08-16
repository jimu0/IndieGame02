using System;
using System.Reflection;

public class EnumLabel : Attribute
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
    string m_Text;
    // -----------------------------------------------
    // 成员变量
    // -----------------------------------------------
    // -----------------------------------------------
    // 函数
    // -----------------------------------------------
    public EnumLabel( string text )
    {
        m_Text = text;
    }

    /// <summary>
    /// 枚举定义获取
    /// </summary>
    /// <param name="en"></param>
    /// <returns></returns>
    public static string GetEnumLabel<T>( T en ) where T : Enum
    {
        var type = en.GetType();

        var memInfos = type.GetMember( en.ToString() );
        if( memInfos != null && memInfos.Length > 0)
        {
            object[] attrs = memInfos[0].GetCustomAttributes( typeof(EnumLabel), false );
            if( attrs != null && attrs.Length > 0 )
            {
                return ( (EnumLabel)attrs[0]).Text;
            }
        }
        return en.ToString();
    }

    /// <summary>
    /// 枚举定义获取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string[] GetEnumLabels<T>() where T : Enum
    {
        var enums = Enum.GetValues( typeof(T) );
        var labels = new string[ enums.Length ];
        for( var i = 0; i < enums.Length; ++i )
        {
            labels[ i ] = GetEnumLabel( (T)enums.GetValue( i ));
        }

        return labels;
    }

    public string Text  => m_Text;
}