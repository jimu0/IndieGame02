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
            
            // ����һ������������̸�
            for (int x = 0; x < xyzSize.x; x++)
            {
                for (int z = 0; z < xyzSize.z; z++)
                {
                    GameObject baseTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    baseTile.transform.position = new Vector3(x, 0, z);
                    baseTile.transform.localScale = new Vector3(1, 0.1f, 1);
                    baseTile.GetComponent<Renderer>().material.color = Color.gray; // ���õײ���ɫ
                    baseTile.transform.parent = transform; // ���ײ㷽������Ϊ���̵��Ӷ���
                }
            }
        }
    }
}
