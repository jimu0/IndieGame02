using UnityEngine;

namespace File_jim.Script.PlayerAbility
{
    public class Push : MonoBehaviour
    {
        private BoxMovement box;
        private Vector3Int dir;
        public float detectionDistance = 0.7f;//射线的检测距离
        public LayerMask boxLayerMask; //仅检测Box层的对象
        //public Rigidbody rb;

        public BoxMovManager boxMovManager;//box管理器类
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
            //获取玩家位置和箱子位置的整数坐标
            Vector3Int playerPos = Vector3Int.RoundToInt(transform.position);
            Vector3Int boxPos = Vector3Int.RoundToInt(box.transform.position);
            Vector3Int direction = boxPos - playerPos;//计算整数坐标差异
            dir = GetCardinalDirection(direction);//获取最接近的主要方向
            if (box != null)
            {
                box.GetComponent<BoxMovement>().PushTo(dir, 0.2f);
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
        
        //临时：生成box的调用
        public void Aaa()
        {
            boxMovManager.GenerateNewBox();
        }
    }
}
