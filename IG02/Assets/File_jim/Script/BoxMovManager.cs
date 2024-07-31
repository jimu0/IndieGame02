using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace File_jim.Script
{
    public class BoxMovManager : MonoBehaviour
    {
        private bool stopCoroutine;//控制主驱动器的运行状态
        private const float Speed = 0.1f;//主驱动器的运动平率
        public static event Action<float> OnMoveBoxesToTarget;//移动事件
        public GameObject boxPrefab;//box预制
        private int nextBoxId = 1;//生成box实例的起始id
        private readonly Dictionary<int, GameObject> objsDic = new();//可通过id查询box实例的字典
        private static Vector3Int tempPos = Vector3Int.zero;//临时参数中转变量
        
        /// <summary>
        /// 通知移动
        /// </summary>
        private static void MoveAllBoxesToTarget(float speed)
        {
            OnMoveBoxesToTarget?.Invoke(speed);
        }
        

        private void Start()
        {
            StartCoroutine(CallMethodEverySecond());
        }

        private void Update()
        {
            //测试：方块生成
            if (Input.GetKeyDown(KeyCode.C))
            {
                GenerateNewBox();
            }
            //作弊：一键消除最底层
            if (Input.GetKeyDown(KeyCode.X))
            {
                for (int z = 0; z < Chessboard.Matrix.GetLength(2); z++)
                {
                    for (int x = 0; x < Chessboard.Matrix.GetLength(0); x++)
                    {
                        DestroyObj(Chessboard.GetMatrixValue(x, 0, z));
                        Chessboard.SetMatrixValue(x, 0, z, 0);
                    }
                }
            }

        }

        /// <summary>
        /// 主驱动器
        /// </summary>
        /// <returns></returns>
        private IEnumerator CallMethodEverySecond()
        {
            while (!stopCoroutine)
            {
                //消除判断
                Elimination();
                //更新数据
                Metronome();
                //刷新频率
                yield return new WaitForSeconds(Speed);
            }
        }

        /// <summary>
        /// 更新数据-下坠逻辑
        /// </summary>
        private static void Metronome()
        {
            //从底层到顶层遍历，防止覆盖
            for (int y = 1; y < Chessboard.Matrix.GetLength(1); y++)
            {
                for (int x = 0; x < Chessboard.Matrix.GetLength(0); x++)
                {
                    for (int z = 0; z < Chessboard.Matrix.GetLength(2); z++)
                    {
                        //检查当前格子是否有箱子
                        if (Chessboard.Matrix[x, y, z] != 0)
                        {
                            //检查下面一层是否为空
                            if (Chessboard.Matrix[x, y - 1, z] == 0)
                            {
                                //将箱子下移一格
                                int boxId = Chessboard.Matrix[x, y, z];
                                Chessboard.Matrix[x, y - 1, z] = boxId;
                                Chessboard.Matrix[x, y, z] = 0;
                                tempPos.x = x;
                                tempPos.y = y-1;
                                tempPos.z = z;
                                Chessboard.Positions[boxId] = tempPos;//更新位置字典
                            }
                        }
                    }
                }
            }
            MoveAllBoxesToTarget(Speed);//通知移动
        }

        /// <summary>
        /// 消除规则-所有面
        /// </summary>
        private void Elimination()
        {
            for (int y = 0; y < Chessboard.Matrix.GetLength(1); y++)
            {
                EliminationY(y, out bool le);
                if (!le) continue;
                for (int z = 0; z < Chessboard.Matrix.GetLength(2); z++)
                {
                    for (int x = 0; x < Chessboard.Matrix.GetLength(0); x++)
                    {
                        DestroyObj(Chessboard.GetMatrixValue(x, y, z));
                        Chessboard.Matrix[x, y, z] = 0;
                    }
                }
            }
        }
        /// <summary>
        /// 消除规则-某层面
        /// </summary>
        /// <param name="y">第几层</param>
        /// <param name="el">满足？</param>
        private void EliminationY(int y, out bool el)
        {
            el = false;
            for (int z = 0; z < Chessboard.Matrix.GetLength(2); z++)
            {
                for (int x = 0; x < Chessboard.Matrix.GetLength(0); x++)
                {
                    if (Chessboard.Matrix[x, y, z] == 0) return;
                }
            }
            el = true;
        }

        /// <summary>
        /// 生成一个box
        /// </summary>
        public void GenerateNewBox()
        {
            int randomValueX = Random.Range(0, Chessboard.Matrix.GetLength(0));
            int randomValueZ = Random.Range(0, Chessboard.Matrix.GetLength(2));
            Vector3Int posInt = new(randomValueX, Chessboard.Matrix.GetLength(1) - 1, randomValueZ);
            if (Chessboard.GetMatrixValue(posInt.x, posInt.y, posInt.z) == 0)
            {
                int newBoxId = nextBoxId++; //生成一个ID
                Chessboard.SetMatrixValue(posInt.x, posInt.y, posInt.z, newBoxId);
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
                if (Chessboard.Positions.ContainsKey(newBoxId))
                {
                    Chessboard.Positions[newBoxId]=posInt;
                }
                else
                {
                    Chessboard.Positions.Add(newBoxId, posInt);
                }

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
            if (f) SetMatrixV(pos, 0);
            b = f;
        }

        /// <summary>
        /// 设置矩阵数据
        /// </summary>
        /// <param name="v">位置</param>
        /// <param name="i">id</param>
        public static void SetMatrixV(Vector3Int v, int i)
        {
            Chessboard.SetMatrixValue(v.x, v.y, v.z, i);
        }
        /// <summary>
        /// 设置矩阵数据-检查
        /// </summary>
        /// <param name="v">位置</param>
        /// <param name="i">id</param>
        /// <param name="f">允许？</param>
        public static void SetMatrixV(Vector3Int v, int i, out bool f)
        {
            if (Chessboard.GetMatrixValue(v.x, v.y, v.z) == 0)
            {
                Chessboard.SetMatrixValue(v.x, v.y, v.z, i);
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
            return Chessboard.GetMatrixValue(v.x, v.y, v.z);
        }
        /// <summary>
        /// 获取位置字典
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>位置</returns>
        public static Vector3Int GetMatrixP(int id)
        {
            return Chessboard.GetMatrixPos(id);
        }

        /// <summary>
        /// 根据ID移除BoxObj
        /// </summary>
        /// <param name="id">id</param>
        public void RemoveBoxObj(int id)
        {
            if (objsDic.ContainsKey(id))
            {
                objsDic.Remove(id);
                Chessboard.Positions.Remove(id);
            }
            else
            {
                Debug.LogWarning($"ID {id} 不存在于字典中。");
            }
        }

        /// <summary>
        /// 销毁BoxObj并从字典中移除
        /// </summary>
        /// <param name="id"></param>
        public void DestroyObj(int id)
        {
            
            if (objsDic.TryGetValue(id, out GameObject obj))
            {
                Destroy(obj); // 销毁GameObject
                objsDic.Remove(id);
                Chessboard.Positions.Remove(id);
            }
            else
            {
                Debug.LogWarning($"ID {id} 不存在于字典中。");
            }
        }
    }
}