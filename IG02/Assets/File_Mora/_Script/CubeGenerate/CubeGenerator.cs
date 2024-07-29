using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class CubeGenerator : MonoBehaviour
    {
        private int index;
        [Header("��������")]
        public int TotalCount = 10;
        [Header("���ӡ���Ҫ�༭")]
        [SerializeField] private int Seed;
        [Header("����ķ���Ԥ����")]
        public List<GameObject> Blocks = new();
        [Header("���ɵȴ�ʱ��")]
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
