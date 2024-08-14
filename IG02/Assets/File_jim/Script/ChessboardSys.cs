using System.Collections.Generic;
using File_jim.Scripts.Logic.System.Base;
using UnityEngine;

namespace File_jim.Script
{
    public class ChessboardSys : LogicBaseSystem<ChessboardSys>
    {
    ///2147483646//int????
    private Vector3Int mapSize = new(8, 23, 8);

    public int[,,] matrix; //????????
    public Dictionary<int, Vector3Int> positions; //?????id???????????
    public Dictionary<int, MapTile> mapTiles;

    // public ChessboardSys()
    // {
    //     matrix = new int[mapSize.x, mapSize.y, mapSize.z];
    //     positions = new Dictionary<int, Vector3Int>();
    // }

    
    private static Vector3Int tempPos = Vector3Int.zero; //??????????????
    

    /// <summary>
    /// ??????????????????????
    /// </summary>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    /// <param name="z">z</param>
    /// <param name="i">id</param>
    public void SetMatrixValue(int x, int y, int z, int i)
    {
        if (x >= 0 && x < matrix.GetLength(0) &&
            y >= 0 && y < matrix.GetLength(1) &&
            z >= 0 && z < matrix.GetLength(2))
        {
            matrix[x, y, z] = i;
            tempPos.x = x;
            tempPos.y = y;
            tempPos.z = z;
            positions[i] = tempPos;
        }

    }

    /// <summary>
    /// ?????????id
    /// </summary>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    /// <param name="z">z</param>
    /// <returns></returns>
    public int GetMatrixValue(int x, int y, int z)
    {
        return x >= 0 && x < matrix.GetLength(0) &&
               y >= 0 && y < matrix.GetLength(1) &&
               z >= 0 && z < matrix.GetLength(2)
            ? matrix[x, y, z]
            : 2000000001; //?????????????????????????????????
    }

    /// <summary>
    /// ???id???????-?????????
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Vector3Int GetMatrixPos(int id)
    {
        // Vector3Int v;
        // if (positions.ContainsKey(id))
        // {
        //     v = positions[id];
        // }
        // else
        // {
        //     v = Vector3Int.zero;
        //     Debug.LogError($"positions????????[{id}]?????");
        // }
        return positions[id];
    }


    }
}

