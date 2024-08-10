using System.Collections;
using System.Collections.Generic;
using File_jim.Script.PlayerAbility;
using UnityEngine;

namespace UITemplate
{
    public class PlayerController2 : MonoBehaviour
    {
        public GameObject meshRoot;//��ɫģ�͸�
        public float moveSpeed = 2f;//�ƶ��ٶ�
        public float jumpForce = 21f;//��Ծ��
        public float rotationSpeed = 860f;//��ת�ٶȣ�ÿ�������
        public LayerMask groundLayers;//�����
        [Header("����ͷ�任_jim")]
        [SerializeField] private bool CameraChangesAxis = true;
        public Camera mainCamera;
        private Rigidbody rb;
        private bool isGrounded;
        private bool canJump;//�Ƿ������Ծ
        public Push push;
        public float additionalJumpForce = 2f;//������Ծ��
        public float downwardForce = 1f;//���µ���
        private VariableJoystick joystick;
        public Canvas anCanvas;
        private PhysicMaterial defaultMaterial;
        private PhysicMaterial noFrictionMaterial;
        private Collider defaultPhysic;
        
        void Start()
        {
            
            rb = GetComponent<Rigidbody>();
            defaultPhysic = GetComponent<Collider>();

            // ����һ��û��Ħ�������������
            noFrictionMaterial = new PhysicMaterial();
            noFrictionMaterial.dynamicFriction = 0f;
            noFrictionMaterial.staticFriction = 0f;
            noFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            // ����Ĭ�ϵ��������
            if (defaultPhysic != null)
            {
                defaultMaterial = defaultPhysic.material;
            }

            
#if UNITY_STANDALONE
            anCanvas.gameObject.SetActive(false);
#endif
#if UNITY_ANDROID
            anCanvas.gameObject.SetActive(true);
            joystick = anCanvas.GetComponentInChildren<VariableJoystick>();
#endif
        }

        void Update()
        {
            // ����Ƿ��ڵ�����
            isGrounded = IsGrounded();
            if (isGrounded)
            {
                canJump = true; // ֻ���ڽӴ�����ʱ������Ծ
            }
            
            float moveX, moveZ;//�ƶ�����
#if UNITY_STANDALONE
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
#endif
#if UNITY_ANDROID
            moveX = joystick.Horizontal;
            moveZ = joystick.Vertical;
#endif
            Transform tsf = transform;
            Vector3 move = Move(tsf, moveX, moveZ);//�ƶ�����
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);//����ˮƽ�ٶȣ����ִ�ֱ�ٶȲ���
            
            //������ƶ����룬����ת��ɫ
            if (move != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
                meshRoot.transform.rotation = Quaternion.RotateTowards(meshRoot.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            if (Input.GetButtonDown("Jump") && canJump) Jump();//��Ծ
            if (Input.GetKeyDown(KeyCode.H))
            {
                push.PushToBox();
            }

            // ������µ���
            rb.AddForce(Vector3.down * downwardForce);
        }

        //��Ծ����
        public void Jump()
        {
            if (isGrounded)
            {
                canJump = false; // ��Ծ�������Ծ��ֱ���Ӵ�����
                // ��ʱ����Ħ����
                if (defaultPhysic != null)
                {
                    defaultPhysic.material = noFrictionMaterial;
                }

                // �����Ծ���Ͷ���������
                rb.AddForce(Vector3.up * (jumpForce + additionalJumpForce), ForceMode.Impulse);

                // �ָ�Ĭ�ϵ��������
                Invoke($"RestoreDefaultMaterial", 0.1f); // �ӳٻָ���ȷ����ԾʱĦ��������

            }
        

        }

        private void RestoreDefaultMaterial()
        {
            defaultPhysic.material = defaultMaterial;
        }

        //����Ƿ��ڵ�����
        bool IsGrounded()
        {
            float distanceToGround = 0.1f;
            return Physics.Raycast(transform.position, Vector3.down, distanceToGround, groundLayers);
        }

        Vector3 Move(Transform tsf, float moveX, float moveZ)
        {
            Vector3 move;
            //���澵ͷ�任
            if (CameraChangesAxis)
            {
                Transform cmaTsf = mainCamera.transform;
                Vector3 cameraForward = cmaTsf.forward;
                Vector3 cameraRight = cmaTsf.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                move = (cameraForward * moveZ + cameraRight * moveX).normalized;
            }
            else
            {
                move = (tsf.right * moveX + tsf.forward * moveZ); //�ƶ�����
            }
            move *= moveSpeed;
            return move;
        }
    }
}
