using UnityEngine;

namespace File_jim.Script.PlayerAbility
{
    public class Push : MonoBehaviour
    {
        private Block box;
        private Vector3Int dir;
        public float detectionDistance = 0.7f;//??????????
        public LayerMask boxLayerMask; //?????Box??????

        public Chessboard chessboard;//box????????
        void Start()
        {
            box = null;
        }

        public void PushToBox()
        {
            Vector3Int selfPosition = Vector3Int.RoundToInt(transform.position);//��ȡ��ҵ�����λ��
            Vector3Int selfDirection = Vector3Int.RoundToInt(transform.forward);//��ȡ��ҵ�����������
            selfDirection.y = 0;
            if (Mathf.Abs(selfDirection.x) > Mathf.Abs(selfDirection.z))//ȷ������Ϊˮƽ����X��Z�ᣩ
            {
                selfDirection.z = 0;
                selfDirection.x = selfDirection.x > 0 ? 1 : -1;
            }
            else
            {
                selfDirection.x = 0;
                selfDirection.z = selfDirection.z > 0 ? 1 : -1;
            }
            Vector3Int targetPos = selfPosition + selfDirection;//����ǰ��һ�������
            int boxId = ChessboardSys.Instance.GetMatrixValue(targetPos.x, targetPos.y, targetPos.z);
            if (boxId != 0 && boxId < 2000000001)
            {
                box = chessboard.objsDic[boxId].GetComponent<Block>()
                    ? chessboard.objsDic[boxId].GetComponent<Block>()
                    : null;
                box.PushTo(selfDirection,0.2f);
            }
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public void Place()
        {
            //chessboard.GenerateNewBox_random();
        }
    }
}
