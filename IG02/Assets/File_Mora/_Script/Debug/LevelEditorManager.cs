using Cube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DebugTools
{
    public class LevelEditorManager : MonoBehaviour
    {
        [Header("UI")]
        public Text CheckRes;
        public Text CheckRes2;

        public void CheckIDpos(InputField idif)
        {
            var lists = DataPool.instance.PosList.Value;
            var res = int.Parse(idif.transform.Find("value").GetComponent<Text>().text);
            for (int i = 0; i < lists.GetLength(0); i++)
            {
                for (int j = 0; j < lists.GetLength(1); j++)
                {
                    for(int k = 0; k < lists.GetLength(2); k++)
                    {
                        if (lists[i,j,k] == res)
                        {
                            CheckRes.text = (new Vector3(i,j,k)).ToString();
                        }
                    }
                }
            }
        }

        public void CheckPos(InputField idif)
        {
            var lists = DataPool.instance.PosList.Value;
            var res = (idif.transform.Find("value").GetComponent<Text>().text);
            var num = res.Replace(" ", "").Split(',');
            CheckRes2.text = lists[int.Parse(num[0]), int.Parse(num[1]), int.Parse(num[2])].ToString();
        }
    }
}
