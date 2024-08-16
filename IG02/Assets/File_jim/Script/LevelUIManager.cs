
using UnityEngine;
using TMPro;
using UITemplate;

namespace File_jim.Script
{
    public class LevelUIManager : MonoBehaviour
    {
        public TMP_Text tmpTextStar;
        public TMP_Text tmpTextScore;
        private int starValue = 0;
        private int scoreValue = 0;
        private bool[] starBool = new bool [7];

        public void OnSetStarValue(int v)
        {
            //starValue += v;
            starValue = 0;
            for (int i = 0; i < starBool.Length; i++)
            {
                if(starBool[i])starValue++;
            }
            tmpTextStar.text = $"{starValue}/3";
        }
        
        public void OnSetScoreValue(int v)
        {
            scoreValue += v;
            tmpTextScore.text = scoreValue.ToString();
        }
        public void OnSetScoreValueEmpty()
        {
            scoreValue = 0;
            tmpTextScore.text ="0";
        }

        public void SetStarBool(int i, bool b)
        {
            starBool[i] = b;
        }

        public bool GetStarBool(int i)
        {
            return starBool[i];
        }

        public void Help()
        {
            UIManager.Instance.messageAppear("Move: W S A D/Up Nn Lt Rt \nJump: J/Space\nPush: H/F/LeftAlt");
        }

        public void Producer()
        {
            UIManager.Instance.messageAppear("producer:\nJim\nFangCheng\nMora\nLee\n.\nYuWei\nKidx");
        }
    }
}
