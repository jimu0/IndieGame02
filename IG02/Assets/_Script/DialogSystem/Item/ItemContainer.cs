using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemContainer : ScriptableData<ItemContainer>
{
    [SerializeField, SerializeReference]
    public GalScript Item = new GalScript();
    // -----------------------------------------------
    //  Ù–‘
    // -----------------------------------------------
}
