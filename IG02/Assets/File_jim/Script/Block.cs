using System;
using System.Collections;
using File_jim.Script.BoxSkill;
using UnityEngine;

namespace File_jim.Script
{
    public class Block : MonoBehaviour
    {
        public Chessboard chessboard;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        
        public Vector3Int objPos;//λ��
        private Vector3Int newObjPos;//��λ��
        public int id = 1;//ΨһID
        //public int skillId = 0;
        private bool isMoving; //�ƶ���?
        public MapTile boxAbi = new();
        //public bool entityBox = true;//��ʵ��?
        //public bool collisionBox = true;//��ײ��?
        //public bool gravityBox = true;//������?
        //public bool removableBoxX = true;//��λ��?
        //public bool removableBoxZ = true;
        //public bool destroyableBox = true;//�ɴݻ�?
        private IBoxSkill assignedSkill;
        public BoxCollider boxCollider;

        private void OnEnable()
        {
            Chessboard.OnMoveBoxesToTarget += MoveTo;
            
            Chessboard.OnMoveBoxesToTarget += TriggerPassive;
            Chessboard.OnMoveBoxesToTarget += TriggerEveryTurn;
        }
        

        private void OnDisable()
        {
            Chessboard.OnMoveBoxesToTarget -= MoveTo;
            
            Chessboard.OnMoveBoxesToTarget -= TriggerPassive;
            Chessboard.OnMoveBoxesToTarget -= TriggerEveryTurn;
        }

        private void Awake()
        {
            objPos = Vector3Int.RoundToInt(gameObject.transform.position);
            newObjPos = objPos;
        }
        
        
        #region BoxMove

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="pulse">频率</param>
        private void MoveTo(float pulse)
        {
            if (isMoving) return; //防止移动冲突
            isMoving = true;
            Vector3Int movPos = Chessboard.GetMatrixP(id);//获取到位置判断一下在不在原地
            if (movPos == objPos)
            {
                isMoving = false;
                return;
            }
            
            newObjPos = movPos;
            StartCoroutine(MoveToCoroutine(pulse));
        }
        private IEnumerator MoveToCoroutine(float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                gameObject.transform.position = Vector3.Lerp(objPos, newObjPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            bool b = newObjPos.y + 0.5f < objPos.y;
            objPos = newObjPos;
            gameObject.transform.position = objPos;
            isMoving = false;
            if(b)TriggerMoveEnd();
        }

        /// <summary>
        /// �Ƴ�ȥ
        /// </summary>
        /// <param name="direction">����</param>
        /// <param name="speed">�ٶ�</param>
        public void PushTo(Vector3Int direction, float speed)
        {
            if (!(boxAbi.Entity) || isMoving) return;
            if (!boxAbi.MoveX) direction.x = 0;
            if (!boxAbi.MoveZ) direction.z = 0;
            if (direction == Vector3Int.zero) return;
            StartCoroutine(PushToCoroutine(direction, speed));
        }
        private IEnumerator PushToCoroutine(Vector3Int direction, float speed)
        {
            Chessboard.FallBoxP(objPos, direction, out bool f);
            if (!f) yield break;
            MoveTo(speed);
        }

        public void SetHp(int n)
        {
            //Hp大于100000即代表无敌
            if (boxAbi.Hp > 100000) return;
            boxAbi.Hp += n;
        }

        #endregion
        
        #region BoxSkill
        
        private void Start()
        {
            boxCollider.enabled = boxAbi.Collision;
            //����ʱ�����ļ���
            assignedSkill = SkillFactory.CreateSkill(boxAbi.SkillId);
            assignedSkill?.OnCreate(this);
        }
        /// <summary>
        /// 移动结束后触发
        /// </summary>
        public void TriggerMoveEnd()
        {
            assignedSkill?.OnMoveEnd(this);
        }
        /// <summary>
        /// ����ʱ����
        /// </summary>
        public void TriggerDestroyBlock()
        {
            if (boxAbi.Hp > 0) return;
            assignedSkill?.OnDestroy(this);
        }

        /// <summary>
        /// 触发技能：被侵蚀
        /// </summary>
        /// <param name="intruderID"></param>
        public void TriggerBeEncroach(int intruderID)
        {
            assignedSkill?.OnBeEncroach(this, intruderID);
        }

        /// <summary>
        /// 触发技能：被动
        /// </summary>
        public void TriggerPassive(float pulse)
        {
            int intruderID = ChessboardSys.Instance.GetMatrixValue(objPos.x, objPos.y + 1, objPos.z);
            if (intruderID == 10)
            {
                assignedSkill?.OnPassive(this, chessboard);
            }

        }
        /// <summary>
        /// 触发技能：持续
        /// </summary>
        public void TriggerEveryTurn(float pulse)
        {
            //生命结束销毁自己
            if (boxAbi.Hp <= 0)
            {
                chessboard.DestroyObj(id);
                return;
            }
            //随心率而响应的技能
            assignedSkill?.OnEveryTurn(this);
        }
        
        #endregion
    }

}
