using Add;
using EditorPlus;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerManagement
{
    /// <summary>
    /// 控制玩家基础行为
    /// </summary>
    public class PlayerController1 : MonoBehaviour
    {
        public BoolVar isPlayerMoving;
        [Header("轴随头变换_jim")]
        [SerializeField] private bool CameraChangesAxis = true;
        public Camera mainCamera;
        [ReadOnly]
        public bool isPullingHori;
        [ReadOnly]
        public bool isPullingVerti;
        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private bool cubePlayer;//站在cube上也允许继续跳跃
        public float PlayerSpeed = 8;
        public float JumpHeight = 4;
        public float GravityValue = -9.81f;
        [Header("射线检测距离")]
        public float GroundCheckDistance;
        [Header("安卓")]

        [SerializeField] private Button JoyBtn;
#if UNITY_ANDROID
        private VariableJoystick joystick;
        private bool isAnJumping;
        private float anJumpingTimer;
#endif
        public Canvas AnCanvas;
        
        private void Start()
        {
            controller = gameObject.GetComponent<CharacterController>();
            AnCanvas.gameObject.SetActive(false);
#if UNITY_ANDROID
            AnCanvas.gameObject.SetActive(true);
            joystick = AnCanvas.GetComponentInChildren<VariableJoystick>();
            JoyBtn.onClick.AddListener(() =>
            {
                isAnJumping = true;
            });
#endif
        }

        void Update()
        {
            groundedPlayer = Physics.Linecast(transform.position, 
                new Vector3(transform.position.x, transform.position.y + GroundCheckDistance, transform.position.z)
                , 1 << LayerMask.NameToLayer("Ground"));
            cubePlayer = Physics.Linecast(transform.position, 
                new Vector3(transform.position.x, transform.position.y + GroundCheckDistance, transform.position.z)
                , 1 << LayerMask.NameToLayer("Cube"));
            
            // if ((groundedPlayer||cubePlayer) && playerVelocity.y < 0)
            // {
            //     playerVelocity.y = 0f;
            // }
            
            float hori;
            float verti;
            
//#if UNITY_STANDALONE
            hori = Input.GetAxis("Horizontal");
            verti = Input.GetAxis("Vertical");
            
            
            if (Input.GetButtonDown("Jump") && (groundedPlayer||cubePlayer) && CanJump())
            {
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
            } 
//#endif
#if UNITY_ANDROID
            hori = joystick.Horizontal;
            verti = joystick.Vertical;
            
            HandleJoystickInput(hori, playerVelocity.y , verti);//处理摇杆输入，移动角色或执行其他操作

            if(isAnJumping && (groundedPlayer||cubePlayer))
            {
                
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
                isAnJumping = false;
            }
#endif
            playerVelocity.y += GravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
            
            if(Mathf.Abs(hori) > 0.2f || Mathf.Abs(verti) > 0.2f)isPlayerMoving.Value = true;
            else isPlayerMoving.Value = false;
            
        }
        
        bool CanJump()
        {
            return (isPullingVerti && isPullingHori) == false;
        }

        public void Jump()
        {
            isAnJumping = true;
            playerVelocity.y += Mathf.Sqrt(10 * -3.0f * GravityValue);

        }

        void HandleJoystickInput(float hori,float pV, float verti)
        {
            Vector3 move;
            if (isPullingVerti) hori = 0;
            if(isPullingHori)verti = 0;
            //轴随镜头变换
            if (CameraChangesAxis)
            {
                Vector3 cameraForward = mainCamera.transform.forward;
                Vector3 cameraRight = mainCamera.transform.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                Vector3 moveDirection = (cameraForward * verti + cameraRight * hori).normalized;
                move = moveDirection;
            }
            else
            {
                move.x=hori;
                move.y = 0;
                move.z=verti;
            }
            
            controller.Move(move * Time.deltaTime * PlayerSpeed);
            
            // if (move != Vector3.zero)
            // {
            //     gameObject.transform.forward = move;
            // }
        }

    }

}
