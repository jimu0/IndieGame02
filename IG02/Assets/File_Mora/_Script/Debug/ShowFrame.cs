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
            // ÿ0.5s����һ��
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
            // ʹ��Label��ʾ�ı�
            GUI.Label(new Rect(40, 40, 400, 40), text, style);

            // ʹ��TextField��ʾ�ɱ༭���ı�
            //text = GUI.TextField(new Rect(10, 40, 400, 40), text);
        }
    }
}
