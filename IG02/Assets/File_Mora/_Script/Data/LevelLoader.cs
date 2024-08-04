using Add;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cube
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject Prefab;
        public static LevelLoader Instance;
        [Header("�����ʹ�ù��༭�����ֶ���ʼ����������Թ�ѡ������")]
        public bool DisableAutoInit;
        [Header("ÿ�е�Ĭ��ֵ")]
        public int DefaultValueOfLine;
        [Header("�гߴ�")]
        public int SizeOfXZ;
        [Header("���")]
        public int SizeOfY;

        [Header("��ʼ����ϵ�����")]
        public ListInt3Var Data;

        [Header("CSV�ļ�")]
        public TextAsset CsvFile;

        private int ID;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            Init();

            GenerateCube();
        }

        public void Init()
        {
            Data.Value = new int[SizeOfXZ, SizeOfY, SizeOfXZ];

            for (int i = 0; i < SizeOfXZ; i++)
            {
                for (int j = 0; j < SizeOfY; j++)
                {
                    for (int k = 0; k < SizeOfXZ; k++)
                    {
                            Data.Value[i, j, k] = DefaultValueOfLine;
                    }
                }
            }

            var Totallycount = 0;
            var csvdatas = CsvFile.ToString();

            List<string> line = csvdatas.ToString().Split('\n').ToList();
            line.RemoveAt(0);
            foreach (string s in line)
            {
                var content = s.ToString().Split(',').ToList();

                if (int.TryParse(content[0], out int res) == false || content.Count < 5)
                {
                    continue;
                }
                ID++;
                if (Pha(content[4]) == 0)
                    continue;
                Data.Value[Pha(content[1]), Pha(content[2]), Pha(content[3])] = Pha(content[4]) * 10000 + ID;
                Totallycount++;
            }


            Debug.Log("��ȡ�������ݹ�ģ = " + Totallycount + "\nռ�ܹ�ģ�ı��� = " + Totallycount / (float)Data.Value.Length + "%");
        }

        public void GenerateCube()
        {
            for(int i = 0; i < Data.Value.GetLength(0); i++)
            {
                for (int j = 0; j < Data.Value.GetLength(1); j++)
                {
                    for(int q = 0; q < Data.Value.GetLength(2); q++)
                    {
                        if (Data.Value[i, j, q] != 0)
                        {
                            var go = Instantiate(Prefab, new Vector3(i, j, q), Quaternion.identity);
                            go.GetComponent<CubeEntry>().ID = Data.Value[i, j, q];
                        }
                    }
                }
            }
        }

        public int Pha(string line)
        {
            int.TryParse(line, out int res);
            return res;
        }

        private void InitData()
        {

        }
    }
}
