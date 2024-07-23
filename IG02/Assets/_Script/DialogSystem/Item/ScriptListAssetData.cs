using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ScriptListAssetData : ScriptableBase<ScriptListAssetData, ItemContainer>
{
#if UNITY_EDITOR

    /// <summary>
    /// ���
    /// </summary>
    public void Add(ItemContainer command)
    {
        List.Add(command);
    }

    /// <summary>
    /// ɾ��
    /// </summary>
    public void RemoveAt(int index)
    {
        List.RemoveAt(index);
    }

    /// <summary>
    /// �����ƶ�
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
    /// �����ƶ�
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
    /// ����
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
