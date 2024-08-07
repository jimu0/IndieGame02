using System;
using System.Collections;
using System.Collections.Generic;
using File_jim.Scripts.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace File_jim.Script
{
    public class Chessboard : MonoBehaviour
    {
        //public Vector3Int mapSize = new(8,23,8);//当前关卡矩阵大小
        public bool stopCoroutine;//控制主驱动器的运行状态
        private const float Pulse = 0.1f;//主驱动器的运动频率
        public static event Action<float> OnMoveBoxesToTarget;//移动事件
        public GameObject boxPrefab;//box预制
        private int nextBoxId = 1;//生成box实例的起始id
        public int uniqueId = 0;//生成Box在棋盘中时的排序id
        private readonly Dictionary<int, GameObject> objsDic = new();//可通过id查询box实例的字典
        private static Vector3Int tempPos = Vector3Int.zero;//临时参数中转变量
        private DataManager dataManager;//data数据处理
        [SerializeField] private string flePath = "/dataTable/Load/";
        [SerializeField] private string mapDataFileName = "mapData";
        [SerializeField] private string mapTileFileName = "mapTile";
        
        private ObjectPool<GameObject> boxPool;

        private void Awake()
        {
            //ChessboardSys.Instance.matrix = new int[mapSize.x, mapSize.y, mapSize.z];
            //matrix = ChessboardSys.Instance.matrix;
            ChessboardSys.Instance.positions = new Dictionary<int, Vector3Int>();
            //positions = ChessboardSys.Instance.positions;
            //ChessboardSys.Instance.mapTiles = new Dictionary<int, MapTile>();
            //mapTiles = ChessboardSys.Instance.mapTiles;
        }

        private void Start()
        {
            
            InitializeBoxes();
            
            StartCoroutine(CallMethodEverySecond());
        }
        

        private void Update()
        {
            //测试：方块生成
            if (Input.GetKeyDown(KeyCode.C))
            {
                //GenerateNewBox_random();
            }
            //作弊：一键消除最底层
            if (Input.GetKeyDown(KeyCode.X))
            {
                for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                {
                    for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                    {
                        DestroyObj(ChessboardSys.Instance.GetMatrixValue(x, 0, z));
                        tempPos.x = x;
                        tempPos.y = 0;
                        tempPos.z = z;
                        SetMatrixV(tempPos, 0);
                    }
                }
            }

        }
        
        GameObject OnCreate()
        {
            return boxPrefab;
        }
        void OnGet(GameObject gameObj)
        {
            Debug.Log("pool:获取");
            //gameObj.SetActive(false);
        }
        void OnRelease(GameObject gameObj)
        {
            Debug.Log("pool:释放");
        }
        void OnDestory(GameObject gameObj)
        {
            Debug.Log("pool:销毁");
        }
        
        /// <summary>
        /// 通知移动
        /// </summary>
        private static void MoveAllBoxesToTarget(float pulse)
        {
            OnMoveBoxesToTarget?.Invoke(pulse);
        }

        /// <summary>
        /// 主驱动器
        /// </summary>
        /// <returns></returns>
        private IEnumerator CallMethodEverySecond()
        {
            while (true)
            {
                if (!stopCoroutine)
                {
                    //消除判断
                    Elimination();
                    //更新数据
                    Metronome();

                    
                }
                //刷新频率
                yield return new WaitForSeconds(Pulse);
            }
        }

        /// <summary>
        /// 更新数据-下坠逻辑
        /// </summary>
        private void Metronome()
        {
            //从底层到顶层遍历，防止覆盖
            for (int y = 1; y < ChessboardSys.Instance.matrix.GetLength(1); y++)
            {
                for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                {
                    for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                    {
                        //检查当前格子是否有箱子
                        if (ChessboardSys.Instance.matrix[x, y, z] != 0)
                        {
                            //检查下面一层是否为空
                            if (ChessboardSys.Instance.matrix[x, y - 1, z] == 0)
                            {
                                //将箱子下移一格
                                int boxId = ChessboardSys.Instance.matrix[x, y, z];
                                ChessboardSys.Instance.matrix[x, y - 1, z] = boxId;
                                ChessboardSys.Instance.matrix[x, y, z] = 0;
                                tempPos.x = x;
                                tempPos.y = y-1;
                                tempPos.z = z;
                                ChessboardSys.Instance.positions[boxId] = tempPos;//更新位置字典
                            }
                        }
                    }
                }
            }
            MoveAllBoxesToTarget(Pulse);//通知移动
        }

        /// <summary>
        /// 消除规则-所有面
        /// </summary>
        private void Elimination()
        {
            //for (int y = matrix.GetLength(1) - 1; y >= 0; y--)
            for (int y = 0; y < ChessboardSys.Instance.matrix.GetLength(1); y++)
            {
                if (!EliminationY(y)) continue;
                for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                {
                    for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                    {
                        //Debug.Log($"{x},{y},{z},{matrix[x, y, z]}");
                        DestroyObj(ChessboardSys.Instance.matrix[x, y, z]);
                        ChessboardSys.Instance.matrix[x, y, z] = 0;
                    }
                }
            }
        }
        /// <summary>
        /// 消除规则-某层面
        /// </summary>
        /// <param name="y">第几层</param>
        private bool EliminationY(int y)
        {
            for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
            {
                for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                {
                    if (ChessboardSys.Instance.matrix[x, y, z] == 0) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 生成一个box_随机(目前有Id冲突问题)
        /// </summary>
        public void GenerateNewBox_random()
        {
            int randomValueX = Random.Range(0, ChessboardSys.Instance.matrix.GetLength(0));
            int randomValueZ = Random.Range(0, ChessboardSys.Instance.matrix.GetLength(2));
            Vector3Int posInt = new(randomValueX, ChessboardSys.Instance.matrix.GetLength(1) - 1, randomValueZ);
            if (ChessboardSys.Instance.GetMatrixValue(posInt.x, posInt.y, posInt.z) == 0)
            {
                int newBoxId = nextBoxId++; //生成一个ID
                SetMatrixV(posInt, newBoxId);
                GameObject newBox = Instantiate(boxPrefab, posInt, Quaternion.identity);
                newBox.name = "Box_" + newBoxId;
                newBox.GetComponent<BoxMovement>().id = newBoxId;
                //实例字典
                if (objsDic.ContainsKey(newBoxId))
                {
                    objsDic[newBoxId] = newBox;
                }
                else
                {
                    objsDic.Add(newBoxId, newBox);
                }
                //位置字典
                if (ChessboardSys.Instance.positions.ContainsKey(newBoxId))
                {
                    ChessboardSys.Instance.positions[newBoxId]=posInt;
                }
                else
                {
                    ChessboardSys.Instance.positions.Add(newBoxId, posInt);
                }

            }
        }

        public void GoPool()
        {
            boxPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestory,
                true, 10, 1000);

        }

        /// <summary>
        /// 生成一个box_指定
        /// </summary>
        public void GenerateNewBox(Vector3Int posInt,int id)
        {
            GameObject newBox = Instantiate(boxPrefab, posInt, Quaternion.identity);
            //GameObject newBox = boxPool.Get();
            newBox.name = "Box_" + id;
            newBox.GetComponent<BoxMovement>().id = id;
            newBox.transform.position = posInt;
            //实例字典
            if (objsDic.ContainsKey(id))
            {
                objsDic[id] = newBox;
            }
            else
            {
                objsDic.Add(id, newBox);
            }
            //位置字典
            if (ChessboardSys.Instance.positions.ContainsKey(id))
            {
                ChessboardSys.Instance.positions[id] = posInt;
            }
            else
            {
                ChessboardSys.Instance.positions.Add(id, posInt);
            }
        }
        
        /// <summary>
        /// 推行为-检查
        /// </summary>
        /// <param name="pos">原位置</param>
        /// <param name="direction">推动方向</param>
        /// <param name="b">允许？</param>
        public static void FallBoxV(Vector3Int pos, Vector3Int direction, out bool b)
        {
            int boxId = GetMatrixV(pos);
            SetMatrixV(pos + direction, boxId, out bool f);
            b = f;
        }

        /// <summary>
        /// 设置矩阵数据
        /// </summary>
        /// <param name="v">位置</param>
        /// <param name="i">id</param>
        private static void SetMatrixV(Vector3Int v, int i)
        {
            ChessboardSys.Instance.SetMatrixValue(v.x, v.y, v.z, i);
        }
        /// <summary>
        /// 设置矩阵数据-检查是不是空位
        /// </summary>
        /// <param name="v">位置</param>
        /// <param name="i">id</param>
        /// <param name="f">允许？</param>
        private static void SetMatrixV(Vector3Int v, int i, out bool f)
        {
            if (ChessboardSys.Instance.GetMatrixValue(v.x, v.y, v.z) == 0)
            {
                ChessboardSys.Instance.SetMatrixValue(v.x, v.y, v.z, i);
                f = true;
            }
            else
            {
                f = false;
            }
        }
        /// <summary>
        /// 获取矩阵数据
        /// </summary>
        /// <param name="v">位置</param>
        /// <returns>id</returns>
        public static int GetMatrixV(Vector3Int v)
        {
            return ChessboardSys.Instance.GetMatrixValue(v.x, v.y, v.z);
        }
        /// <summary>
        /// 获取位置字典
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>位置</returns>
        public static Vector3Int GetMatrixP(int id)
        {
            return ChessboardSys.Instance.GetMatrixPos(id);
        }

        /// <summary>
        /// 根据ID移除实例字典和位置字典中的信息
        /// </summary>
        /// <param name="id">id</param>
        public void RemoveObjsDic(int id)
        {
            if (objsDic.ContainsKey(id))
            {
                objsDic.Remove(id);
                ChessboardSys.Instance.positions.Remove(id);
            }
            else
            {
                Debug.LogWarning($"ID {id} 不存在于字典中。");
            }
        }

        /// <summary>
        /// 根据ID销毁实例并移除实例字典和位置字典中的信息
        /// </summary>
        /// <param name="id"></param>
        public void DestroyObj(int id)
        {
            if (objsDic.TryGetValue(id, out GameObject obj))
            {
                Destroy(obj); // 销毁GameObject
                objsDic.Remove(id);
                ChessboardSys.Instance.positions.Remove(id);
            }
            else
            {
                Debug.LogWarning($"ID {id} 不存在于字典中。");
            }
        }

        /// <summary>
        /// 初始化地图box
        /// </summary>
        private void InitializeBoxes()
        {
            //GameObject dataManagerObj = new("dataManager");
            //DataManager dataManagerC = dataManagerObj.AddComponent<DataManager>();
            DataManager.Instance.SetValues(flePath, mapDataFileName, mapTileFileName, this);
            //dataManagerC.SetValues(flePath, mapDataFileName, mapTileFileName, this);
            DataManager.Instance.LoadMapTile();
            DataManager.Instance.LoadMapData();
        }
        
        
        /// <summary>
        /// 绘制矩阵数据可视化
        /// </summary>
        void OnDrawGizmos()
        {
            if (ChessboardSys.Instance.matrix == null) return;

            for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
            {
                for (int y = 0; y < ChessboardSys.Instance.matrix.GetLength(1); y++)
                {
                    for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                    {
                        if (ChessboardSys.Instance.matrix[x, y, z] != 0)
                        {
                            // 设置 Gizmos 的颜色
                            Gizmos.color = Color.red;
                            // 绘制一个球体，表示非零位置
                            Gizmos.DrawSphere(new Vector3(x, y, z), 0.05f);
                        }
                    }
                }
            }
        }
    }
}