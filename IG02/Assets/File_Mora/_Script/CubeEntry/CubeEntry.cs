
using System.Collections.Generic;
using Add;
using AduioDesign;
using EditorPlus;
using UnityEngine;

namespace Cube
{
    public class CubeEntry : MonoBehaviour
    {
        [ReadOnly] public int ID;
        public bool DisableAudio;
        public bool DisbaleAutoFallen;
        [ReadOnly] public bool isBeHolding;
        private AudioPlayer aud;
        public BoolVar isPlayerMoving;
        public bool isDiableUpdate;
        [ReadOnly] public bool isMovingStatic = true;
        [EditorPlus.ReadOnly]
        public float Speed = 3f;
        private int index = 0;
        private List<Vector3> poss = new();
        public float LenthOfLine = 0.06f;
        private bool isGroundAudio;

        // Start is called before the first frame update
        void Start()
        {
            isGroundAudio = true;
            isBeHolding = false;
            aud = GetComponent<AudioPlayer>();
            index = 0;
            gameObject.name += " * " + ID;
            poss = new();
            for (int i = 0; i < 10; i++)
            {
                poss.Add(Vector3.zero);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.x < 0 || transform.position.y < 0 || transform.position.z < 0)
                return;

            if (DisableAudio == false)
            {
                if (isPlayerMoving.Value && isBeHolding)
                {
                    aud.PlayAudioClip(1);
                }
                else
                {
                    aud.StopPlayAudioClip();
                }
            }


            if (isDiableUpdate == false)
                UpdatePos();

            if (IsNotTouchedCube() && DisbaleAutoFallen == false)
            {
                transform.Translate(Vector3.down * Speed * Time.deltaTime);
            }

            else if (isMovingStatic == false && DisbaleAutoFallen == false)
            {
                CheckIfNeedDestroy();
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

        public void CheckIfNeedDestroy()
        {
            //if(isGround)
            //{
            //}
            isGroundAudio = true;
            //transform.position = new Vector3(Round(transform.position.x), Round(transform.position.y), Round(transform.position.z));
            var res = DataPool.instance.CheckIfReadyToBomb(
            Round(transform.position.y));
            Debug.Log(res.Count);
            if (res.Count > 0)
            {
                Generator.CubeGenerator.instance.SetDestroyEntrys(res);
            }
            isMovingStatic = true;
        }

        private void OnDestroy()
        {
            foreach (var item in poss)
            {
                DataPool.instance.ReSetInList(Round(item.x),
                            Round(item.y),
                            Round(item.z));
            }
        }

        public void UpdatePos()
        {
            if (index < 9)
            {
                index++;
                poss[index] = new Vector3(Round(transform.position.x),
                Round(transform.position.y),
                Round(transform.position.z));
                DataPool.instance.SetInList(ID, Round(transform.position.x),
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
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") && isGroundAudio)
                {
                    isGroundAudio = false;
                    aud.PlayAudioClip(0);
                }
                else
                {

                }
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
