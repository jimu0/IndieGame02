using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerManagement
{
    /// <summary>
    /// 控制玩家基础行为
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        public float PlayerSpeed = 8;
        public float JumpHeight = 4;
        public float GravityValue = -9.81f;
        public float GroundCheckDistance;
        [Header("安卓")]
#if UNITY_ANDROID

        [SerializeField] private Button JoyBtn;

        private VariableJoystick joystick;
        private bool isAnJumping;
        private float anJumpingTimer;

#endif
        public Canvas AnCanvas;

        private void Start()
        {
#if UNITY_ANDROID == false
            controller = gameObject.GetComponent<CharacterController>();
            AnCanvas.gameObject.SetActive(false);
#endif
#if UNITY_ANDROID
            joystick = GetComponentInChildren<VariableJoystick>();
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

            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            float hori = Input.GetAxis("Horizontal");
            float verti = Input.GetAxis("Vertical");


#if UNITY_ANDROID
            hori = joystick.Horizontal;
            verti = joystick.Vertical;
#endif
            if ((hori != 0 && verti == 0) || (hori == 0 && verti != 0))
            {
                Vector3 move = new Vector3(hori, 0, verti);
                controller.Move(move * Time.deltaTime * PlayerSpeed);

                // 改变角色的高度位置
                if (move != Vector3.zero)
                {
                    gameObject.transform.forward = move;
                }
            }
            else
            {
                if(hori == 0)
                {
                    Vector3 move = new Vector3(0, 0, verti);
                    controller.Move(move * Time.deltaTime * PlayerSpeed);

                    // 改变角色的高度位置
                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                }
                else
                {
                    Vector3 move = new Vector3(hori, 0, 0);
                    controller.Move(move * Time.deltaTime * PlayerSpeed);

                    // 改变角色的高度位置
                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                }
            }



            // 跳跃逻辑
            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
            }

#if UNITY_ANDROID
            if(isAnJumping && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
                isAnJumping = false;
            }
#endif

            playerVelocity.y += GravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + GroundCheckDistance, transform.position.z));
        }
    }

}
