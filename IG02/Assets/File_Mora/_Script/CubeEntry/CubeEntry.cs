
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Cube
{
    public class CubeEntry : MonoBehaviour
    {
        private int index = 0;
        private List<Vector3> poss = new();
        public float LenthOfLine = 0.06f;
        // Start is called before the first frame update
        void Start()
        {
            index = 0;
            gameObject.name += " * " + Time.time.ToString();
            poss = new();
            for (int i = 0; i < 300; i++)
            {
                poss.Add(Vector3.zero);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (index < 299)
            {
                index++;
                if(index >= 1)
                {
                    if(DataPool.instance.CheckIfInList(Round(transform.position.x),
                Round(transform.position.y),
                Round(transform.position.z)))
                    DataPool.instance.ReSetInList(Round(poss[index - 1].x),
                        Round(poss[index - 1].y),
                        Round(poss[index - 1].z));
                }
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


            if (isTouchedCube())
            {
                transform.Translate(Vector3.down * 3f * Time.deltaTime);
            }
            //foreach (Vector3 v in poss)
            //{
            //    if (v != new Vector3(Round(transform.position.x),
            //    Round(transform.position.y),
            //    Round(transform.position.z)))
            //    {
            //        DataPool.instance.ReSetInList(Round(v.x),
            //            Round(v.y),
            //            Round(v.z));
            //    }
            //}
        }

        bool isTouchedCube()
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - LenthOfLine, transform.position.z));
        }
    }
}
