using System;
using System.Collections;
using File_jim.Script.BoxSkill;
using UnityEngine;

namespace File_jim.Script
{
    public class Block : MonoBehaviour
    {
        private Vector3Int objPos;//λ��
        private Vector3Int newObjPos;//��λ��
        public int id = 1;//ΨһID
        private bool isMoving; //�ƶ���?
        
        public bool entityBox = true;//��ʵ��?
        public bool collisionBox = true;//��ײ��?
        public bool gravityBox = true;//������?
        public bool removableBoxX = true;//��λ��?
        public bool removableBoxZ = true;
        public bool destroyableBox = true;//�ɴݻ�?
        private IBoxSkill assignedSkill;
        public BoxCollider boxCollider;

        private void OnEnable()
        {
            if(entityBox) Chessboard.OnMoveBoxesToTarget += MoveTo;
            Chessboard.OnMoveBoxesToTarget += TriggerEveryTurn;
        }
        

        private void OnDisable()
        {
            if(entityBox) Chessboard.OnMoveBoxesToTarget -= MoveTo;
            Chessboard.OnMoveBoxesToTarget -= TriggerEveryTurn;
        }

        private void Awake()
        {
            objPos = Vector3Int.RoundToInt(transform.position);
            newObjPos = objPos;
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="tile"></param>
        public void SetAbility(MapTile tile)
        {
            entityBox = tile.Entity;
            collisionBox = tile.Collision;
            gravityBox = tile.Gravity;
            removableBoxX = tile.MoveX;
            removableBoxZ = tile.MoveZ;
            destroyableBox = tile.Hp > 1;
        }

        #region BoxMove
        /// <summary>
        /// �ƶ�
        /// </summary>
        /// <param name="speed">�ٶ�</param>
        private void MoveTo(float speed)
        {
            if (isMoving) return; //�����������ƶ���ͻ
            Vector3Int movPos = Chessboard.GetMatrixP(id);//��ȡ���¹���λ��
            if (movPos == objPos) return;
            
            // if (!gravityBox)//�ж��ƶ������Ƿ�������
            // {
            //     if (movPos.y < objPos.y && Mathf.Approximately(objPos.x, movPos.x) &&
            //         Mathf.Approximately(objPos.z, movPos.z)) return;
            // }

            newObjPos = movPos;
            StartCoroutine(MoveToCoroutine(speed));
        }
        private IEnumerator MoveToCoroutine(float duration)
        {
            isMoving = true;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(objPos, newObjPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = newObjPos;
            objPos = newObjPos;
            isMoving = false;
        }

        /// <summary>
        /// �Ƴ�ȥ
        /// </summary>
        /// <param name="direction">����</param>
        /// <param name="speed">�ٶ�</param>
        public void PushTo(Vector3Int direction, float speed)
        {
            if (!(entityBox) || isMoving) return;
            StartCoroutine(PushToCoroutine(direction, speed));
        }
        private IEnumerator PushToCoroutine(Vector3Int direction, float speed)
        {
            Chessboard.FallBoxP(objPos, direction, out bool f);
            if (!f) yield break;
            MoveTo(speed);
        }
        #endregion
        
        #region BoxSkill
        
        private void Start()
        {
            boxCollider.enabled = collisionBox;
            //����ʱ�����ļ���
            assignedSkill = SkillFactory.CreateSkill(id);
            assignedSkill?.OnCreate(this);
        }
        /// <summary>
        /// �ƶ�����ʱ����
        /// </summary>
        /// <param name="newPosition"></param>
        public void TriggerMoveEnd(Vector3 newPosition)
        {
            transform.position = newPosition;
            assignedSkill?.OnMoveEnd(this);
        }
        /// <summary>
        /// ����ʱ����
        /// </summary>
        public void TriggerDestroyBlock()
        {
            if (!destroyableBox) return;
            assignedSkill?.OnDestroy(this);
            Destroy(gameObject);
        }
        /// <summary>
        /// ��������
        /// </summary>
        public void TriggerPassive()
        {
            assignedSkill?.OnPassive(this);
        }
        /// <summary>
        /// ����������������
        /// </summary>
        public void TriggerEveryTurn(float speed)
        {
            assignedSkill?.OnEveryTurn(this);
        }
        
        #endregion
    }

}

