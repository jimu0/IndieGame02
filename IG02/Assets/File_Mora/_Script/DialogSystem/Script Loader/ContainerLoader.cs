using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContainerLoader : MonoBehaviour
{
    public static ContainerLoader Instance;
    public ScriptListAssetData scriptContainer;
    public TW_MultiStrings_Regular tWRegular;
    public Text SpeakerName;
    [HideInInspector] public int Index = 0;

    public GameObject ChoiseBtnGroup;
    public GameObject ChoisePrafeb;

    public CanvasGroup cvgroup;
    [HideInInspector] public bool isEnd;
    [HideInInspector] public bool isFirstStart;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Index == 0 && !isEnd && !isFirstStart)
        {
            isFirstStart = true;
            Push();
            Index++;
        }

        if (tWRegular.isStop == true && Input.GetMouseButtonDown(0) && Index < scriptContainer.List.Count)
        {
            Push();
            Index++;
        }
        else if (tWRegular.isStop == false && Index < scriptContainer.List.Count - 1 && Input.GetMouseButtonDown(0))//skip¿ÉÒÔÓÃisstartÅÐ¶Ï
        {
            tWRegular.SkipTypewriter();
        }
    }


    public void Push()
    {
        if (isEnd)
        { Index = 0; return; }
        if(Index < scriptContainer.List.Count)
        {
            SpeakerName.text = scriptContainer.List[Index].Item.Name;

            tWRegular.MultiStrings[0] = scriptContainer.List[Index].Item.Talk;

            tWRegular.NextString();

            if (scriptContainer.List[Index].Item.galCommandpics.Count > 0)
            {
                foreach (var command in scriptContainer.List[Index].Item.galCommandpics)
                {
                    CommandScript.Instance.ShowImage(command.name,
                    float.Parse(command.positionX),
                    float.Parse(command.positionY));
                }
            }


            if (scriptContainer.List[Index].Item.galCommandturns.turns.Count > 0 && !isEnd)
            {
                CommandScript.Instance.ShowCho(scriptContainer.List[Index].Item.galCommandturns.turns);
                Index++;
                Push();
                isEnd = true;
            }
        }

    }
}
