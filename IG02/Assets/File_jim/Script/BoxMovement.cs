using System.Collections;
using UnityEngine;

namespace File_jim.Script
{
    public class BoxMovement : MonoBehaviour
    {
        private Vector3Int objPos;//λ��
        private Vector3Int newObjPos;//��λ��
        public int id = 1;//Ψһid
        private bool isMoving; //�ƶ��У�
        private void OnEnable()
        {
            BoxMovManager.OnMoveBoxesToTarget += MoveTo;
        }
        private void OnDisable()
        {
            BoxMovManager.OnMoveBoxesToTarget -= MoveTo;
        }

        private void Start()
        {
            objPos = Vector3Int.RoundToInt(transform.position);
            newObjPos = objPos;
        }

        /// <summary>
        /// �ƶ������ݿ�ָ����λ��
        /// </summary>
        /// <param name="speed">�ٶ�</param>
        private void MoveTo(float speed)
        {
            if (isMoving) return; // ��ֹͬʱ�ƶ�
            Vector3Int movPos = BoxMovManager.GetMatrixP(id); //ͨ��box�������ҵ��ҵ���λ��
            if (movPos == objPos) return; //λ��û��Ͳ��ƶ�
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
        /// ��
        /// </summary>
        /// <param name="direction">����</param>
        /// <param name="speed">�ٶ�</param>
        public void PushTo(Vector3Int direction, float speed)
        {
            if (isMoving) return;
            StartCoroutine(PushToCoroutine(direction, speed));
        }
        private IEnumerator PushToCoroutine(Vector3Int direction, float speed)
        {
            BoxMovManager.FallBoxV(objPos, direction, out bool f);
            if (!f) yield break;
            MoveTo(speed);
        }
    }

}

