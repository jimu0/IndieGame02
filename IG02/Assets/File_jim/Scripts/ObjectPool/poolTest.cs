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
            return new GameObject("wokao");
        }
        void OnGet(GameObject gameObject)
        {
            Debug.Log("Onget");
        }
        void OnRelease(GameObject gameObject)
        {
            Debug.Log("OnRelease");
        }
        void OnDestory(GameObject gameObject)
        {
            Debug.Log("OnDestory");
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
