using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemEditor : EditorWindow
{
    ScriptListAssetData itemData;
    private ScriptListAssetData m_Edit;
    private int m_SelectListIndex;
    private int m_SelectEventItemIndex;
    private Vector2 m_ListScrollPos;
    private Vector2 m_CommandListScrollPos;
    string filenme = "Script 1";

    [MenuItem("�༭��/����༭��")]
    /// <summary>
    /// �򿪴���
    /// </summary>
    public static void Open()
    {
        var window = EditorWindow.GetWindow<ItemEditor>("����༭��");
        window.Initialize();
        window.Show();
    }


    /// <summary>
    /// ����
    /// </summary>
    public void OnGUI()
    {
        OnDrawHeader();
        EditorGUILayout.BeginHorizontal();
        {
            OnDrawEventList(400, 800);

            OnDrawEventCommandList(600, 800);

            OnDrawCommandDetail(400, 800);
        }
        EditorGUILayout.EndHorizontal();
    }


    /// <summary>
    /// �����б����
    /// </summary>
    void OnDrawEventList(float width, float height)
    {
        if (m_Edit == null)
        {
            using (new ColorScope(Color.red))
            {
                EditorGUILayout.LabelField("��ǰ�༭��Դ��Ч");
            }
            return;
        }

        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.Height(height));
        {
            using (new ColorScope(Color.green))
            {
                if (GUILayout.Button("����ű�"))
                {
                    m_Edit.List.Add(new ItemContainer());
                    SetSelectEvent(m_Edit.List.Count - 1);
                    SetSelectCommand(m_Edit.List.Count - 1);
                    GUIUtility.ExitGUI();
                }
            }

            m_ListScrollPos = EditorGUILayout.BeginScrollView(m_ListScrollPos);
            {
                var btnStyle = new GUIStyle(EditorStyles.miniButton) { alignment = TextAnchor.MiddleLeft, richText = true };
                filenme = EditorGUILayout.TextField("�ļ���", filenme);
                for (var i = 0; i < m_Edit.List.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        using (new ColorScope(Color.red))
                        {
                            if (GUILayout.Button("��", GUILayout.Width(25)))
                            {
                                m_Edit.List.RemoveAt(i);
                                SetSelectEvent(Mathf.Clamp(m_SelectListIndex, 0, m_Edit.List.Count - 1));
                                GUIUtility.ExitGUI();
                            }
                        }

                        var isSelect = m_SelectListIndex == i;
                        var color = isSelect ? Color.cyan : Color.white;

                        using (new ColorScope(color))
                        {
                            if (GUILayout.Button($"[����ID��<color=yellow>{m_Edit.List[i].Item.id:D8}</color>] [{m_Edit.List[i].Item.Name + " - " +  m_Edit.List[i].Item.Talk}]", btnStyle))
                            {
                                SetSelectEvent(i);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// ָ���б����
    /// </summary>
    /// <param name="width"></param>
    void OnDrawEventCommandList(float width, float height)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.Height(height));
        {
            if (CurrentData == null)
            {
                using (new ColorScope(Color.red))
                {
                    EditorGUILayout.LabelField("��ǰѡ��ű���Ч");
                }
            }
            else
            {
                EditorGUILayout.LabelField("�Ի�ID", CurrentData.id.ToString());
                CurrentData.id = m_Edit.List.FindIndex(t => t.Item == CurrentData);
                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField("������Ϣ", new GUIStyle(EditorStyles.boldLabel));
                CurrentData.Name = EditorGUILayout.TextField("˵��������", CurrentData.Name);
                CurrentData.Talk = EditorGUILayout.TextField("˵������", CurrentData.Talk, GUILayout.Height(50));
                EditorGUILayout.Space(20);
                string wast = EditorGUILayout.TextField("�������ַ�", "");
                //CurrentData.icon = (Sprite)EditorGUILayout.ObjectField(CurrentData.icon, typeof(Sprite), false);
                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField("ָ��", new GUIStyle(EditorStyles.boldLabel));
                //EditorGUILayout.LabelField("Ѫ������");
                //CurrentData.hp_increase = EditorGUILayout.FloatField(CurrentData.hp_increase);
                //EditorGUILayout.LabelField("����������");
                //CurrentData.critical_rate = EditorGUILayout.FloatField(CurrentData.critical_rate);
                //EditorGUILayout.LabelField("�����˺�����");
                //CurrentData.critical_damage = EditorGUILayout.FloatField(CurrentData.critical_damage);

                using (new ColorScope(Color.green))
                {
                    if (GUILayout.Button("����ָ��Ч��"))
                    {
                        var menu = new GenericMenu();
                        var commads = System.Enum.GetValues(typeof(ScriptCommandType));
                        var commandNames = EnumLabel.GetEnumLabels<ScriptCommandType>();

                        if(commads.Length == 0)
                        {
                            CurrentData.galCommandpics.Add(new GalCommandPicture());
                        }

                        for (var i = 0; i < commads.Length; ++i)
                        {
                            var commandType = (ScriptCommandType)commads.GetValue(i);
                            menu.AddItem
                            (
                                new GUIContent($"{EnumLabel.GetEnumLabel(commandType)}"),
                                false,
                                () =>
                                {
                                    if(commandType == ScriptCommandType.ͼ��)
                                    {
                                        CurrentData.galCommandpics.Add(new GalCommandPicture());
                                    }
                                }
                            );
                        }
                        menu.ShowAsContext();
                    }
                }



                if (CurrentData.galCommandpics != null && CurrentData.galCommandpics.Count > 0)
                {
                    EditorGUILayout.Space(30);
                    EditorGUILayout.LabelField("ͼ��ָ��ϵ��", new GUIStyle(EditorStyles.boldLabel));
                    for (int q = 0; q < CurrentData.galCommandpics.Count; q++)
                    {
                        if(q > 0)
                        {
                            EditorGUILayout.Space(8);
                        }
                        EditorGUILayout.LabelField("ָ��" + q + "����");
                        CurrentData.galCommandpics[q].name = EditorGUILayout.TextField("ͼ����Դ����", CurrentData.galCommandpics[q].name);
                        CurrentData.galCommandpics[q].positionX = EditorGUILayout.TextField("ͼ��λ��X", CurrentData.galCommandpics[q].positionX);
                        CurrentData.galCommandpics[q].positionY = EditorGUILayout.TextField("ͼ��λ��Y", CurrentData.galCommandpics[q].positionY);
                        if (GUILayout.Button("�Ƴ�����", GUILayout.Width(125)))
                        {
                            CurrentData.galCommandpics.Remove(CurrentData.galCommandpics[q]);
                            break;
                        }
                        EditorGUILayout.Space(8);
                    }
                    EditorGUILayout.Space(15);
                }


                if (CurrentData.galCommandturns != null)
                {
                    EditorGUILayout.Space(15);
                    EditorGUILayout.LabelField("��תָ��ϵ��", new GUIStyle(EditorStyles.boldLabel));
                    if (GUILayout.Button("������תѡ��", GUILayout.Width(200)))
                    {
                        var turns = new Turn();
                        CurrentData.galCommandturns.turns.Add(turns);
                    }
                    foreach (var turn in CurrentData.galCommandturns.turns)
                    {
                        turn.TurnAsset = (ScriptListAssetData)EditorGUILayout.ObjectField("�ű���Դ����", turn.TurnAsset, typeof(ScriptListAssetData), false);
                        turn.talk = EditorGUILayout.TextField("ѡ������", turn.talk);
                        if (GUILayout.Button("�Ƴ�����", GUILayout.Width(125)))
                        {
                            CurrentData.galCommandturns.turns.Remove(turn);
                            break;
                        }
                    }
                    EditorGUILayout.Space(15);
                }




                GUILayout.Space(15);
                GUILayout.Label("�Ե�ǰ�ű�������");
                m_CommandListScrollPos = EditorGUILayout.BeginScrollView(m_CommandListScrollPos);
                {

                        EditorGUILayout.BeginHorizontal();
                        {
                            var command = CurrentData;
                            using (new ColorScope(Color.red))
                            {
                                if (GUILayout.Button("��", GUILayout.Width(25)))
                                {
                                    m_Edit.List.RemoveAt(m_SelectListIndex);
                                    SetSelectCommand(Mathf.Clamp(m_SelectEventItemIndex, 0, m_Edit.List.Count - 1));
                                    GUIUtility.ExitGUI();
                                }
                            }

                            if (GUILayout.Button("��", GUILayout.Width(25)))
                            {
                                m_Edit.MoveUp(m_SelectListIndex);
                                GUIUtility.ExitGUI();
                            }
                            if (GUILayout.Button("��", GUILayout.Width(25)))
                            {
                                m_Edit.MoveDown(m_SelectListIndex);
                                GUIUtility.ExitGUI();
                            }

                            //var isSelect = m_SelectEventCommandIndex == i;
                            //var color = isSelect ? Color.cyan : Color.white;
                            //using (new ColorScope(color))
                            //{
                            //    if (GUILayout.Button($"{EnumLabel.GetEnumLabel(command.Name)}", GUILayout.Width(width - 120)))
                            //    {
                            //        SetSelectCommand(i);
                            //    }
                            //}
                        
                        EditorGUILayout.EndHorizontal();

                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// ָ����ϸ���û���
    /// </summary>
    /// <param name="width"></param>
    void OnDrawCommandDetail(float width, float height)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.Height(height));
        {
            if (m_Edit.List == null)
            {
                using (new ColorScope(Color.red))
                {
                    EditorGUILayout.LabelField("��ǰѡ��ָ����Ч");
                }
            }
            else
            {
                //cu.OnDraw();
            }
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// ���õ�ǰ����ѡ��
    /// </summary>
    /// <param name="index"></param>
    void SetSelectEvent(int index)
    {
        m_SelectListIndex = index;
        SetSelectCommand(0);
    }

    /// <summary>
    /// ����
    /// </summary>
    void Save()
    {
        if (IsEnableSave(m_Edit))
        {
            itemData.CopyFrom(m_Edit);
            itemData.List = new List<ItemContainer>();
            foreach (var item in m_Edit.List)
            {
                itemData.List.Add(item);
            }

            EditorUtility.SetDirty(itemData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    /// <summary>
    /// �Ƿ���Ա���
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    bool IsEnableSave(ScriptListAssetData data)
    {
        var existIdList = new List<int>();
        for (var i = 0; i < data.List.Count; ++i)
        {
            var d = data.List[i];
            d.Id = d.Item.id;
            if (existIdList.Contains(d.Item.id))
            {
                EditorUtility.DisplayDialog("����", "�����ظ�ID,����ʧ��", "OK ");
                return false;
            }

            existIdList.Add(d.Item.id);
        }

        return true;
    }

    /// <summary>
    /// ̧ͷ
    /// </summary>
    void OnDrawHeader()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            if (GUILayout.Button("����", GUILayout.Width(200)))
            {
                Load();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("����", GUILayout.Width(200)))
            {
                Save();
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    /// <summary>
    /// ���ڻ�
    /// </summary>
    void Initialize()
    {
        Load();
    }

    void Load()
    {
        itemData = ScriptListAssetData.ResourceLoad(filenme);
        if (itemData == null)
        {
            Debug.LogError("ԭ����ԴΪ��");
            return;
        }
        m_Edit = ScriptableObject.CreateInstance<ScriptListAssetData>();
        m_Edit.CopyFrom(itemData);

        m_Edit.List = new List<ItemContainer>();
        foreach (var item in itemData.List)
        {
            m_Edit.List.Add(item);
        }

        SetSelectEvent(0);
    }

    /// <summary>
    /// ���õ�ǰָ��ѡ��
    /// </summary>
    /// <param name="index"></param>
    void SetSelectCommand(int index)
    {
        m_SelectEventItemIndex = index;
    }
    /// <summary>
    /// ��ǰ����
    /// </summary>
    GalScript CurrentData
    {
        get
        {
            if (m_Edit == null)
            {
                return null;
            }

            if (m_SelectListIndex > m_Edit.List.Count - 1)
            {
                return null;
            }

            if (m_SelectListIndex < 0)
            {
                return null;
            }

            return m_Edit.List[m_SelectListIndex].Item;
        }
    }
}

public enum ScriptCommandType
{
    ͼ��
}