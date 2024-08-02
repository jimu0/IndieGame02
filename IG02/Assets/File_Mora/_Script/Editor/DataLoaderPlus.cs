using Add;
using Cube;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{

    [CustomEditor(typeof(LevelLoader), true)]
    public class DataLoaderPlus : Editor
    {
        LevelLoader loader;
        int IDOfReplace;
        ListInt3Var list;
        Transform targetTran;
        Vector3 targetVector = -Vector3Int.one;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(30);
            GUILayout.Label("�༭���ֶ���ʼ��ʱѡ����");
            loader = EditorGUILayout.ObjectField("�����б�", loader, typeof(LevelLoader), true) as LevelLoader;
            if (GUILayout.Button("�ֶ���ʼ��", GUILayout.Height(30)))
            {
                loader.Init();
            }

            GUILayout.Space(30);
            IDOfReplace = EditorGUILayout.IntField("��Ҫ�滻��Ŀ��ID", IDOfReplace);
            list = EditorGUILayout.ObjectField("�����б�", list, typeof(ListInt3Var), false) as ListInt3Var;
            GUILayout.Space(10);
            GUILayout.Label("����2ѡ1��");
            targetTran = EditorGUILayout.ObjectField("��Ҫ���õ�Ŀ��", targetTran, typeof(Transform), true) as Transform;
            targetVector = EditorGUILayout.Vector3Field("Ŀ������", targetVector);
            GUILayout.Space(10);
            if (targetTran != null && GUILayout.Button("��ѡ�������滻ID", GUILayout.Height(30)))
            {
                Vector3Int fr = new Vector3Int(Round(targetTran.position.x), Round(targetTran.position.y), (Round(targetTran.position.z)));
                UpdateVar(fr.x, fr.y, fr.z, IDOfReplace);
                if (list.Value[fr.x, fr.y, fr.z] == IDOfReplace)
                {
                    Debug.Log("�滻�ɹ� :> " + IDOfReplace + "    λ�� = " + fr);
                }
                else
                {
                    Debug.Log("�滻δ��� :<" + "  λ�� = " + fr);
                }
            }
            GUILayout.Space(10);
            if (targetVector != -Vector3Int.one && GUILayout.Button("��ѡ�������滻ID", GUILayout.Height(30)))
            {
                Vector3Int fr = new Vector3Int(Round(targetVector.x), Round(targetVector.y), (Round(targetVector.z)));
                UpdateVar(fr.x, fr.y, fr.z, IDOfReplace);
                if (list.Value[fr.x, fr.y, fr.z] == IDOfReplace)
                {
                    Debug.Log("�滻�ɹ� :> " + IDOfReplace + "    λ�� = " + fr);
                }
                else
                {
                    Debug.Log("�滻δ��� :<" + "  λ�� = " + fr);
                }
            }
        }

        void UpdateVar(int x, int y, int z, int value)
        {
            list.Value[x, y, z] = value;
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

