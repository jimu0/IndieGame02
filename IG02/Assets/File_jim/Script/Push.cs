using System.Collections;
using System.Collections.Generic;
using Add;
using File_jim.Script;
using UnityEngine;
using UnityEngine.UIElements;

namespace File_jim
{
    public class Push : MonoBehaviour
    {
        private BoxMovement box;
        private Vector3Int dir;
        public float detectionDistance = 0.7f; // 射线的检测距离
        public LayerMask boxLayerMask; // 仅检测 Box 层的对象
        public Rigidbody rb;

        public BoxMovManager boxMovManager;
        // Start is called before the first frame update
        void Start()
        {
            box = null;
        }

        // Update is called once per frame
        void Update()
        {
            
            if (rb != null)
            {
                Vector3 velocity = rb.velocity;
                if (velocity.sqrMagnitude > 0.03f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0 * Time.deltaTime);
                }
            }
            Vector3 forward = transform.forward;
            Vector3Int forwardInt = Vector3Int.RoundToInt(forward);
            dir = forwardInt;

        }

        BoxMovement FindBoxInFront()
        {
            Vector3 forward = transform.forward;
            Ray ray = new Ray(transform.position, forward);
            Debug.DrawRay(transform.position, forward * detectionDistance, Color.red, 1.0f);

            RaycastHit[] hits = Physics.RaycastAll(ray, detectionDistance, boxLayerMask);
            if (hits.Length > 0)
            {
                return hits[0].collider.GetComponent<BoxMovement>();
            }
            else
            {
                return null;
            }
            
            //
            // if (Physics.RaycastAll(ray, out RaycastHit hit, detectionDistance, boxLayerMask))
            // {
            //     BoxMovement objBox = hit.collider.GetComponent<BoxMovement>();
            //     Debug.Log(objBox);
            //     return objBox;
            // }
            // return null;
        }
        public void PushToBox()
        {
            box = FindBoxInFront();
            
            if (box != null)
            {
                box.GetComponent<BoxMovement>().PushTo(dir, 0.2f);
            }

        }


        public void Aaa()
        {
            boxMovManager.GenerateNewBox();
        }
    }
}
