using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class CubeGenerator : MonoBehaviour
    {
        private int index;
        [Header("下落总数")]
        public int TotalCount = 10;
        [Header("种子。不要编辑")]
        [SerializeField] private int Seed;
        [Header("下落的方块预制体")]
        public List<GameObject> Blocks = new();
        [Header("生成等待时间")]
        public float MaxWaitTime = 1;
        float timer;

        // Start is called before the first frame update
        void Start()
        {
            Seed = Random.Range(100000, 999999);
            Random.InitState(Seed);
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (timer > MaxWaitTime && index < TotalCount)
            {
                index++;
                timer = 0;
                var go = Instantiate(Blocks[Random.Range(0, Blocks.Count - 1)], new Vector3((int)Random.Range(transform.position.x, transform.position.x + 8)
                    , (int)Random.Range(transform.position.y, transform.position.y + 8), (int)transform.position.z), Quaternion.identity);
            }
        }
    }
}
