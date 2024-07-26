using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private void Start()
        {
            controller = gameObject.GetComponent<CharacterController>();
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
            if((hori != 0 && verti == 0) || (hori == 0 && verti != 0))
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
