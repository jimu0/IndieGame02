using UnityEngine;
using UnityEngine.Serialization;

namespace File_jim.Script.PlayerAbility
{
    public class Push : MonoBehaviour
    {
        private BoxMovement box;
        private Vector3Int dir;
        public float detectionDistance = 0.7f;//??????????
        public LayerMask boxLayerMask; //?????Box??????
        //public Rigidbody rb;

        [FormerlySerializedAs("boxManager")] [FormerlySerializedAs("boxMovManager")] public Chessboard chessboard;//box????????
        void Start()
        {
            box = null;
        }

        void Update()
        {
            
            // if (rb != null)
            // {
            //     Vector3 velocity = rb.velocity;
            //     if (velocity.sqrMagnitude > 0.03f)
            //     {
            //         Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0 * Time.deltaTime);
            //     }
            // }

        }

        
        BoxMovement FindBoxInFront()
        {
            var transform1 = transform;
            Vector3 forward = transform1.forward;
            var position = transform1.position;
            Ray ray = new Ray(position, forward);
            Debug.DrawRay(position, forward * detectionDistance, Color.red, 1.0f);

            RaycastHit[] hits = Physics.RaycastAll(ray, detectionDistance, boxLayerMask);
            if (hits.Length > 0)
            {
                return hits[0].collider.GetComponent<BoxMovement>();
            }
            return null;
        }
        public void PushToBox()
        {
            box = FindBoxInFront();
            //?????????????????????????????
            Vector3Int playerPos = Vector3Int.RoundToInt(transform.position);
            Vector3Int boxPos = Vector3Int.RoundToInt(box.transform.position);
            Vector3Int direction = boxPos - playerPos;//???????????????
            dir = GetCardinalDirection(direction);//????????????????
            if (box != null)
            {
                box.PushTo(dir,0.2f);
            }

        }
        
        private Vector3Int GetCardinalDirection(Vector3 direction)
        {
            Vector3Int cardinalDirection;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                cardinalDirection = direction.x > 0 ? Vector3Int.right : Vector3Int.left;
            }
            else
            {
                cardinalDirection = direction.z > 0 ? Vector3Int.forward : Vector3Int.back;
            }

            return cardinalDirection;
        }
        
        //?????????box?????
        public void Aaa()
        {
            chessboard.GenerateNewBox_random();
        }
    }
}
