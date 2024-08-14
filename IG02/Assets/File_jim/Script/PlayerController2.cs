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
        public GameObject meshRoot;//��ɫģ�͸�
        public float moveSpeed = 2f;//�ƶ��ٶ�
        public float jumpForce = 21f;//��Ծ��
        public float rotationSpeed = 860f;//��ת�ٶȣ�ÿ�������
        public LayerMask groundLayers;//�����
        [Header("�������ʦģʽ_jim")]
        [SerializeField] public bool DesignerMode = false;
        public GameObject playerSelectBox;//���ѡ��
        [Header("�ƶ����澵ͷ�任_jim")]
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
            //anCanvas.gameObject.SetActive(false);
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
                if (Input.GetKeyDown(KeyCode.Equals))//����=��
                {
                    objectId++;
                    if (objectId < 10001) objectId = 10001;
                    UpdateTex();
                }

                if (Input.GetKeyDown(KeyCode.Minus))//����-��
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
                if (Input.GetButtonDown("Jump") && canJump) Jump();//��Ծ
                if (Input.GetKeyDown(KeyCode.H))
                {
                    push.PushToBox();
                }
                // ������µ���
                rb.AddForce(Vector3.down * downwardForce);
            }

            if (Input.GetKeyDown(KeyCode.Tab)) DesignerMode = !DesignerMode;//ȱ�����ʦģʽ/���ģʽ

        }

        private void WhereI(float pulse)
        {
            
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
                rb.velocity = Vector3.zero; // �����ٶ�
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
                Transform camTsf = mainCamera.transform;
                Vector3 cameraForward = camTsf.forward;
                Vector3 cameraRight = camTsf.right;
                cameraForward.y = 0;
                cameraRight.y = 0;
                move = (cameraForward * moveZ + cameraRight * moveX).normalized;
            }
            else
            {
                move = (tsf.right * moveX + tsf.forward * moveZ); //�ƶ�����
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
                Debug.LogWarning($"��ҵ�λ{name}ȱʧplayerSelectBox");
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
            //�����ٽ�
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
                    uiText.text = $"id:{objectId},(��Ч)";
                }
                
            }
        }
        
        public void SetHp(int n)
        {
            //Hp����100000�������޵�
            if (hp > 100000) return;
            hp += n;
            if (hp <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Debug.Log("��Ҵ�������");
        }

        public void AddForce(Vector3 d,int i,ForceMode fm, bool re)
        {
            if (re) rb.velocity = Vector3.zero;
            rb.AddForce(d * i, fm);
        }
    }
}
