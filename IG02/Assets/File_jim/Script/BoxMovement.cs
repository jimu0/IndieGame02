using System.Collections;
using UnityEngine;

namespace File_jim.Script
{
    public class BoxMovement : MonoBehaviour
    {
        private Vector3Int objPos;//????
        private Vector3Int newObjPos;//??????
        public int id = 1;//???id
        private bool isMoving; //???????
        private void OnEnable()
        {
            Chessboard.OnMoveBoxesToTarget += MoveTo;
        }
        private void OnDisable()
        {
            Chessboard.OnMoveBoxesToTarget -= MoveTo;
        }

        private void Start()
        {
            objPos = Vector3Int.RoundToInt(transform.position);
            newObjPos = objPos;
        }

        /// <summary>
        /// ???????????????????
        /// </summary>
        /// <param name="speed">???</param>
        private void MoveTo(float speed)
        {
            if (isMoving) return; // ????????
            Vector3Int movPos = Chessboard.GetMatrixP(id); //???box??????????????????
            if (movPos == objPos) return; //????????????
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
        /// ??
        /// </summary>
        /// <param name="direction">????</param>
        /// <param name="speed">???</param>
        public void PushTo(Vector3Int direction, float speed)
        {
            if (isMoving) return;
            StartCoroutine(PushToCoroutine(direction, speed));
        }
        private IEnumerator PushToCoroutine(Vector3Int direction, float speed)
        {
            Chessboard.FallBoxV(objPos, direction, out bool f);
            if (!f) yield break;
            MoveTo(speed);
        }
    }

}

