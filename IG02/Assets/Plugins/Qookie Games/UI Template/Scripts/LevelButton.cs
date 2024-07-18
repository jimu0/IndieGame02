using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace UITemplate
{
    public class LevelButton : MonoBehaviour
    {
        public TextMeshProUGUI levelTextNumber;
        void Start()
        {
            levelTextNumber.text = this.gameObject.name;
            if (UIManager.Instance.mainFont != null)
            {
                levelTextNumber.font = UIManager.Instance.mainFont;
            }
            levelTextNumber.color = UIManager.Instance.textColor;
        }

        public void LoadThisLevel()
        {
            UIManager.Instance.loadLevel(int.Parse(this.gameObject.name));
            UIManager.Instance.clickSound();
        }
    }
}