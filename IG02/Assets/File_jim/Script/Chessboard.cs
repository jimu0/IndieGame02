using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using File_jim.Script.ViewAngle;
using UITemplate;
using UnityEngine;

using Color = UnityEngine.Color;


namespace File_jim.Script
{
    public class Chessboard : MonoBehaviour
    {
        public int levelId = 1;
        private Vector3Int matrixSize;//关卡尺寸(读取mapData得到)
        [Header("驱动器暂停")]
        public bool stopCoroutine;//驱动器暂停
        private const float Pulse = 0.1f;//驱动器心跳频率
        public static event Action<float> OnMoveBoxesToTarget;//
        public static event Action<float> OnDecisionRoleAllToTarget;
        public static event Action OnGameService;
        public Block blockPrefab;//Box基础预制
        //private int nextBoxId = 1;//起始id
        public int uniqueId = 0;//注册Box的Id时的当前序号
        public readonly Dictionary<int, Block> objsDic = new();//id对应box实例的字典
        private static Vector3Int tempPos = Vector3Int.zero;//临时变量
        private DataManager dataManager;//data管理器
        [SerializeField] private string flePath = "/dataTable/Load/";
        [SerializeField] public string mapDataFileName = "mapData";
        [SerializeField] private string mapTileFileName = "mapTile";
        public GameObject grid;
        public PlayerController2 player;
        public Vector3Int playerOldPos; //玩家前一回合的位置
        public bool playerDead = true;
        public CmSwitchHeight cmSwitchHeight;
        public CameraChange cmVcam;
        public Vector3Int startPos;
        public GameObject fx_eliminate;
        public GameObject fx_end;
        
        /// <summary>
        /// 储藏一个矩阵坐标于Id
        /// </summary>
        struct CachedPosition
        {
            public Vector3Int position;
            public int originalID;

            public CachedPosition(Vector3Int pos, int id)
            {
                position = pos;
                originalID = id;
            }
        }
        private CachedPosition? cachedPosition = null;
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
            
            GenerativePlayer(startPos);//生成玩家
            StartCoroutine(CallMethodEverySecond());//驱动器

        }
        

        private void Update()
        {
            if(player) stopCoroutine = player.DesignerMode;
            //
            if (Input.GetKeyDown(KeyCode.C))
            {
                //GenerateNewBox_random();
            }
            //作弊：玩家所在行下方的所有方块强行-1Hp
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!stopCoroutine)
                {
                    int y = Mathf.RoundToInt(FindObjectOfType<PlayerController2>().transform.position.y + 0.4f) - 1;
                    if (EliminationY(y))
                    {
                        for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                        {
                            for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                            {
                                int boxId = ChessboardSys.Instance.matrix[x, y, z];
                                objsDic[boxId].SetHp(-1);
                            }
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
        /// block更新通知
        /// </summary>
        private static void MoveAllBoxesToTarget(float pulse)
        {
            OnMoveBoxesToTarget?.Invoke(pulse);
        }        
        /// <summary>
        /// player判定通知
        /// </summary>
        private static void DecisionRoleAllToTarget(float obj)
        {
            OnDecisionRoleAllToTarget?.Invoke(obj);
        }
        /// <summary>
        /// 胜利条件判定通知
        /// </summary>
        private static void GameService()
        {
            OnGameService?.Invoke();
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
                    //更新角色在矩阵中的位置
                    UpdatePlayerPositionInMatrix();
                    //角色判定
                    DecisionRole();
                    //判断消除
                    EliminationLayer();
                    //自由落体
                    Metronome();
                    
                    //游戏判定
                    GameService();


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
                        if (boxId == 0)
                        {
                        }
                        //判断是不是block规则的id
                        else if (boxId is < 100000000 and > 0)
                        {
                            if (boxId == 10)
                            {
                                int boxDId = ChessboardSys.Instance.matrix[x, y - 1, z];
                                if (boxDId == 0) { }
                                else
                                {
                                    if (boxDId/10000!=10004)
                                    {
                                        objsDic[boxDId].TriggerBeEncroach(boxId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int boxDId = ChessboardSys.Instance.matrix[x, y - 1, z];
                            //判断下面是不是id0
                            if (boxDId == 0)
                            {
                                //【方块属性：重力检查】
                                if (objsDic.ContainsKey(boxId) && objsDic[boxId].boxAbi.Gravity)
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
                            else if(boxDId == 10)//临时判断是不是玩家单位
                            {
                            }
                            else
                            {
                                //【方块技能：侵占】
                                if(boxDId/10000!=10004)objsDic[boxDId].TriggerBeEncroach(boxId);
                            }
                        }
                    }
                }
            }
            MoveAllBoxesToTarget(Pulse);//֪ͨ�ƶ�
        }

        private void UpdatePlayerPositionInMatrix()
        {
            Vector3Int playerNewPos = default;
            Vector3 position = player.transform.position;
            playerNewPos.x = Mathf.RoundToInt(position.x);
            playerNewPos.y = Mathf.RoundToInt(position.y+0.4f);
            playerNewPos.z = Mathf.RoundToInt(position.z);
            RestorePreviousPosition();
            if (playerNewPos.x < matrixSize.x && playerNewPos.y < matrixSize.y &&
                playerNewPos.z < matrixSize.z)
            {
                if (playerNewPos != playerOldPos)
                {
                    if (cachedPosition.HasValue)
                    {
                        //将之前缓存的ID还原到矩阵中
                        ChessboardSys.Instance.matrix[cachedPosition.Value.position.x, cachedPosition.Value.position.y, cachedPosition.Value.position.z] = cachedPosition.Value.originalID;
                    }
                    //缓存新位置的ID
                    int originalID = ChessboardSys.Instance.matrix[playerNewPos.x, playerNewPos.y, playerNewPos.z];
                    cachedPosition = new CachedPosition(playerNewPos, originalID);

                    //将玩家的ID放到新位置
                    ChessboardSys.Instance.matrix[playerNewPos.x, playerNewPos.y, playerNewPos.z] = 10;
                    playerOldPos = playerNewPos;
                }
            }
        }

        public void RestorePreviousPosition()
        {
            if (cachedPosition.HasValue)
            {
                ChessboardSys.Instance.matrix[cachedPosition.Value.position.x, cachedPosition.Value.position.y, cachedPosition.Value.position.z] = cachedPosition.Value.originalID;
                cachedPosition = null; // 清空缓存
            }
        }
        
        private void DecisionRole()
        {
            if (playerOldPos.x < matrixSize.x && playerOldPos.y <= matrixSize.y &&
                playerOldPos.z <= matrixSize.z)
            {
                DecisionRoleAllToTarget(Pulse);
                // int boxDId = ChessboardSys.Instance.matrix[playerOldPos.x, playerOldPos.y - 1, playerOldPos.z];
                // if (boxDId == 0)
                // {  
                //     
                // }                            
                // if (boxDId == 0)
                // {
                //                 
                // }
            }

            

        }

        /// <summary>
        /// 消除所有合格面
        /// </summary>
        private void EliminationLayer()
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
                        objsDic[boxId].SetHp(-1);
                        
                        // int hp = objsDic[boxId].boxAbi.Hp--;
                        // if (hp > 0) continue;
                        //
                        // EliminationOne(boxId);
                    }
                }
                AudioManager.instance.Play("eliminate");

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
                    int id = ChessboardSys.Instance.matrix[x, y, z];
                    if (id is < 100000000 and >= 0) return false;
                }
            }
            return true;
        }
        private void EliminationOne(int soleId)
        {
            DestroyObj(soleId);
        }
        private void EliminationCategory(int id)
        {
        }
        

        /// <summary>
        /// 创建一个新方块
        /// </summary>
        public void GenerateNewBox(Vector3Int posInt,int id,int soleId)
        {
            
            if (ChessboardSys.Instance.mapTiles.TryGetValue(id, out MapTile tile))
            {
                Block block = Instantiate(blockPrefab, posInt, Quaternion.identity);
                block.name = "Box_" + soleId;
                block.chessboard = this;
                block.id = soleId;
                block.boxAbi.Id = id;
                block.boxAbi = tile;//SetBoxAbility(tile);
                if (block.meshFilter != null)block.meshFilter.sharedMesh = tile.Mesh;
                if (block.meshRenderer != null)block.meshRenderer.sharedMaterial = tile.Material;

                ChessboardSys.Instance.SetMatrixValue(posInt.x, posInt.y, posInt.z, soleId);
                block.gameObject.transform.position = posInt;
                if (objsDic.ContainsKey(soleId))
                {
                    objsDic[soleId] = block;
                }
                else
                {
                    objsDic.Add(soleId, block);
                }
                
                if (ChessboardSys.Instance.positions.ContainsKey(soleId))
                {
                    ChessboardSys.Instance.positions[soleId] = posInt;
                }
                else
                {
                    ChessboardSys.Instance.positions.Add(soleId, posInt);
                }

                if (id == 10006)//传终点SoleId给服务器
                {
                    GameService gameService = FindObjectOfType<GameService>();
                    gameService.SetEndPosId(soleId);
                }


            }
            else
            {
                Debug.LogWarning($"没有找到id:[{id}]的mapTile数据");
            }

            

        }
        
        /// <summary>
        /// 初始化玩家单位
        /// </summary>
        public void GenerativePlayer(Vector3Int startPos)
        {
            var p=Instantiate(player, startPos, Quaternion.identity);
            player = p;
            player.gameObject.SetActive(true);
            playerDead = false;
            player.transform.position = startPos;
            var cnS=Instantiate(cmSwitchHeight, Vector3.zero, Quaternion.identity);
            cmSwitchHeight = cnS;
            cmSwitchHeight.player = player.gameObject;
            var cmV= Instantiate(cmVcam, Vector3.zero, Quaternion.identity);
            cmVcam = cmV;
            cmVcam.pivot = cmSwitchHeight.transform;
            player.mainCamera = Camera.main;
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
            if (id is 0 or 10 or 2000000001) return;
            if (objsDic.TryGetValue(id, out Block obj))
            {
                Vector3Int v = ChessboardSys.Instance.positions[id];
                ChessboardSys.Instance.matrix[v.x, v.y, v.z] = 0;
                ChessboardSys.Instance.positions.Remove(id);
                objsDic.Remove(id);
                Destroy(obj.gameObject); // 销毁GameObject
                // 播放销毁特效
                GenerateNewEffect(fx_eliminate,v);
            }
            else
            {
                Debug.LogWarning($"没有在objsDic中找到ID[{id}]");
            }
        }

        public void GenerateNewEffect(GameObject fx,Vector3 v)
        {
            if (fx == null) return;
            // 获取摄像机的方向
            if (Camera.main != null)
            {
                Vector3 cameraDirection = Camera.main.transform.position - v;
                cameraDirection.y = 0; // 保持X轴对齐，不让特效上下倾斜
                // 创建一个旋转，朝向摄像机
                Quaternion lookRotation = Quaternion.LookRotation(cameraDirection);
                // 在方块的位置实例化特效Prefab
                GameObject effect = Instantiate(fx, v,lookRotation);
                StartCoroutine(DestroyEffectAfterTime(effect, 3f));
            }
        }

        private IEnumerator DestroyEffectAfterTime(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            //DestroyImmediate (effect, true);
            Destroy(effect);
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
                gridSize.y = matrixSize.z;
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
        /// 检查目标位置在地图范围内
        /// </summary>
        /// <param name="posInt"></param>
        /// <returns></returns>
        public bool CheckInRange(Vector3Int posInt)
        {
            return posInt.x >= 0 && posInt.x < matrixSize.x &&
                   posInt.y >= 0 && posInt.y < matrixSize.y &&
                   posInt.z >= 0 && posInt.z < matrixSize.z;
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