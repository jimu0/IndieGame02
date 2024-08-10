using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using File_jim.Scripts.ObjectPool;
using UITemplate;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace File_jim.Script
{
    public class Chessboard : MonoBehaviour
    {
        private Vector3Int matrixSize;//关卡尺寸(读取mapData得到)
        public bool stopCoroutine;//驱动器暂停
        private const float Pulse = 0.1f;//驱动器心跳频率
        public static event Action<float> OnMoveBoxesToTarget;//
        public GameObject boxPrefab;//Box基础预制
        private int nextBoxId = 1;//起始id
        public int uniqueId = 0;//注册Box的Id时的当前序号
        public readonly Dictionary<int, GameObject> objsDic = new();//id对应box实例的字典
        private static Vector3Int tempPos = Vector3Int.zero;//临时变量
        private DataManager dataManager;//data管理器
        [SerializeField] private string flePath = "/dataTable/Load/";
        [SerializeField] private string mapDataFileName = "mapData";
        [SerializeField] private string mapTileFileName = "mapTile";
        public GameObject grid;
        //private ObjectPool<GameObject> boxPool;

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
            
            InitializeBoxes();//生成方块资源
            SetGrid();//设置栅格
            StartCoroutine(CallMethodEverySecond());//驱动器
        }
        

        private void Update()
        {
            //
            if (Input.GetKeyDown(KeyCode.C))
            {
                //GenerateNewBox_random();
            }
            //作弊：强行消除玩家所在行下方的所有方块
            if (Input.GetKeyDown(KeyCode.X))
            {
                int y = Mathf.RoundToInt(FindObjectOfType<PlayerController2>().transform.position.y + 0.4f) - 1;
                if (EliminationY(y))
                {
                    for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                    {
                        for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                        {
                            int boxId = ChessboardSys.Instance.matrix[x, y, z];
                            int tileId = boxId / 10000;//去掉四位数的唯一id即是类id
                            if (ChessboardSys.Instance.mapTiles[tileId].Hp > 1) continue;

                            DestroyObj(boxId);
                            ChessboardSys.Instance.matrix[x, y, z] = 0;
                        }
                    }
                }
            

            }

        }
        
        // GameObject OnCreate()
        // {
        //     return boxPrefab;
        // }
        // void OnGet(GameObject gameObj)
        // {
        //     Debug.Log("pool:��ȡ");
        //     //gameObj.SetActive(false);
        // }
        // void OnRelease(GameObject gameObj)
        // {
        //     Debug.Log("pool:�ͷ�");
        // }
        // void OnDestory(GameObject gameObj)
        // {
        //     Debug.Log("pool:����");
        // }
        
        /// <summary>
        /// 位置更新通知
        /// </summary>
        private static void MoveAllBoxesToTarget(float pulse)
        {
            OnMoveBoxesToTarget?.Invoke(pulse);
        }

        /// <summary>
        /// 驱动器
        /// </summary>
        /// <returns></returns>
        private IEnumerator CallMethodEverySecond()
        {
            while (true)
            {
                if (!stopCoroutine)
                {
                    //判断消除
                    Elimination();
                    //自由落体
                    Metronome();

                    
                }
                yield return new WaitForSeconds(Pulse);//心跳
            }
        }

        /// <summary>
        /// 执行自由落体
        /// </summary>
        private void Metronome()
        {
            //从下至上遍历避免覆盖
            for (int y = 1; y < ChessboardSys.Instance.matrix.GetLength(1); y++)
            {
                for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                {
                    for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                    {
                        int boxId = ChessboardSys.Instance.matrix[x, y, z];
                        //判断是不是id0
                        if (boxId != 0)
                        {
                            //判断下面是不是id0
                            if (ChessboardSys.Instance.matrix[x, y - 1, z] == 0)
                            {
                                int tileId = boxId / 10000;//去掉四位数的唯一id即是类id
                                if (ChessboardSys.Instance.mapTiles[tileId].Gravity)
                                {
                                    //向下位移设置
                                    ChessboardSys.Instance.matrix[x, y - 1, z] = boxId;
                                    ChessboardSys.Instance.matrix[x, y, z] = 0;
                                    tempPos.x = x;
                                    tempPos.y = y-1;
                                    tempPos.z = z;
                                    ChessboardSys.Instance.positions[boxId] = tempPos;
                                }


                            }
                        }
                    }
                }
            }
            MoveAllBoxesToTarget(Pulse);//֪ͨ�ƶ�
        }

        /// <summary>
        /// 消除所有合格面
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
                        int boxId = ChessboardSys.Instance.matrix[x, y, z];
                        int tileId = boxId / 10000;//去掉四位数的唯一id即是类id
                        if (ChessboardSys.Instance.mapTiles[tileId].Hp > 1) continue;

                        DestroyObj(boxId);
                        ChessboardSys.Instance.matrix[x, y, z] = 0;
                    }
                }
            }
        }
        /// <summary>
        /// 消除判断
        /// </summary>
        /// <param name="y">检查结果</param>
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

        // /// <summary>
        // /// ����һ��box_���(Ŀǰ��Id��ͻ����)
        // /// </summary>
        // public void GenerateNewBox_random()
        // {
        //     int randomValueX = Random.Range(0, ChessboardSys.Instance.matrix.GetLength(0));
        //     int randomValueZ = Random.Range(0, ChessboardSys.Instance.matrix.GetLength(2));
        //     Vector3Int posInt = new(randomValueX, ChessboardSys.Instance.matrix.GetLength(1) - 1, randomValueZ);
        //     if (ChessboardSys.Instance.GetMatrixValue(posInt.x, posInt.y, posInt.z) == 0)
        //     {
        //         int newBoxId = nextBoxId++; //����һ��ID
        //         SetMatrixV(posInt, newBoxId);
        //         GameObject newBox = Instantiate(boxPrefab, posInt, Quaternion.identity);
        //         newBox.name = "Box_" + newBoxId;
        //         newBox.GetComponent<Block>().id = newBoxId;
        //         //ʵ���ֵ�
        //         if (objsDic.ContainsKey(newBoxId))
        //         {
        //             objsDic[newBoxId] = newBox;
        //         }
        //         else
        //         {
        //             objsDic.Add(newBoxId, newBox);
        //         }
        //         //λ���ֵ�
        //         if (ChessboardSys.Instance.positions.ContainsKey(newBoxId))
        //         {
        //             ChessboardSys.Instance.positions[newBoxId]=posInt;
        //         }
        //         else
        //         {
        //             ChessboardSys.Instance.positions.Add(newBoxId, posInt);
        //         }
        //
        //     }
        // }

        // public void GoPool()
        // {
        //     boxPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestory,
        //         true, 10, 1000);
        //
        // }

        /// <summary>
        /// 创建一个新方块
        /// </summary>
        public void GenerateNewBox(Vector3Int posInt,int id,int soleId)
        {
            GameObject newBox = Instantiate(boxPrefab, posInt, Quaternion.identity);
            
            //GameObject newBox = boxPool.Get();
            newBox.name = "Box_" + soleId;
            Block block = newBox.GetComponent<Block>();
            block.id = soleId;

            //Debug.Log(ChessboardSys.Instance.mapTiles[id].Mesh);
            if (ChessboardSys.Instance.mapTiles.TryGetValue(id, out MapTile tile))
            {
                block.SetAbility(tile);
                MeshFilter meshFilter = newBox.GetComponent<MeshFilter>();
                if (meshFilter != null)meshFilter.sharedMesh = tile.Mesh;
                MeshRenderer meshRenderer = newBox.GetComponent<MeshRenderer>();
                if (meshRenderer != null)meshRenderer.sharedMaterial = tile.Material;
            }
            else Debug.LogWarning($"没有找到id:[{id}]的mapTile数据");

            newBox.transform.position = posInt;
            
            if (objsDic.ContainsKey(soleId))
            {
                objsDic[soleId] = newBox;
            }
            else
            {
                objsDic.Add(soleId, newBox);
            }
            if (ChessboardSys.Instance.positions.ContainsKey(soleId))
            {
                ChessboardSys.Instance.positions[soleId] = posInt;
            }
            else
            {
                ChessboardSys.Instance.positions.Add(soleId, posInt);
            }
        }
        
        /// <summary>
        /// 方块运动行为
        /// </summary>
        /// <param name="pos">当前位置</param>
        /// <param name="direction">方向</param>
        /// <param name="b">检查结果</param>
        public static void FallBoxP(Vector3Int pos, Vector3Int direction, out bool b)
        {
            int boxId = GetMatrixV(pos);
            SetMatrixV(pos + direction, boxId, out bool f);
            if (f) SetMatrixV(pos, 0);
            b = f;
        }

        /// <summary>
        /// 矩阵数据设置：设置位置上的ID，直接
        /// </summary>
        /// <param name="v">位置坐标</param>
        /// <param name="i">id</param>
        private static void SetMatrixV(Vector3Int v, int i)
        {
            ChessboardSys.Instance.SetMatrixValue(v.x, v.y, v.z, i);
        }
        /// <summary>
        /// 矩阵数据设置：设置位置上的ID，有ID则返回冲突结果
        /// </summary>
        /// <param name="v">位置坐标</param>
        /// <param name="i">id</param>
        /// <param name="f">检查结果</param>
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
        /// 矩阵数据获取：通过坐标获取ID
        /// </summary>
        /// <param name="v">坐标位置</param>
        /// <returns>id</returns>
        public static int GetMatrixV(Vector3Int v)
        {
            return ChessboardSys.Instance.GetMatrixValue(v.x, v.y, v.z);
        }
        /// <summary>
        /// 位置列表获取：通过ID获取坐标
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>坐标位置</returns>
        public static Vector3Int GetMatrixP(int id)
        {
            return ChessboardSys.Instance.GetMatrixPos(id);
        }

        /// <summary>
        /// 通过ID Remove方块列表的Key
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
                Debug.LogWarning($"没有在objsDic中找到ID[{id},无法Remove这个Key]");
            }
        }

        /// <summary>
        /// 通过ID销毁方块并清除相关列表
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
                Debug.LogWarning($"没有在objsDic中找到ID[{id}]");
            }
        }

        /// <summary>
        /// 更具配置表生成方块资源
        /// </summary>
        private void InitializeBoxes()
        {
            //GameObject dataManagerObj = new("dataManager");
            //DataManager dataManagerC = dataManagerObj.AddComponent<DataManager>();
            DataManager.Instance.SetValues(flePath, mapDataFileName, mapTileFileName, this);
            //dataManagerC.SetValues(flePath, mapDataFileName, mapTileFileName, this);
            DataManager.Instance.LoadMapTile();
            DataManager.Instance.LoadMapData(out Vector3Int mapSize);
            matrixSize = mapSize;
        }

        /// <summary>
        /// 设置底层栅格与边界，匹配关卡尺寸。
        /// </summary>
        private void SetGrid()
        {
            GameObject gridMesh = grid.transform.Find("GridMesh")?.gameObject;
            if (gridMesh != null)
            {
                Vector3Int gridSize = matrixSize;
                gridSize.x = matrixSize.x;
                gridSize.y = matrixSize.y;
                gridSize.z = 1;
                Vector2 textureTiling = new (gridSize.x, gridSize.y); 
                Vector3 gridMeshLocalPos = new (gridSize.x*0.5f-0.5f,0,gridSize.y*0.5f-0.5f);
                gridMesh.transform.localPosition = gridMeshLocalPos;
                gridMesh.transform.localScale = gridSize;
                Renderer gridMeshRenderer = gridMesh.GetComponent<Renderer>();
                gridMeshRenderer.material.SetTextureScale("_MainTex", textureTiling);
                
                GameObject colliderRoot = gridMesh.transform.Find("ColliderRoot")?.gameObject;
                if (colliderRoot != null) colliderRoot.transform.localScale = new(1, matrixSize.y, 1);

            }
            else
            {
                Debug.LogError("GridMesh不存在，无法获取");
            }
        }

        /// <summary>
        /// 绘制调试图形
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
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(new Vector3(x, y, z), 0.05f);
                        }
                    }
                }
            }
        }
    }
}