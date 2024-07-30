using File_jim.Scripts.ObjectPool;
using UnityEngine;

namespace File_jim.Script
{
    public class Chessboard : MonoBehaviour
    {
        public Vector3Int xyzSize = new (8,99,8);
        public GameObject boxPrefab; 
        private GameObject[,,] board;
        public static int[,,] matrix;
        private ObjectPool<GameObject> boxPool;
        //2147483646
        
        void Start()
        {
            boxPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestory,
                true, 10, 1000);
            board = new GameObject[xyzSize.x, xyzSize.y, xyzSize.z];
            matrix = new int[xyzSize.x, xyzSize.y, xyzSize.z];
            
            //UIManager.Instance.completeLevel(true);
            
            //ReFreshDisplay();
        }


        public static void SetMatrixValue(int x,int y,int z,int i)
        {
            if (x >= 0 && x < matrix.GetLength(0) &&
                y >= 0 && y < matrix.GetLength(1) &&
                z >= 0 && z < matrix.GetLength(2)) 
                matrix[x, y, z] = i;
        }

        public static int GetMatrixValue(int x,int y,int z)
        {
            return x >= 0 && x < matrix.GetLength(0) &&
                   y >= 0 && y < matrix.GetLength(1) &&
                   z >= 0 && z < matrix.GetLength(2)
                ? matrix[x, y, z] : 99;
        }

        public void ReFreshDisplay()
        {
            for (int y = 0; y < xyzSize.y-1; y++)
            {
                for (int z = 0; z < xyzSize.z-1; z++)
                {
                    for (int x = 0; x < xyzSize.x-1; x++)
                    {
                        if (matrix[x, y, z] > 0)
                        {
                            boxPool.Get();
                        }
                        else
                        {
                            if (board[x, y, z] != null) boxPool.Release(board[x, y, z]);
                        }
                    }
                }
            }
        }

        GameObject OnCreate()
        {
            return new GameObject("Wokao");
        }
        void OnGet(GameObject gameObject)
        {
            Debug.Log("Onget");
        }
        void OnRelease(GameObject gameObject)
        {
            Debug.Log("OnRelease");
        }
        void OnDestory(GameObject gameObject)
        {
            Debug.Log("OnDestory");
        }

        void AddRandomBoxLayer()
        {
            for (int x = 0; x < xyzSize.x; x++)
            {
                for (int z = 0; z < xyzSize.z; z++)
                {
                    if (Random.value > 0.8f) // 20%
                    {
                        
                        int y = FindHighestPosition(x, z);
                        
                        GameObject newBox = Instantiate(boxPrefab, new Vector3(x, y, z), Quaternion.identity);
                        newBox.transform.parent = transform; 
                        board[x, y, z] = newBox;
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
            return xyzSize.y - 1;
        }
        
        
        public float pointSize = 0.1f;

        private void OnDrawGizmos()
        {
            if (matrix == null) return;

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    for (int z = 0; z < matrix.GetLength(2); z++)
                    {
                        int value = matrix[x, y, z];

                        if (value > 0)
                        {
                            Gizmos.color = Color.red;
                            DrawDebugPoint(new Vector3(x, y+0.5f, z), pointSize);
                        }
                        else if (value < 0)
                        {
                            Gizmos.color = Color.blue;
                            DrawDebugPoint(new Vector3(x, y+0.5f, z), pointSize);
                        }
                    }
                }
            }
        }


        private void DrawDebugPoint(Vector3 position, float size)
        {
            Gizmos.DrawCube(position, Vector3.one * size);
        }
    }
}

