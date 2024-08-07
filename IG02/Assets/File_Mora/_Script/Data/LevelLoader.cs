using Add;
using File_jim.Script;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Cube
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject Prefab;
        public static LevelLoader Instance;
        [Header("如果你使用过编辑器内手动初始化，务必亲自勾选！！！")]
        public bool DisableAutoInit;
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


            Debug.Log("读取到的数据规模 = " + Totallycount + "\n占总规模的比例 = " + Totallycount / (float)Data.Value.Length + "%");
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

        /// <summary>
        /// 保存为csv文件
        /// </summary>
        /// <param name="sizeMsg">地图的行尺寸</param>
        /// <param name="fileName">文件名</param>
        /// <param name="boxs">（可选）当前存在的box实体</param>
        /// <returns></returns>
        public IEnumerator SaveCSVFile(string sizeMsg, string fileName, List<BoxMovement> boxs = null)
        {
            string filePath = Application.persistentDataPath + "/" + fileName;
            // 使用File.Create方法创建文件
            var file = File.Create(filePath);
            file.Close();
            if (boxs == null)
            {
                boxs = FindObjectsOfType<BoxMovement>().ToList();
            }

            File.AppendAllText(filePath, "num,x,y,z,id" + "\n", Encoding.UTF8);
            int ind = 0;

            var size = new Vector3Int(int.Parse(sizeMsg.Substring(1, 2)), int.Parse(sizeMsg.Substring(3, 2)), int.Parse(sizeMsg.Substring(5, 2)));

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int q = 0; q < size.z; q++)
                    {
                        var target = boxs.Find(t => (Round(t.transform.position.x) == i) &&
                        (Round(t.transform.position.y) == j) && Round(t.transform.position.z) == q);

                        if (target != null)
                        {
                            File.AppendAllText(filePath, $"{ind},{i},{j},{q},{(target.id / 10000)}" + "\n");
                        }
                        else
                        {
                            File.AppendAllText(filePath, $"{ind},{i},{j},{q},{0}" + "\n", Encoding.UTF8);
                        }
                        ind++;
                    }
                    yield return null;
                }
                yield return null;
            }
            Debug.Log("save success -- " + filePath);
        }

        public void TestSave(string fileName)
        {
            StartCoroutine(SaveCSVFile("1070707", fileName));
        }
        public int Round(float value, int digits = 0)
        {
            if (value == 0)
            {
                return 0;
            }
            float multiple = Mathf.Pow(10, digits);
            float tempValue = value > 0 ? value * multiple + 0.5f : value * multiple - 0.5f;
            tempValue = Mathf.FloorToInt(tempValue);
            return (int)(tempValue / multiple);
        }
    }
}
