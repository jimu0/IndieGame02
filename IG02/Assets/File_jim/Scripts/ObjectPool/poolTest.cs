using UnityEngine;

namespace File_jim.Scripts.ObjectPool
{
    public class PoolTest : MonoBehaviour
    {
        private ObjectPool<GameObject> Pool;
        void Start()
        {
            Pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestory,
                true, 10, 1000);
        }
        GameObject OnCreate()
        {
            return new GameObject("woko");
        }
        void OnGet(GameObject gameObject)
        {
            Debug.Log("pool:获取");
        }
        void OnRelease(GameObject gameObject)
        {
            Debug.Log("pool:释放");
        }
        void OnDestory(GameObject gameObject)
        {
            Debug.Log("pool:销毁");
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pool.Get();
            }
        }
    }
}
