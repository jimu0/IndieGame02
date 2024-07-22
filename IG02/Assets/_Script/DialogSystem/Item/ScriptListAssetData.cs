using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ScriptListAssetData : ScriptableBase<ScriptListAssetData, ItemContainer>
{
#if UNITY_EDITOR

    /// <summary>
    /// 添加
    /// </summary>
    public void Add(ItemContainer command)
    {
        List.Add(command);
    }

    /// <summary>
    /// 删除
    /// </summary>
    public void RemoveAt(int index)
    {
        List.RemoveAt(index);
    }

    /// <summary>
    /// 向上移动
    /// </summary>
    public void MoveUp(int index)
    {
        if (index <= 0)
        {
            return;
        }

        Swap(index, index - 1);

    }

    /// <summary>
    /// 向下移动
    /// </summary>
    /// <param name="index"></param>
    public void MoveDown(int index)
    {
        if (index >= List.Count - 1)
        {
            return;
        }

        Swap(index, index + 1);
    }

    /// <summary>
    /// 交换
    /// </summary>
    /// <param name="fromIndex"></param>
    /// <param name="toIndex"></param>
    void Swap(int fromIndex, int toIndex)
    {
        var fromCommand = List[fromIndex];
        var toCommand = List[toIndex];
        List[toIndex] = fromCommand;
        List[fromIndex] = toCommand;
    }
#endif
}
