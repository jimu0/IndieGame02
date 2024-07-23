using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GalScript
{
    public int id;
    /// <summary>
    /// 说话人名字
    /// </summary>
    [Header("说话人姓名")]
    public string Name;
    /// <summary>
    /// 说胡内容
    /// </summary>
    [Header("对话内容")]
    [TextArea]
    public string Talk;

    public List<GalCommandPicture> galCommandpics = new List<GalCommandPicture>();
    public GalCommandTurn galCommandturns = new GalCommandTurn();
}

[Serializable]
public class GalCommandPicture
{
    public string name;
    public string positionX;
    public string positionY;
}

[Serializable]
public class GalCommandTurn
{
    public List<Turn> turns = new List<Turn>();
}

[Serializable]
public class Turn
{
    public string talk;
    public ScriptListAssetData TurnAsset;
}