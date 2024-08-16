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
        public static event Action<int> OnGameStar;
        public static event Action<int> OnGameScore;

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

            bool b = newObjPos.y < objPos.y;
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
            if (boxAbi.Hp >= 999990) return;
            GameScore(10);
            boxAbi.Hp += n;
            
        }

        public void GameStar(int id)
        {
            OnGameStar?.Invoke(id);
        }
        public void GameScore(int v)
        {
            OnGameScore?.Invoke(v);
        }
        
        #endregion
        
        #region BoxSkill
        
        private void Start()
        {
            boxCollider.enabled = boxAbi.Collision;
            //����ʱ�����ļ���
            assignedSkill = SkillFactory.CreateSkill(boxAbi.SkillId);
            if(chessboard.stopCoroutine)return;
            assignedSkill?.OnCreate(this);
        }
        /// <summary>
        /// 移动结束后触发
        /// </summary>
        public void TriggerMoveEnd()
        {
            if(chessboard.stopCoroutine)return;
            assignedSkill?.OnMoveEnd(this);
        }
        /// <summary>
        /// 触发技能：销毁时
        /// </summary>
        public void TriggerDestroyBlock()
        {
            if(chessboard.stopCoroutine)return;
            if (boxAbi.Hp > 0) return;
            assignedSkill?.OnDestroy(this);
        }

        /// <summary>
        /// 触发技能：被侵占时
        /// </summary>
        /// <param name="intruderID"></param>
        public void TriggerBeEncroach(int intruderID)
        {
            if(chessboard.stopCoroutine)return;
            assignedSkill?.OnBeEncroach(this, chessboard, intruderID);
        }

        /// <summary>
        /// 触发技能：被动时
        /// </summary>
        public void TriggerPassive(float pulse)
        {
            if(chessboard.stopCoroutine)return;
            int intruderID = ChessboardSys.Instance.GetMatrixValue(objPos.x, objPos.y, objPos.z);
            if (intruderID == 10)
            {
                assignedSkill?.OnPassive(this, chessboard);
            }

        }
        /// <summary>
        /// 触发技能：持续时
        /// </summary>
        public void TriggerEveryTurn(float pulse)
        {
            if(chessboard.stopCoroutine)return;
            //生命结束销毁自己
            if (boxAbi.Hp <= 0)
            {
                chessboard.DestroyObj(id);
                return;
            }
            //随心率而响应的技能
            assignedSkill?.OnEveryTurn(this, chessboard);
        }
        
        #endregion
    }

}

