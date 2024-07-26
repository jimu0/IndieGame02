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

    [MenuItem("编辑器/剧情编辑器")]
    /// <summary>
    /// 打开窗口
    /// </summary>
    public static void Open()
    {
        var window = EditorWindow.GetWindow<ItemEditor>("剧情编辑器");
        window.Initialize();
        window.Show();
    }


    /// <summary>
    /// 绘制
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
    /// 道具列表绘制
    /// </summary>
    void OnDrawEventList(float width, float height)
    {
        if (m_Edit == null)
        {
            using (new ColorScope(Color.red))
            {
                EditorGUILayout.LabelField("当前编辑资源无效");
            }
            return;
        }

        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width), GUILayout.Height(height));
        {
            using (new ColorScope(Color.green))
            {
                if (GUILayout.Button("新添脚本"))
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
                filenme = EditorGUILayout.TextField("文件名", filenme);
                for (var i = 0; i < m_Edit.List.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        using (new ColorScope(Color.red))
                        {
                            if (GUILayout.Button("×", GUILayout.Width(25)))
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
                            if (GUILayout.Button($"[剧情ID：<color=yellow>{m_Edit.List[i].Item.id:D8}</color>] [{m_Edit.List[i].Item.Name + " - " +  m_Edit.List[i].Item.Talk}]", btnStyle))
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
    /// 指令列表绘制
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
                    EditorGUILayout.LabelField("当前选择脚本无效");
                }
            }
            else
            {
                EditorGUILayout.LabelField("对话ID", CurrentData.id.ToString());
                CurrentData.id = m_Edit.List.FindIndex(t => t.Item == CurrentData);
                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField("基础信息", new GUIStyle(EditorStyles.boldLabel));
                CurrentData.Name = EditorGUILayout.TextField("说话者名字", CurrentData.Name);
                CurrentData.Talk = EditorGUILayout.TextField("说话内容", CurrentData.Talk, GUILayout.Height(50));
                EditorGUILayout.Space(20);
                string wast = EditorGUILayout.TextField("无意义字符", "");
                //CurrentData.icon = (Sprite)EditorGUILayout.ObjectField(CurrentData.icon, typeof(Sprite), false);
                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField("指令", new GUIStyle(EditorStyles.boldLabel));
                //EditorGUILayout.LabelField("血量增益");
                //CurrentData.hp_increase = EditorGUILayout.FloatField(CurrentData.hp_increase);
                //EditorGUILayout.LabelField("暴击率增益");
                //CurrentData.critical_rate = EditorGUILayout.FloatField(CurrentData.critical_rate);
                //EditorGUILayout.LabelField("暴击伤害增益");
                //CurrentData.critical_damage = EditorGUILayout.FloatField(CurrentData.critical_damage);

                using (new ColorScope(Color.green))
                {
                    if (GUILayout.Button("新添指令效果"))
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
                                    if(commandType == ScriptCommandType.图像)
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
                    EditorGUILayout.LabelField("图像指令系列", new GUIStyle(EditorStyles.boldLabel));
                    for (int q = 0; q < CurrentData.galCommandpics.Count; q++)
                    {
                        if(q > 0)
                        {
                            EditorGUILayout.Space(8);
                        }
                        EditorGUILayout.LabelField("指令" + q + "——");
                        CurrentData.galCommandpics[q].name = EditorGUILayout.TextField("图像资源名字", CurrentData.galCommandpics[q].name);
                        CurrentData.galCommandpics[q].positionX = EditorGUILayout.TextField("图像位置X", CurrentData.galCommandpics[q].positionX);
                        CurrentData.galCommandpics[q].positionY = EditorGUILayout.TextField("图像位置Y", CurrentData.galCommandpics[q].positionY);
                        if (GUILayout.Button("移出本条", GUILayout.Width(125)))
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
                    EditorGUILayout.LabelField("跳转指令系列", new GUIStyle(EditorStyles.boldLabel));
                    if (GUILayout.Button("增加跳转选项", GUILayout.Width(200)))
                    {
                        var turns = new Turn();
                        CurrentData.galCommandturns.turns.Add(turns);
                    }
                    foreach (var turn in CurrentData.galCommandturns.turns)
                    {
                        turn.TurnAsset = (ScriptListAssetData)EditorGUILayout.ObjectField("脚本资源名字", turn.TurnAsset, typeof(ScriptListAssetData), false);
                        turn.talk = EditorGUILayout.TextField("选项内容", turn.talk);
                        if (GUILayout.Button("移出本条", GUILayout.Width(125)))
                        {
                            CurrentData.galCommandturns.turns.Remove(turn);
                            break;
                        }
                    }
                    EditorGUILayout.Space(15);
                }




                GUILayout.Space(15);
                GUILayout.Label("对当前脚本操作：");
                m_CommandListScrollPos = EditorGUILayout.BeginScrollView(m_CommandListScrollPos);
                {

                        EditorGUILayout.BeginHorizontal();
                        {
                            var command = CurrentData;
                            using (new ColorScope(Color.red))
                            {
                                if (GUILayout.Button("×", GUILayout.Width(25)))
                                {
                                    m_Edit.List.RemoveAt(m_SelectListIndex);
                                    SetSelectCommand(Mathf.Clamp(m_SelectEventItemIndex, 0, m_Edit.List.Count - 1));
                                    GUIUtility.ExitGUI();
                                }
                            }

                            if (GUILayout.Button("↑", GUILayout.Width(25)))
                            {
                                m_Edit.MoveUp(m_SelectListIndex);
                                GUIUtility.ExitGUI();
                            }
                            if (GUILayout.Button("↓", GUILayout.Width(25)))
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
    /// 指令详细设置绘制
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
                    EditorGUILayout.LabelField("当前选择指令无效");
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
    /// 设置当前道具选择
    /// </summary>
    /// <param name="index"></param>
    void SetSelectEvent(int index)
    {
        m_SelectListIndex = index;
        SetSelectCommand(0);
    }

    /// <summary>
    /// 保存
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
    /// 是否可以保存
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
                EditorUtility.DisplayDialog("错误", "存在重复ID,保存失败", "OK ");
                return false;
            }

            existIdList.Add(d.Item.id);
        }

        return true;
    }

    /// <summary>
    /// 抬头
    /// </summary>
    void OnDrawHeader()
    {
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            if (GUILayout.Button("重载", GUILayout.Width(200)))
            {
                Load();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("保存", GUILayout.Width(200)))
            {
                Save();
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    /// <summary>
    /// 初期化
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
            Debug.LogError("原型资源为空");
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
    /// 设置当前指令选择
    /// </summary>
    /// <param name="index"></param>
    void SetSelectCommand(int index)
    {
        m_SelectEventItemIndex = index;
    }
    /// <summary>
    /// 当前道具
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
    图像
}