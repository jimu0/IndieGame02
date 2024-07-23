using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoTurn : MonoBehaviour
{
    public ScriptListAssetData ScriptText;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(
        () => {
            TurnScript();
        });
    }

    public void TurnScript()
    {
        ContainerLoader.Instance.scriptContainer = ScriptText;
        ContainerLoader.Instance.isEnd = false;
        ContainerLoader.Instance.Index = 0;
        ContainerLoader.Instance.Push();
        ContainerLoader.Instance.Index++;
        Destroy(gameObject.transform.parent.gameObject);
    }
}
