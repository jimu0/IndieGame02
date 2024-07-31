
using System.Collections.Generic;
using EditorPlus;
using UnityEngine;

namespace Cube
{
    public class CubeEntry : MonoBehaviour
    {
        public bool isDiableUpdate;
        [SerializeField] private float MaxFixTimer = 0.4f;
        [ReadOnly] public bool isMovingStatic;
        [EditorPlus.ReadOnly]
        public float Speed = 3f;
        private int index = 0;
        private List<Vector3> poss = new();
        public float LenthOfLine = 0.06f;
        private bool isTouch;

        // Start is called before the first frame update
        void Start()
        {
            index = 0;
            gameObject.name += " * " + Time.time.ToString();
            poss = new();
            for (int i = 0; i < 301; i++)
            {
                poss.Add(Vector3.zero);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(isDiableUpdate == false)
                UpdatePos();

            if (IsNotTouchedCube())
            {
                transform.Translate(Vector3.down * Speed * Time.deltaTime);
            }

            else if(isMovingStatic == false)
            {
                //transform.position = new Vector3(Round(transform.position.x), Round(transform.position.y), Round(transform.position.z));
                var res = DataPool.instance.CheckIfReadyToBomb(Round(transform.position.x),
                Round(transform.position.y),
                Round(transform.position.z));
                Debug.Log(res.Count);
                if(res.Count > 0)
                {
                    foreach (var item in res)
                    {
                        DataPool.instance.ReSetInList(Round(item.x),
                            Round(item.y),
                            Round(item.z));
                    }    
                    Generator.CubeGenerator.instance.SetDestroyEntrys(res);
                }
                isMovingStatic = true;
            }

            foreach (Vector3 v in poss)
            {
                if (v != new Vector3(Round(transform.position.x),
                Round(transform.position.y),
                Round(transform.position.z)))
                {
                    DataPool.instance.ReSetInList(Round(v.x),
                        Round(v.y),
                        Round(v.z));
                }
            }
        }

        private void OnDestroy()
        {
            foreach(var item in poss)
            {
                DataPool.instance.ReSetInList(Round(item.x),
                            Round(item.y),
                            Round(item.z));
            }
        }

        public void UpdatePos()
        {
            if (index < 300)
            {
                index++;
                poss[index] = new Vector3(Round(transform.position.x),
                Round(transform.position.y),
                Round(transform.position.z));
                DataPool.instance.SetInList(Round(transform.position.x),
                    Round(transform.position.y),
                    Round(transform.position.z));
            }
            else
            {
                index = 0;
            }
        }

        private bool IsNotTouchedCube()
        {
            //var ray1 = Physics.Linecast(transform.position, new Vector3(transform.position.x + LenthOfLine, transform.position.y, transform.position.z));
            //var ray2 = Physics.Linecast(transform.position, new Vector3(transform.position.x - LenthOfLine, transform.position.y, transform.position.z));
            //var ray3 = Physics.Linecast(transform.position, new Vector3(transform.position.x, transform.position.y + LenthOfLine, transform.position.z));
            var ray4 = Physics.Linecast(transform.position, new Vector3(transform.position.x, transform.position.y - LenthOfLine, transform.position.z), out var hit, (1 << LayerMask.NameToLayer("Cube")) | (1 << LayerMask.NameToLayer("Ground")));
            //var ray5 = Physics.Linecast(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + LenthOfLine));
            //var ray6 = Physics.Linecast(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - LenthOfLine));
            if (ray4 && hit.collider.gameObject != gameObject)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// digits:保留几位小数
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

        private void OnTriggerEnter(Collider other)
        {
            isTouch = true;
        }

        private void OnTriggerExit(Collider other)
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - LenthOfLine, transform.position.z));
        }
    }
}
