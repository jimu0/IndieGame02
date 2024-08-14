using System.Collections;
using System.Collections.Generic;
using File_jim.Script;
using File_jim.Script.PlayerAbility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UITemplate
{
    public class PlayerController2 : MonoBehaviour
    {
        public int id = 10;
        public int hp = 1;
        public GameObject meshRoot;//角色模型根
        public float moveSpeed = 2f;//移动速度
        public float jumpForce = 21f;//跳跃力
        public float rotationSpeed = 860f;//旋转速度（每秒度数）
        public LayerMask groundLayers;//地面层
        [Header("开启设计师模式_jim")]
        [SerializeField] public bool DesignerMode = false;
        public GameObject playerSelectBox;//玩家选框
        [Header("移动轴随镜头变换_jim")]
        [SerializeField] private bool CameraChangesAxis = true;
        public Camera mainCamera;
        private Rigidbody rb;
        private bool isGrounded;
        private bool canJump;//是否可以跳跃
        public Push push;
        public float additionalJumpForce = 2f;//额外跳跃力
        public float downwardForce = 1f;//向下的力
        private VariableJoystick joystick;
        public Canvas anCanvas;
        private PhysicMaterial defaultMaterial;
        private PhysicMaterial noFrictionMaterial;
        private Collider defaultPhysic;
        private Chessboard chessboard;
        private int objectId = 10001;
        public Text uiText;
        
        private void OnEnable()
        {
            Chessboard.OnDecisionRoleAllToTarget += WhereI;
        }

        private void OnDisable()
        {
            Chessboard.OnDecisionRoleAllToTarget -= WhereI;
        }

        void Start()
        {
            saveButton.onClick.AddListener(() => SaveMspData(inputField.text));
            if (uiText != null) objectId = 10001; UpdateTex();
            rb = GetComponent<Rigidbody>();
            defaultPhysic = GetComponent<Collider>();

            // 创建一个没有摩擦力的物理材质
            noFrictionMaterial = new PhysicMaterial();
            noFrictionMaterial.dynamicFriction = 0f;
            noFrictionMaterial.staticFriction = 0f;
            noFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            // 保存默认的物理材质
            if (defaultPhysic != null)
            {
                defaultMaterial = defaultPhysic.material;
            }

            
#if UNITY_STANDALONE
            //anCanvas.gameObject.SetActive(false);
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
            if (DesignerMode)
            {
                rb.useGravity = false;
                rb.constraints = rb.constraints | RigidbodyConstraints.FreezePositionY;
                defaultPhysic.enabled = false;
                playerSelectBox.SetActive(true);
                anCanvas.gameObject.SetActive(true);
                moveSpeed = 4f;
                //Vector3 move = Move(tsf, moveX, moveZ);
                if (Input.GetKeyDown(KeyCode.LeftShift)||Input.GetKeyDown(KeyCode.RightShift)||Input.GetButtonDown("Jump"))
                {
                    Vector3 tsfPos = tsf.position;
                    tsfPos.y += 1;
                    tsf.position = tsfPos;
                }
                if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.N))
                {
                    Vector3 tsfPos = tsf.position;
                    tsfPos.y -= 1;
                    tsf.position = tsfPos;
                }
                //float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (Input.GetKeyDown(KeyCode.Equals))//按下=号
                {
                    objectId++;
                    if (objectId < 10001) objectId = 10001;
                    UpdateTex();
                }

                if (Input.GetKeyDown(KeyCode.Minus))//按下-号
                {
                    objectId--;
                    if (objectId < 10001) objectId = 0;
                    UpdateTex();
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    Vector3Int v = default;
                    Vector3 position = playerSelectBox.transform.position;
                    v.x = Mathf.RoundToInt(position.x);
                    v.y = Mathf.RoundToInt(position.y);
                    v.z = Mathf.RoundToInt(position.z);
                    Fangzhi(v,objectId);
                }
                if (Input.GetMouseButtonDown(2))objectId = 10001; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha0))objectId = 0; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha1))objectId = 10001; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha2))objectId = 10002; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha3))objectId = 10003; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha4))objectId = 10004; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha5))objectId = 10005; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha6))objectId = 10006; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha7))objectId = 10007; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha8))objectId = 10008; UpdateTex();
                if (Input.GetKeyDown(KeyCode.Alpha9))objectId = 10009; UpdateTex();
            }
            else
            {
                rb.useGravity = true;
                rb.constraints = rb.constraints & ~RigidbodyConstraints.FreezePositionY;
                defaultPhysic.enabled = true;
                playerSelectBox.SetActive(false);
                anCanvas.gameObject.SetActive(false);
                moveSpeed = 2f;
                if (Input.GetButtonDown("Jump") && canJump) Jump();//跳跃
                if (Input.GetKeyDown(KeyCode.H))
                {
                    push.PushToBox();
                }
                // 添加向下的力
                rb.AddForce(Vector3.down * downwardForce);
            }

            if (Input.GetKeyDown(KeyCode.Tab)) DesignerMode = !DesignerMode;//缺换设计师模式/玩家模式

        }

        private void WhereI(float pulse)
        {
            
        }
        
        //跳跃方法
        public void Jump()
        {
            if (isGrounded)
            {
                canJump = false; // 跳跃后禁用跳跃，直到接触地面
                // 临时降低摩擦力
                if (defaultPhysic != null)
                {
                    defaultPhysic.material = noFrictionMaterial;
                }

                // 添加跳跃力和额外向上力
                rb.velocity = Vector3.zero; // 重置速度
                rb.AddForce(Vector3.up * (jumpForce + additionalJumpForce), ForceMode.Impulse);
                // 恢复默认的物理材质
                Invoke($"RestoreDefaultMaterial", 0.1f); // 延迟恢复，确保跳跃时摩擦力降低

            }
        

        }

        private void RestoreDefaultMaterial()
        {
            defaultPhysic.material = defaultMaterial;
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
                Transform camTsf = mainCamera.transform;
                Vector3 cameraForward = camTsf.forward;
                Vector3 cameraRight = camTsf.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                move = (cameraForward * moveZ + cameraRight * moveX).normalized;
            }
            else
            {
                move = (tsf.right * moveX + tsf.forward * moveZ); //移动方向
            }
            move *= moveSpeed;
            if (playerSelectBox)
            {
                Vector3Int playerSelectBoxPos = default;
                Vector3 position = tsf.position;
                playerSelectBoxPos.x = Mathf.RoundToInt(position.x);
                playerSelectBoxPos.y = Mathf.RoundToInt(position.y);
                playerSelectBoxPos.z = Mathf.RoundToInt(position.z);
                playerSelectBox.transform.position = playerSelectBoxPos;
            }
            else
            {
                Debug.LogWarning($"玩家单位{name}缺失playerSelectBox");
            }
            return move;
        }

        // void Move2(Transform tsf, float moveX, float moveZ)
        // {
        //     if (moveX != 0 || moveZ != 0)
        //     {
        //         Vector3Int moveVector = new((int)moveX, 0, (int)moveZ);
        //         tsf.position += (Vector3)moveVector * moveSpeed;
        //     }
        // }

        void Fangzhi(Vector3Int posInt,int id)
        {
            if(!chessboard) chessboard = FindObjectOfType<Chessboard>();
            if (!chessboard.CheckInRange(posInt)) return;
            //清了再建
            chessboard.DestroyObj(ChessboardSys.Instance.matrix[posInt.x,posInt.y,posInt.z]);
            if (id != 0)
            {
                chessboard.uniqueId++;
                int soleId = id * 10000 + chessboard.uniqueId;
                chessboard.GenerateNewBox(posInt, id, soleId);
            }
        }

        
        public TMP_InputField inputField;
        public Button saveButton; 
        public void SaveMspData(string inputFieldText)
        {
            DataManager.Instance.SaveMapData(inputFieldText);
        }
        
        public void UpdateTex()
        {
            if (uiText != null&&ChessboardSys.Instance.mapTiles!=null)
            {
                MapTile tileData;
                if (ChessboardSys.Instance.mapTiles.ContainsKey(objectId))
                {
                    tileData = ChessboardSys.Instance.mapTiles[objectId];
                    uiText.text =
                        $"ID:{objectId.ToString()},\nName:{tileData.Name}.\nEntity:{tileData.Entity},\nCollision:{tileData.Collision},\nGravity:{tileData.Gravity},\nMove:X {tileData.MoveX},Z {tileData.MoveZ},\nHp:{tileData.Hp},\nSkillId:{tileData.SkillId},\nDescription:{tileData.Description}";
                }                
                else
                {
                    uiText.text = $"id:{objectId},(无效)";
                }
                
            }
        }
        
        public void SetHp(int n)
        {
            //Hp大于100000即代表无敌
            if (hp > 100000) return;
            hp += n;
            if (hp <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Debug.Log("玩家触发死亡");
        }

        public void AddForce(Vector3 d,int i,ForceMode fm, bool re)
        {
            if (re) rb.velocity = Vector3.zero;
            rb.AddForce(d * i, fm);
        }
    }
}
