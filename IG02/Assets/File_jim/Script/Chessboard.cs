using UnityEngine;

namespace UITemplate.File_jim.Script
{
    public class Chessboard : MonoBehaviour
    {
        public Vector3Int xyzSize = new (8,99,8); // 棋盘的大小，默认为8*99*8
        public GameObject boxPrefab; // 方块的预制体
        private GameObject[,,] board;

        void Start()
        {
            board = new GameObject[xyzSize.x, xyzSize.y, xyzSize.z];
            
            // 创建一个基础层的棋盘格
            for (int x = 0; x < xyzSize.x; x++)
            {
                for (int z = 0; z < xyzSize.z; z++)
                {
                    GameObject baseTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    baseTile.transform.position = new Vector3(x, 0, z);
                    baseTile.transform.localScale = new Vector3(1, 0.1f, 1);
                    baseTile.GetComponent<Renderer>().material.color = Color.gray; // 设置底层颜色
                    baseTile.transform.parent = transform; // 将底层方块设置为棋盘的子对象
                }
            }
        }
    }
}
