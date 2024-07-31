using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace File_jim.Script
{
    public class BoxMovManager : MonoBehaviour
    {
        private bool stopCoroutine;//������������������״̬
        private const float Speed = 0.1f;//�����������˶�ƽ��
        public static event Action<float> OnMoveBoxesToTarget;//�ƶ��¼�
        public GameObject boxPrefab;//boxԤ��
        private int nextBoxId = 1;//����boxʵ������ʼid
        private readonly Dictionary<int, GameObject> objsDic = new();//��ͨ��id��ѯboxʵ�����ֵ�
        private static Vector3Int tempPos = Vector3Int.zero;//��ʱ������ת����
        
        /// <summary>
        /// ֪ͨ�ƶ�
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
            //���ԣ���������
            if (Input.GetKeyDown(KeyCode.C))
            {
                GenerateNewBox();
            }
            //���ף�һ��������ײ�
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
        /// ��������
        /// </summary>
        /// <returns></returns>
        private IEnumerator CallMethodEverySecond()
        {
            while (!stopCoroutine)
            {
                //�����ж�
                Elimination();
                //��������
                Metronome();
                //ˢ��Ƶ��
                yield return new WaitForSeconds(Speed);
            }
        }

        /// <summary>
        /// ��������-��׹�߼�
        /// </summary>
        private static void Metronome()
        {
            //�ӵײ㵽�����������ֹ����
            for (int y = 1; y < Chessboard.Matrix.GetLength(1); y++)
            {
                for (int x = 0; x < Chessboard.Matrix.GetLength(0); x++)
                {
                    for (int z = 0; z < Chessboard.Matrix.GetLength(2); z++)
                    {
                        //��鵱ǰ�����Ƿ�������
                        if (Chessboard.Matrix[x, y, z] != 0)
                        {
                            //�������һ���Ƿ�Ϊ��
                            if (Chessboard.Matrix[x, y - 1, z] == 0)
                            {
                                //����������һ��
                                int boxId = Chessboard.Matrix[x, y, z];
                                Chessboard.Matrix[x, y - 1, z] = boxId;
                                Chessboard.Matrix[x, y, z] = 0;
                                tempPos.x = x;
                                tempPos.y = y-1;
                                tempPos.z = z;
                                Chessboard.Positions[boxId] = tempPos;//����λ���ֵ�
                            }
                        }
                    }
                }
            }
            MoveAllBoxesToTarget(Speed);//֪ͨ�ƶ�
        }

        /// <summary>
        /// ��������-������
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
        /// ��������-ĳ����
        /// </summary>
        /// <param name="y">�ڼ���</param>
        /// <param name="el">���㣿</param>
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
        /// ����һ��box
        /// </summary>
        public void GenerateNewBox()
        {
            int randomValueX = Random.Range(0, Chessboard.Matrix.GetLength(0));
            int randomValueZ = Random.Range(0, Chessboard.Matrix.GetLength(2));
            Vector3Int posInt = new(randomValueX, Chessboard.Matrix.GetLength(1) - 1, randomValueZ);
            if (Chessboard.GetMatrixValue(posInt.x, posInt.y, posInt.z) == 0)
            {
                int newBoxId = nextBoxId++; //����һ��ID
                Chessboard.SetMatrixValue(posInt.x, posInt.y, posInt.z, newBoxId);
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
        /// ����Ϊ-���
        /// </summary>
        /// <param name="pos">ԭλ��</param>
        /// <param name="direction">�ƶ�����</param>
        /// <param name="b">����</param>
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
        public static void SetMatrixV(Vector3Int v, int i)
        {
            Chessboard.SetMatrixValue(v.x, v.y, v.z, i);
        }
        /// <summary>
        /// ���þ�������-���
        /// </summary>
        /// <param name="v">λ��</param>
        /// <param name="i">id</param>
        /// <param name="f">����</param>
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
        /// ��ȡ��������
        /// </summary>
        /// <param name="v">λ��</param>
        /// <returns>id</returns>
        public static int GetMatrixV(Vector3Int v)
        {
            return Chessboard.GetMatrixValue(v.x, v.y, v.z);
        }
        /// <summary>
        /// ��ȡλ���ֵ�
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>λ��</returns>
        public static Vector3Int GetMatrixP(int id)
        {
            return Chessboard.GetMatrixPos(id);
        }

        /// <summary>
        /// ����ID�Ƴ�BoxObj
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
                Debug.LogWarning($"ID {id} ���������ֵ��С�");
            }
        }

        /// <summary>
        /// ����BoxObj�����ֵ����Ƴ�
        /// </summary>
        /// <param name="id"></param>
        public void DestroyObj(int id)
        {
            
            if (objsDic.TryGetValue(id, out GameObject obj))
            {
                Destroy(obj); // ����GameObject
                objsDic.Remove(id);
                Chessboard.Positions.Remove(id);
            }
            else
            {
                Debug.LogWarning($"ID {id} ���������ֵ��С�");
            }
        }
    }
}