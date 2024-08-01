using System.Collections.Generic;
using UnityEngine;

namespace File_jim.Script
{
    public static class Chessboard
    {
        ///2147483646//int最大值
        public static readonly int[,,] Matrix;//矩阵数据
        public static readonly Dictionary<int, Vector3Int> Positions;//可通过id查询位置的字典
        static Chessboard()
        {
            Matrix = new int[8,23,8];
            Positions = new Dictionary<int, Vector3Int>();
        }
        private static Vector3Int tempPos = Vector3Int.zero;//临时参数中转变量
        
        /// <summary>
        /// 设置矩阵数据并修改位置字典
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="i">id</param>
        public static void SetMatrixValue(int x,int y,int z,int i)
        {
            if (x >= 0 && x < Matrix.GetLength(0) &&
                y >= 0 && y < Matrix.GetLength(1) &&
                z >= 0 && z < Matrix.GetLength(2))
            {
                Matrix[x, y, z] = i;
                tempPos.x = x;
                tempPos.y = y;
                tempPos.z = z;
                Positions[i] = tempPos;
            }

        }
        /// <summary>
        /// 通过位置获取id
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns></returns>
        public static int GetMatrixValue(int x,int y,int z)
        {
            return x >= 0 && x < Matrix.GetLength(0) &&
                   y >= 0 && y < Matrix.GetLength(1) &&
                   z >= 0 && z < Matrix.GetLength(2)
                ? Matrix[x, y, z] : 2000000001;//如果获取的位置超出矩阵范围，赋一个大数
        }
        /// <summary>
        /// 通过id获取位置-没有检查字典
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Vector3Int GetMatrixPos(int id)
        {
            // Vector3Int v;
            // if (positions.ContainsKey(id))
            // {
            //     v = positions[id];
            // }
            // else
            // {
            //     v = Vector3Int.zero;
            //     Debug.LogError($"positions字典中没有[{id}]这个键");
            // }
            return Positions[id];
        }


    }
}

