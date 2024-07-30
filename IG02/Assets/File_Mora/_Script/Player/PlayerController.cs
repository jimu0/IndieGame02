using Add;
using DG.Tweening;
using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerManagement
{
    /// <summary>
    /// ������һ�����Ϊ
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("ѡ�����mora�Ķ�����Ʒ���")]
        [SerializeField] private bool isDisAbleFloatVarMode = false;
        [Header("�Ƕȱ���(Disableʱ����Ҫ��ֵ)")]
        [SerializeField] private FloatVar angleWhenInPulling;
        [ReadOnly]
        public bool isPullingHori;
        [ReadOnly]
        public bool isPullingVerti;
        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private bool cubePlayer;//jimx��������ʱ����վ��cube��Ҳ���������Ծ
        public float PlayerSpeed = 8;
        public float JumpHeight = 4;
        public float GravityValue = -9.81f;
        [Header("���߼�����")]
        public float GroundCheckDistance;
        [Header("��׿")]

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
            if ((groundedPlayer||cubePlayer) && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            float hori = Input.GetAxis("Horizontal");
            float verti = Input.GetAxis("Vertical");
#if UNITY_ANDROID
            hori = joystick.Horizontal;
            verti = joystick.Vertical;
#endif
            //��������Ҫ�ŵ�hori��verti��ȡ������
            SolveIfLockAxis(ref hori, ref verti);
            if ((hori != 0 && verti == 0) || (hori == 0 && verti != 0))
            {
                Vector3 move = new Vector3(hori, 0, verti);

                controller.Move(move * Time.deltaTime * PlayerSpeed);

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

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                }
                else
                {
                    Vector3 move = new Vector3(hori, 0, 0);
                    controller.Move(move * Time.deltaTime * PlayerSpeed);

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                }
            }



            // ��Ծ�߼�
            if (Input.GetButtonDown("Jump") && (groundedPlayer||cubePlayer) && CanJump())
            {
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
            }

#if UNITY_ANDROID
            if(isAnJumping && (groundedPlayer||cubePlayer))
            {
                playerVelocity.y += Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
                isAnJumping = false;
            }
#endif
            if(isDisAbleFloatVarMode == false)
            {
                if (angleWhenInPulling.Value == 0 || angleWhenInPulling.Value == 180)
                {
                    isPullingVerti = true;
                    transform.Rotate(Vector3.up, angleWhenInPulling.Value - transform.eulerAngles.y);
                }
                if (angleWhenInPulling.Value == 90)
                {
                    isPullingHori = true;
                    transform.Rotate(Vector3.up, angleWhenInPulling.Value - transform.eulerAngles.y);
                }
                if (angleWhenInPulling.Value == 270)
                {
                    isPullingHori = true;
                    transform.Rotate(Vector3.down, -270 + transform.eulerAngles.y);
                }

                if (angleWhenInPulling.Value == -1)
                {
                    isPullingHori = isPullingVerti = false;
                }
            }
            playerVelocity.y += GravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

        }

        bool CanJump()
        {
            return (isPullingVerti && isPullingHori) == false;
        }

        void SolveIfLockAxis(ref float hori, ref float verti)
        {
            if (isPullingVerti)
            {
                hori = 0;
            }
            if(isPullingHori)
            {
                verti = 0;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + GroundCheckDistance, transform.position.z));
        }
    }

}
