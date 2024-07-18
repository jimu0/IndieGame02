using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace UITemplate
{
    public class MiniGamesButton : MonoBehaviour
    {
        public TextMeshProUGUI levelName;
        public string sceneName;
        public int id;
        void Start()
        {
            levelName.text = this.gameObject.name;
            if (UIManager.Instance.mainFont != null)
            {
                levelName.font = UIManager.Instance.mainFont;
            }
            levelName.color = UIManager.Instance.textColor;
        }

        public void LoadThisGame()
        {
            UIManager.Instance.loadGame(id);
            UIManager.Instance.clickSound();
        }
    }
}