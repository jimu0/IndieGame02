using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cube
{
    public class DataPool : MonoBehaviour
    {
        public int MapStartXInt = 20;
        public int MapStartZInt = 20;
        [Range(3, 99)]
        [SerializeField] private int MaxBombSaveCount = 5;
        private int[,,] PosList;
        public static DataPool instance;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            //var cont = 0;
            //for (int i = 0; i < 99; i++)
            //{
            //    for (int j = 0; j < 99; j++)
            //    {
            //        for (int k = 0; k < 99; k++)
            //        {
            //            if (PosList[i, j, k] == 1)
            //            {
            //                cont++;
            //            }
            //        }
            //    }
            //}
            //Debug.Log("----*** " + cont);
        }

        public List<Vector3> CheckIfReadyToBomb(int y)
        {
            //int xCout = 0;
            //List<Vector3> xList = new List<Vector3>();
            //for (int i = x - MaxBombSaveCount + 1; i < x + MaxBombSaveCount; i++)
            //{
            //    if (PosList[i, y, z] == 1)
            //    {
            //        xCout++;
            //        xList.Add(new Vector3(i, y, z));
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}


            //if (xList.Count < MaxBombSaveCount)
            //{
            //    xList.Clear();
            //}
            //else
            //{
            //    return xList;
            //}


            //int yCout = 0;
            //List<Vector3> yList = new List<Vector3>();

            //for (int j = y - MaxBombSaveCount + 1; j < y + MaxBombSaveCount; j++)
            //{

            //    if (PosList[x, j, z] == 1)
            //    {
            //        yCout++;
            //        yList.Add(new Vector3(x, j, z));
            //    }
            //    else
            //    {
            //        break;
            //    }


            //}
            //if (yList.Count < MaxBombSaveCount)
            //{
            //    yList.Clear();
            //}
            //else
            //{
            //    return yList;
            //}
            List<Vector3> resList = new List<Vector3>();

            for (int i = MapStartXInt; i < MapStartXInt + MaxBombSaveCount; i++)
            {
                for (int j = MapStartZInt; j < MapStartZInt + MaxBombSaveCount; j++)
                {
                    if (PosList[i,y,j] == 1)
                    {
                        resList.Add(new Vector3(i, y, j));
                    }
                }
            }
            Debug.Log(resList.Count);
            if (resList.Count < MaxBombSaveCount * MaxBombSaveCount)
            {
                return new List<Vector3>();
            }
            else
            {
                return resList;
            }

        }


        public bool CheckIfInList(int x, int y, int z)
        {
            if (PosList[x, y, z] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void ReSetInList(int x, int y, int z)
        {
            PosList[x, y, z] = 0;
        }

        public void SetInList(int x, int y, int z)
        {
            PosList[x, y, z] = 1;
        }

        private void Init()
        {
            PosList = new int[99, 99, 99];
            for (int i = 0; i < 99; i++)
            {
                for (int j = 0; j < 99; j++)
                {
                    for (int s = 0; s < 99; s++)
                    {
                        PosList[i, j, s] = 0;
                    }
                }
            }
        }
    }
}
