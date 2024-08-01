using System.Collections;
using UnityEngine;

namespace File_jim.Script
{
    public class BoxMovement : MonoBehaviour
    {
        private Vector3Int objPos;//位置
        private Vector3Int newObjPos;//新位置
        public int id = 1;//唯一id
        private bool isMoving; //移动中？
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
        /// 移动到数据库指定的位置
        /// </summary>
        /// <param name="speed">速度</param>
        private void MoveTo(float speed)
        {
            if (isMoving) return; // 防止同时移动
            Vector3Int movPos = BoxMovManager.GetMatrixP(id); //通过box管理器找到我的新位置
            if (movPos == objPos) return; //位置没变就不移动
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
        /// 推
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="speed">速度</param>
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

