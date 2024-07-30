using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cube
{
    public class DataPool : MonoBehaviour
    {
        [SerializeField] private int[,,] PosList;
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
            PosList = new int[99,99,99];
            for (int i = 0; i < 99; i++)
            {
                for(int j = 0; j < 99; j++) 
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
