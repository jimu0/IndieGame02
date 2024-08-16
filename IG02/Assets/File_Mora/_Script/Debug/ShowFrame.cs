using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugTools
{
    public class ShowFrame : MonoBehaviour
    {

        private float m_LastTime;
        private float m_UpdateInterval = 0.5f;
        private float m_Frames;
        float fps;

        void Start()
        {
            m_Frames = 0;
            m_LastTime = Time.realtimeSinceStartup;
        }

        void Update()
        {
            m_Frames++;
            float curTime = Time.realtimeSinceStartup;
            // 每0.5s更新一次
            if (curTime > m_LastTime + m_UpdateInterval)
            {
                fps = m_Frames / (curTime - m_LastTime);
            }
        }

        private void OnGUI()
        {
            string text = fps.ToString();
            var style = new GUIStyle();
            style.fontSize = 30;
            // 使用Label显示文本
            GUI.Label(new Rect(40, 40, 400, 40), text, style);

            // 使用TextField显示可编辑的文本
            //text = GUI.TextField(new Rect(10, 40, 400, 40), text);
        }
    }
}
