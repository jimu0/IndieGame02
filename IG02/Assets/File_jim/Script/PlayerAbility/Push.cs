using File_jim.Script;
using UnityEngine;

namespace File_jim.Script.PlayerAbility
{
    public class Push : MonoBehaviour
    {
        private BoxMovement box;
        private Vector3Int dir;
        public float detectionDistance = 0.7f;//���ߵļ�����
        public LayerMask boxLayerMask; //�����Box��Ķ���
        public Rigidbody rb;

        public BoxMovManager boxMovManager;//box��������
        void Start()
        {
            box = null;
        }

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
            return null;
        }
        public void PushToBox()
        {
            box = FindBoxInFront();
            
            if (box != null)
            {
                box.GetComponent<BoxMovement>().PushTo(dir, 0.2f);
            }

        }
        
        //��ʱ������box�ĵ���
        public void Aaa()
        {
            boxMovManager.GenerateNewBox();
        }
    }
}
