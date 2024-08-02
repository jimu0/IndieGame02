using Add;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cube
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("每行的默认值")]
        public int DefaultValueOfLine;
        [Header("行尺寸")]
        public int SizeOfXZ;
        [Header("深度")]
        public int SizeOfY;

        [Header("初始化完毕的数据")]
        public ListInt3Var Data;

        [Header("CSV文件")]
        public TextAsset CsvFile;

        // Start is called before the first frame update
        void Start()
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
                Data.Value[Pha(content[1]), Pha(content[2]), Pha(content[3])] = Pha(content[4]);
                Totallycount++;
            }


            Debug.Log("读取到的数据规模 = " + Totallycount + "\n占总规模的比例 = " + Totallycount / (float)Data.Value.Length + "%");
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
