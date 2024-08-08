using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using File_jim.Scripts.ObjectPool;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace File_jim.Script
{
    public class Chessboard : MonoBehaviour
    {
        private Vector3Int matrixSize;//关卡尺寸(读取mapData得到)
        public bool stopCoroutine;//������������������״̬
        private const float Pulse = 0.1f;//�����������˶�Ƶ��
        public static event Action<float> OnMoveBoxesToTarget;//�ƶ��¼�
        public GameObject boxPrefab;//boxԤ��
        private int nextBoxId = 1;//����boxʵ������ʼid
        public int uniqueId = 0;//����Box��������ʱ������id
        public readonly Dictionary<int, GameObject> objsDic = new();//��ͨ��id��ѯboxʵ�����ֵ�
        private static Vector3Int tempPos = Vector3Int.zero;//��ʱ������ת����
        private DataManager dataManager;//data���ݴ���
        [SerializeField] private string flePath = "/dataTable/Load/";
        [SerializeField] private string mapDataFileName = "mapData";
        [SerializeField] private string mapTileFileName = "mapTile";
        public GameObject grid;
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
            SetGrid();
            StartCoroutine(CallMethodEverySecond());
        }
        

        private void Update()
        {
            //���ԣ���������
            if (Input.GetKeyDown(KeyCode.C))
            {
                //GenerateNewBox_random();
            }
            //���ף�һ��������ײ�
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
            Debug.Log("pool:��ȡ");
            //gameObj.SetActive(false);
        }
        void OnRelease(GameObject gameObj)
        {
            Debug.Log("pool:�ͷ�");
        }
        void OnDestory(GameObject gameObj)
        {
            Debug.Log("pool:����");
        }
        
        /// <summary>
        /// ֪ͨ�ƶ�
        /// </summary>
        private static void MoveAllBoxesToTarget(float pulse)
        {
            OnMoveBoxesToTarget?.Invoke(pulse);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        private IEnumerator CallMethodEverySecond()
        {
            while (true)
            {
                if (!stopCoroutine)
                {
                    //�����ж�
                    Elimination();
                    //��������
                    Metronome();

                    
                }
                //ˢ��Ƶ��
                yield return new WaitForSeconds(Pulse);
            }
        }

        /// <summary>
        /// ��������-��׹�߼�
        /// </summary>
        private void Metronome()
        {
            //�ӵײ㵽�����������ֹ����
            for (int y = 1; y < ChessboardSys.Instance.matrix.GetLength(1); y++)
            {
                for (int x = 0; x < ChessboardSys.Instance.matrix.GetLength(0); x++)
                {
                    for (int z = 0; z < ChessboardSys.Instance.matrix.GetLength(2); z++)
                    {
                        //��鵱ǰ�����Ƿ�������
                        if (ChessboardSys.Instance.matrix[x, y, z] != 0)
                        {
                            //�������һ���Ƿ�Ϊ��
                            if (ChessboardSys.Instance.matrix[x, y - 1, z] == 0)
                            {
                                //����������һ��
                                int boxId = ChessboardSys.Instance.matrix[x, y, z];
                                ChessboardSys.Instance.matrix[x, y - 1, z] = boxId;
                                ChessboardSys.Instance.matrix[x, y, z] = 0;
                                tempPos.x = x;
                                tempPos.y = y-1;
                                tempPos.z = z;
                                ChessboardSys.Instance.positions[boxId] = tempPos;//����λ���ֵ�
                            }
                        }
                    }
                }
            }
            MoveAllBoxesToTarget(Pulse);//֪ͨ�ƶ�
        }

        /// <summary>
        /// ��������-������
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
        /// ��������-ĳ����
        /// </summary>
        /// <param name="y">�ڼ���</param>
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
        /// ����һ��box_���(Ŀǰ��Id��ͻ����)
        /// </summary>
        public void GenerateNewBox_random()
        {
            int randomValueX = Random.Range(0, ChessboardSys.Instance.matrix.GetLength(0));
            int randomValueZ = Random.Range(0, ChessboardSys.Instance.matrix.GetLength(2));
            Vector3Int posInt = new(randomValueX, ChessboardSys.Instance.matrix.GetLength(1) - 1, randomValueZ);
            if (ChessboardSys.Instance.GetMatrixValue(posInt.x, posInt.y, posInt.z) == 0)
            {
                int newBoxId = nextBoxId++; //����һ��ID
                SetMatrixV(posInt, newBoxId);
                GameObject newBox = Instantiate(boxPrefab, posInt, Quaternion.identity);
                newBox.name = "Box_" + newBoxId;
                newBox.GetComponent<BoxMovement>().id = newBoxId;
                //ʵ���ֵ�
                if (objsDic.ContainsKey(newBoxId))
                {
                    objsDic[newBoxId] = newBox;
                }
                else
                {
                    objsDic.Add(newBoxId, newBox);
                }
                //λ���ֵ�
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
        /// ����һ��box_ָ��
        /// </summary>
        public void GenerateNewBox(Vector3Int posInt,int id)
        {
            GameObject newBox = Instantiate(boxPrefab, posInt, Quaternion.identity);
            //GameObject newBox = boxPool.Get();
            newBox.name = "Box_" + id;
            newBox.GetComponent<BoxMovement>().id = id;
            newBox.transform.position = posInt;
            //ʵ���ֵ�
            if (objsDic.ContainsKey(id))
            {
                objsDic[id] = newBox;
            }
            else
            {
                objsDic.Add(id, newBox);
            }
            //λ���ֵ�
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
        /// ����Ϊ-���
        /// </summary>
        /// <param name="pos">ԭλ��</param>
        /// <param name="direction">�ƶ�����</param>
        /// <param name="b">������</param>
        public static void FallBoxV(Vector3Int pos, Vector3Int direction, out bool b)
        {
            int boxId = GetMatrixV(pos);
            SetMatrixV(pos + direction, boxId, out bool f);
            if (f) SetMatrixV(pos, 0);
            b = f;
        }

        /// <summary>
        /// ���þ�������
        /// </summary>
        /// <param name="v">λ��</param>
        /// <param name="i">id</param>
        private static void SetMatrixV(Vector3Int v, int i)
        {
            ChessboardSys.Instance.SetMatrixValue(v.x, v.y, v.z, i);
        }
        /// <summary>
        /// ���þ�������-����ǲ��ǿ�λ
        /// </summary>
        /// <param name="v">λ��</param>
        /// <param name="i">id</param>
        /// <param name="f">������</param>
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
        /// ��ȡ��������
        /// </summary>
        /// <param name="v">λ��</param>
        /// <returns>id</returns>
        public static int GetMatrixV(Vector3Int v)
        {
            return ChessboardSys.Instance.GetMatrixValue(v.x, v.y, v.z);
        }
        /// <summary>
        /// ��ȡλ���ֵ�
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>λ��</returns>
        public static Vector3Int GetMatrixP(int id)
        {
            return ChessboardSys.Instance.GetMatrixPos(id);
        }

        /// <summary>
        /// ����ID�Ƴ�ʵ���ֵ��λ���ֵ��е���Ϣ
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
                Debug.LogWarning($"ID {id} ���������ֵ��С�");
            }
        }

        /// <summary>
        /// ����ID����ʵ�����Ƴ�ʵ���ֵ��λ���ֵ��е���Ϣ
        /// </summary>
        /// <param name="id"></param>
        public void DestroyObj(int id)
        {
            if (objsDic.TryGetValue(id, out GameObject obj))
            {
                Destroy(obj); // ����GameObject
                objsDic.Remove(id);
                ChessboardSys.Instance.positions.Remove(id);
            }
            else
            {
                Debug.LogWarning($"ID {id} ���������ֵ��С�");
            }
        }

        /// <summary>
        /// ��ʼ����ͼbox
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
        /// ����դ��
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
                Debug.LogError("GridRoot��û���ҵ���ΪGridMesh��������");
            }
        }

        /// <summary>
        /// ���ƾ������ݿ��ӻ�
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
                            // ���� Gizmos ����ɫ
                            Gizmos.color = Color.red;
                            // ����һ�����壬��ʾ����λ��
                            Gizmos.DrawSphere(new Vector3(x, y, z), 0.05f);
                        }
                    }
                }
            }
        }
    }
}