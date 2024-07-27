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
            //UIManager.Instance.completeLevel(true);
        }
        
        
        void Update()
        {
            // 检测是否按下 'v' 键
            if (Input.GetKeyDown(KeyCode.V))
            {
                AddRandomBoxLayer();
            }
        }
        
        void AddRandomBoxLayer()
        {
            // 随机生成一个新的方块层
            for (int x = 0; x < xyzSize.x; x++)
            {
                for (int z = 0; z < xyzSize.z; z++)
                {
                    if (Random.value > 0.8f) // 20% 概率生成方块
                    {
                        // 找到当前格子中的最高位置
                        int y = FindHighestPosition(x, z);
                        // 创建新方块
                        GameObject newBox = Instantiate(boxPrefab, new Vector3(x, y, z), Quaternion.identity);
                        newBox.transform.parent = transform; // 将新方块设置为棋盘的子对象
                        board[x, y, z] = newBox; // 更新棋盘格子的方块信息
                    }
                }
            }
        }

        int FindHighestPosition(int x, int z)
        {
            for (int y = 0; y < xyzSize.y; y++)
            {
                if (board[x, y, z] == null)
                {
                    return y;
                }
            }
            return xyzSize.y - 1; // 如果所有位置都被占用，返回最高位置
        }
    }
}
