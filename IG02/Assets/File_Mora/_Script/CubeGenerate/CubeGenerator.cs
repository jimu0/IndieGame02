using Cube;
using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generator
{
    public class CubeGenerator : MonoBehaviour
    {
        public static CubeGenerator instance;
        [ReadOnly]
        public List<CubeEntry> entries = new List<CubeEntry>();
        public int Width = 30;
        public int Height = 30;
        [Space(10)]
        [SerializeField] private float fallenSpeed = 6.5f;
        private int index;
        [Header("��������")]
        public int TotalCount = 10;
        [Header("����")]
        [ReadOnly]
        [SerializeField] private int Seed;
        [Header("����ķ���Ԥ����")]
        public List<GameObject> Blocks = new();
        [Header("���ɵȴ�ʱ��")]
        public float MaxWaitTime = 1;
        float timer;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            Seed = Random.Range(0, 9999999);
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
                var go = Instantiate(Blocks[Random.Range(0, Blocks.Count - 1)], new Vector3(Random.Range((int)transform.position.x, (int)transform.position.x + Width)
                    ,transform.position.y, Random.Range((int)transform.position.z, (int)transform.position.z + (int)Height)), Quaternion.identity);
                var lists = go.GetComponentsInChildren<CubeEntry>().ToList();
                entries.AddRange(lists);
                foreach (var item in lists)
                {
                    item.Speed = fallenSpeed;
                }
            }
        }

        public void SetDestroyEntrys(List<Vector3> target)
        {
            List<CubeEntry> tp = new();
            foreach (var entry in entries)
            {
                if(entry == null)
                    continue;

                if (target.Contains(new Vector3(Round(entry.transform.position.x),
                Round(entry.transform.position.y),
                Round(entry.transform.position.z))))
                {
                    tp.Add(entry);
                }
            }

            StartCoroutine(DestroyEntrys(tp));
        }

        private IEnumerator DestroyEntrys(List<CubeEntry> target)
        {
            foreach (var entry in target)
            {
                if (entry == null || entry.gameObject == null)
                {
                    continue;
                }
                entry.GetComponent<MeshRenderer>().material.color = Color.red;
                yield return new WaitForSeconds(0.3f);
                if (entry == null || entry.gameObject == null)
                {
                    continue;
                }
                entries.Remove(entries.Find(t => t.name == entry.gameObject.name));
                Destroy(entry.gameObject);
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// digits:������λС��
        public static int Round(float value, int digits = 0)
        {
            if (value == 0)
            {
                return 0;
            }
            float multiple = Mathf.Pow(10, digits);
            float tempValue = value > 0 ? value * multiple + 0.5f : value * multiple - 0.5f;
            tempValue = Mathf.FloorToInt(tempValue);
            return (int)(tempValue / multiple);
        }
    }
}
