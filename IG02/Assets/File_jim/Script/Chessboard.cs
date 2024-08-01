using System.Collections.Generic;
using UnityEngine;

namespace File_jim.Script
{
    public static class Chessboard
    {
        ///2147483646//int���ֵ
        public static readonly int[,,] Matrix;//��������
        public static readonly Dictionary<int, Vector3Int> Positions;//��ͨ��id��ѯλ�õ��ֵ�
        static Chessboard()
        {
            Matrix = new int[8,23,8];
            Positions = new Dictionary<int, Vector3Int>();
        }
        private static Vector3Int tempPos = Vector3Int.zero;//��ʱ������ת����
        
        /// <summary>
        /// ���þ������ݲ��޸�λ���ֵ�
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
        /// ͨ��λ�û�ȡid
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
                ? Matrix[x, y, z] : 2000000001;//�����ȡ��λ�ó�������Χ����һ������
        }
        /// <summary>
        /// ͨ��id��ȡλ��-û�м���ֵ�
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
            //     Debug.LogError($"positions�ֵ���û��[{id}]�����");
            // }
            return Positions[id];
        }


    }
}

