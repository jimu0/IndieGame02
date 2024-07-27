using UnityEngine;

namespace UITemplate.File_jim.Script
{
    public class Chessboard : MonoBehaviour
    {
        public Vector3Int xyzSize = new (8,99,8); // ���̵Ĵ�С��Ĭ��Ϊ8*99*8
        public GameObject boxPrefab; // �����Ԥ����
        private GameObject[,,] board;

        void Start()
        {
            board = new GameObject[xyzSize.x, xyzSize.y, xyzSize.z];
            //UIManager.Instance.completeLevel(true);
        }
        
        
        void Update()
        {
            // ����Ƿ��� 'v' ��
            if (Input.GetKeyDown(KeyCode.V))
            {
                AddRandomBoxLayer();
            }
        }
        
        void AddRandomBoxLayer()
        {
            // �������һ���µķ����
            for (int x = 0; x < xyzSize.x; x++)
            {
                for (int z = 0; z < xyzSize.z; z++)
                {
                    if (Random.value > 0.8f) // 20% �������ɷ���
                    {
                        // �ҵ���ǰ�����е����λ��
                        int y = FindHighestPosition(x, z);
                        // �����·���
                        GameObject newBox = Instantiate(boxPrefab, new Vector3(x, y, z), Quaternion.identity);
                        newBox.transform.parent = transform; // ���·�������Ϊ���̵��Ӷ���
                        board[x, y, z] = newBox; // �������̸��ӵķ�����Ϣ
                    }
                }
            }
        }

        int FindHighestPosition(int x, int z)
        {
            for (int y = 0; y < xyzSize.y; y++)
            {
                if (board[x, y, z] == null)
                {
                    return y;
                }
            }
            return xyzSize.y - 1; // �������λ�ö���ռ�ã��������λ��
        }
    }
}
