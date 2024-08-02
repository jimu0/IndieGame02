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
        int IDOfReplace;
        ListInt3Var list;
        Transform targetTran;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(30);
            IDOfReplace = EditorGUILayout.IntField("��Ҫ�滻��Ŀ��ID", IDOfReplace);
            targetTran = EditorGUILayout.ObjectField("��Ҫ���õ�Ŀ��", targetTran, typeof(Transform), true) as Transform;
            list = EditorGUILayout.ObjectField("�����б�", list, typeof(ListInt3Var), false) as ListInt3Var;
            GUILayout.Space(5);
            if (GUILayout.Button("�滻ID", GUILayout.Height(30)))
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

