using Add;
using Cube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerManagement
{
    public class AttachCube : MonoBehaviour
    {
        [SerializeField] private FloatVar angleWhenInPulling;
        [EditorPlus.ReadOnly] [SerializeField] bool isIn;
        [EditorPlus.ReadOnly] [SerializeField] GameObject cube;

        // Start is called before the first frame update
        void Start()
        {
            angleWhenInPulling.Value = -1;
            cube = null;
        }

        // Update is called once per frame
        void Update()
        {
            if(isIn && cube)
            {
                angleWhenInPulling.Value = transform.parent.eulerAngles.y;
                cube.transform.position = transform.position;
            }
            else
            {
                angleWhenInPulling.Value = -1;
            }

            if (Input.GetKey(KeyCode.E))
            {
                GetComponent<Collider>().enabled = true;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                GetComponent<Collider>().enabled = false;
            }

            if (Input.GetKey(KeyCode.E) && cube && !isIn)
            {
                isIn = true; 
                cube.GetComponent<Collider>().isTrigger = false;
                cube.GetComponent<Collider>().enabled = false;
                cube.layer = LayerMask.NameToLayer("Default");
            }
            if(Input.GetKey(KeyCode.Q) && cube && isIn && DataPool.instance.CheckIfInList(Round(cube.transform.position.x), Round(cube.transform.position.y + 0.35f), Round(cube.transform.position.z)) == false)
            {
                isIn = false;
                cube.GetComponent<Collider>().enabled = true;
                cube.layer = LayerMask.NameToLayer("Cube");
                cube.transform.position = new Vector3(Round(cube.transform.position.x), Round(cube.transform.position.y + 0.35f), Round(cube.transform.position.z));
                cube.GetComponent<Collider>().isTrigger = true;
                //cubes.Clear();
                cube = null;
            }
            //if (cube && cube.GetComponent<CubeEntry>().isColliderIn)
            //{
            //    cube.GetComponent<Collider>().isTrigger = false;
            //    cube.GetComponent<Collider>().enabled = false;
            //    cube.transform.position = transform.position;
            //}
            //else if (cube)
            //{
            //    cube.GetComponent<Collider>().isTrigger = false;
            //    cube.transform.position = new Vector3((int)cube.transform.position.x, (int)cube.transform.position.y, (int)cube.transform.position.z);
            //}
        }

        public void Ebtn()
        {
            isIn = true;
            cube.GetComponent<Collider>().isTrigger = false;
            cube.GetComponent<Collider>().enabled = false;
            cube.layer = LayerMask.NameToLayer("Default");
        }

        public void Qbtn()
        {
            isIn = false;
            cube.GetComponent<Collider>().enabled = true;
            cube.layer = LayerMask.NameToLayer("Cube");
            cube.transform.position = new Vector3(Round(cube.transform.position.x), Round(cube.transform.position.y + 0.35f), Round(cube.transform.position.z));
            cube.GetComponent<Collider>().isTrigger = true;
            //cubes.Clear();
            cube = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Cube"))
            if (other.gameObject.layer != LayerMask.NameToLayer("Cube") || cube != null)
                return;
            cube = other.gameObject;
            

        }
        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject == cube)
            {
                cube = null;
            }
            //canPlace = true;
            //if (other.gameObject.layer == LayerMask.NameToLayer("Cube"))
            //{
            //    cubes.Remove(cubes.Find(t => t.name == other.gameObject.name));
            //}
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
    }
}
