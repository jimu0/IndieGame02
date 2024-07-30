using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace File_jim.Script
{
    public class BoxMovManager : MonoBehaviour
    {
        private bool onTheMove; //
        private bool standby; //
        private bool stopCoroutine;
        
        //public Vector3 targetPosition;

        public static float MoveDuration = 0.1f;
        public static event Action<int, int, int, float> OnMoveBoxesToTarget;
        public static event Action<Vector3Int, int, int, int, float> OnMoveBoxToTarget;
        public static event Action<int, int, int, int, float> OnMoveBoxIdToTarget;
        private Queue<(BoxMovement, Vector3Int, Vector3Int)> updateQueue = new();

        public GameObject boxPrefab;
        private int nextBoxId = 1;
        public Dictionary<int, GameObject> idToObj = new();

        private void OnEnable()
        {
            BoxMovement.OnPositionChanged += QueueUpdate;
        }

        private void OnDisable()
        {
            BoxMovement.OnPositionChanged -= QueueUpdate;
            stopCoroutine = false;
        }

        private void QueueUpdate(BoxMovement box, Vector3Int oldPos, Vector3Int newPos)
        {
            updateQueue.Enqueue((box, oldPos, newPos));
        }

        public static void MoveAllBoxesToTarget(int x, int y, int z)
        {
            OnMoveBoxesToTarget?.Invoke(x, y, z, MoveDuration);
        }

        public static void MoveBoxToTarget(Vector3Int pos, int x, int y, int z)
        {
            OnMoveBoxToTarget?.Invoke(pos, x, y, z, MoveDuration);
        }

        public static void MoveBoxIdToTarget(int id, int x, int y, int z)
        {
            OnMoveBoxIdToTarget?.Invoke(id, x, y, z, MoveDuration);
        }

        private void Start()
        {
            StartCoroutine(CallMethodEverySecond());
        }

        private void Update()
        {
            if (updateQueue.Count > 0)
            {
                (BoxMovement box, Vector3Int oldPos, Vector3Int newPos) = updateQueue.Dequeue();
                StartCoroutine(UpdateDatabaseAsync(box, oldPos, newPos));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                //MoveAllBoxesToTarget(Vector3Int.down);
                GenerateNewBox();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                //MoveAllBoxesToTarget(Vector3Int.down);
                for (int z = 0; z < Chessboard.matrix.GetLength(2); z++)
                {
                    for (int x = 0; x < Chessboard.matrix.GetLength(0); x++)
                    {
                        DestroyObj(Chessboard.GetMatrixValue(x, 0, z));
                        Chessboard.SetMatrixValue(x, 0, z, 0);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                idToObj[2].GetComponent<BoxMovement>().PushTo(Vector3Int.left, 0.2f);
            }
        }

        private IEnumerator UpdateDatabaseAsync(BoxMovement box, Vector3Int oldPos, Vector3Int newPos)
        {
            // ģ���첽����
            yield return Task.Run(() =>
            {
                // ������ʵ�ʵ����ݿ�����߼�
                // ���� UpdateDatabase ��һ��ͬ������
                UpdateDatabase(box, oldPos, newPos);
            });
        }

        private void UpdateDatabase(BoxMovement box, Vector3Int oldPos, Vector3Int newPos)
        {
            //���ݿ����
            SetMatrixV(newPos, box.id, out bool f);
            if (f) SetMatrixV(oldPos, 0);
        }

        private IEnumerator CallMethodEverySecond()
        {
            while (!stopCoroutine)
            {
                Metronome();
                Elimination();
                yield return new WaitForSeconds(MoveDuration);
            }
        }

        public static void Metronome()
        {
            // �ӵײ㵽�����������ֹ����
            for (int y = 1; y < Chessboard.matrix.GetLength(1); y++)
            {
                for (int x = 0; x < Chessboard.matrix.GetLength(0); x++)
                {
                    for (int z = 0; z < Chessboard.matrix.GetLength(2); z++)
                    {
                        // ��鵱ǰ�����Ƿ�������
                        if (Chessboard.matrix[x, y, z] != 0)
                        {
                            // �������һ���Ƿ�Ϊ��
                            if (Chessboard.matrix[x, y - 1, z] == 0)
                            {
                                // ����������һ��
                                int boxId = Chessboard.matrix[x, y, z];
                                Chessboard.matrix[x, y - 1, z] = boxId;
                                Chessboard.matrix[x, y, z] = 0;

                                // ����ʵ������Ԥ����λ��
                                //var targetPos = new Vector3Int(x, y - 1, z);
                                MoveBoxIdToTarget(boxId, x, y - 1, z);
                                //GameObject box = GameObject.Find("Box_" + boxId);
                                //if (box != null)
                                //{
                                //box.transform.position = new Vector3(x, y - 1, z);

                                //}
                            }
                        }
                    }
                }
            }
        }

        private void Elimination()
        {
            for (int y = 0; y < Chessboard.matrix.GetLength(1); y++)
            {
                EliminationY(y, out bool le);
                if (!le) continue;
                for (int z = 0; z < Chessboard.matrix.GetLength(2); z++)
                {
                    for (int x = 0; x < Chessboard.matrix.GetLength(0); x++)
                    {
                        DestroyObj(Chessboard.GetMatrixValue(x, y, z));
                        Chessboard.matrix[x, y, z] = 0;
                    }
                }
            }
        }

        private void EliminationY(int y, out bool el)
        {
            el = false;
            for (int z = 0; z < Chessboard.matrix.GetLength(2); z++)
            {
                for (int x = 0; x < Chessboard.matrix.GetLength(0); x++)
                {
                    if (Chessboard.matrix[x, y, z] == 0) return;
                }
            }

            el = true;
        }


        public void GenerateNewBox()
        {
            int randomValueX = Random.Range(0, Chessboard.matrix.GetLength(0));
            int randomValueZ = Random.Range(0, Chessboard.matrix.GetLength(2));
            Vector3Int posInt = new(randomValueX, Chessboard.matrix.GetLength(1) - 1, randomValueZ);
            if (Chessboard.GetMatrixValue(posInt.x, posInt.y, posInt.z) == 0)
            {
                int newBoxId = nextBoxId++; //����һ��ID
                Chessboard.SetMatrixValue(posInt.x, posInt.y, posInt.z, newBoxId);
                GameObject newBox = Instantiate(boxPrefab, posInt, Quaternion.identity);
                newBox.name = "Box_" + newBoxId;
                newBox.GetComponent<BoxMovement>().id = newBoxId;
                idToObj.Add(newBoxId, newBox);
            }
        }

        public static void FallBoxV(Vector3Int pos, Vector3Int direction, out bool b)
        {
            var boxId = GetMatrixV(pos);
            if (GetMatrixV(pos + direction) == 0)
            {
                SetMatrixV(pos + direction, boxId, out bool f);
                if (f) SetMatrixV(pos, 0);
                b = f;
            }
            else
            {
                b = false;
            }
        }


        public static void SetMatrixV(Vector3Int v, int i)
        {
            Chessboard.SetMatrixValue(v.x, v.y, v.z, i);
        }

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


        public static int GetMatrixV(Vector3Int v)
        {
            return Chessboard.GetMatrixValue(v.x, v.y, v.z);
        }

        // ����ID�Ƴ�BoxObj
        public void RemoveBoxObj(int id)
        {
            if (idToObj.ContainsKey(id))
            {
                idToObj.Remove(id);
            }
            else
            {
                Debug.LogWarning($"ID {id} ���������ֵ��С�");
            }
        }

        // ����BoxObj�����ֵ����Ƴ�
        public void DestroyObj(int id)
        {
            if (idToObj.TryGetValue(id, out GameObject boxObj))
            {
                Destroy(boxObj); // ����GameObject
                idToObj.Remove(id); // ���ֵ����Ƴ�
            }
            else
            {
                Debug.LogWarning($"ID {id} ���������ֵ��С�");
            }
        }
    }
}