using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UITemplate
{
    public class PlayerController2 : MonoBehaviour
    {
        public GameObject meshRoot;//角色模型根
        public float moveSpeed = 3f;//移动速度
        public float jumpForce = 3f;//跳跃力
        public float rotationSpeed = 720f; //旋转速度（每秒度数）
        public LayerMask groundLayers;//地面层
        [Header("轴随头变换_jim")]
        [SerializeField] private bool CameraChangesAxis = true;
        public Camera mainCamera;
        private Rigidbody rb;
        private bool isGrounded;
        private bool canJump;               // 是否可以跳跃
        
        public float additionalJumpForce = 2f; // 额外跳跃力
        public float downwardForce = 1f;     // 向下的力
        private VariableJoystick joystick;
        public Canvas anCanvas;
        private PhysicMaterial defaultMaterial;
        private PhysicMaterial noFrictionMaterial;

        void Start()
        {
            
            rb = GetComponent<Rigidbody>();
            // 创建一个没有摩擦力的物理材质
            noFrictionMaterial = new PhysicMaterial();
            noFrictionMaterial.dynamicFriction = 0f;
            noFrictionMaterial.staticFriction = 0f;
            noFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            // 保存默认的物理材质
            if (GetComponent<Collider>() != null)
            {
                defaultMaterial = GetComponent<Collider>().material;
            }
            
#if UNITY_STANDALONE
            AnCanvas.gameObject.SetActive(false);
#endif
#if UNITY_ANDROID
            anCanvas.gameObject.SetActive(true);
            joystick = anCanvas.GetComponentInChildren<VariableJoystick>();
#endif
        }

        void Update()
        {
            // 检查是否在地面上
            isGrounded = IsGrounded();
            if (isGrounded)
            {
                canJump = true; // 只有在接触地面时才能跳跃
            }
            
            float moveX, moveZ;//移动输入
#if UNITY_STANDALONE
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
#endif
#if UNITY_ANDROID
            moveX = joystick.Horizontal;
            moveZ = joystick.Vertical;
#endif
            Transform tsf = transform;
            Vector3 move = Move(tsf, moveX, moveZ);//移动方向
            rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);//设置水平速度，保持垂直速度不变
            
            //如果有移动输入，则旋转角色
            if (move != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
                meshRoot.transform.rotation = Quaternion.RotateTowards(meshRoot.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            
            if (Input.GetButtonDown("Jump") && canJump) Jump();//跳跃
            
            // 添加向下的力
            rb.AddForce(Vector3.down * downwardForce);
        }

        //跳跃方法
        public void Jump()
        {
            if (isGrounded)
            {
                canJump = false; // 跳跃后禁用跳跃，直到接触地面
                // 临时降低摩擦力
                if (GetComponent<Collider>() != null)
                {
                    GetComponent<Collider>().material = noFrictionMaterial;
                }

                // 添加跳跃力和额外向上力
                rb.AddForce(Vector3.up * (jumpForce + additionalJumpForce), ForceMode.Impulse);

                // 恢复默认的物理材质
                Invoke("RestoreDefaultMaterial", 0.1f); // 延迟恢复，确保跳跃时摩擦力降低

            }
        

        }

        //检测是否在地面上
        bool IsGrounded()
        {
            float distanceToGround = 0.1f;
            return Physics.Raycast(transform.position, Vector3.down, distanceToGround, groundLayers);
        }

        Vector3 Move(Transform tsf, float moveX, float moveZ)
        {
            Vector3 move;
            //轴随镜头变换
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
                move = (tsf.right * moveX + tsf.forward * moveZ); //移动方向
            }
            move *= moveSpeed;
            return move;
        }
    }
}
