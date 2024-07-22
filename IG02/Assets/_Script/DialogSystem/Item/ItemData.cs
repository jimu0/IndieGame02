using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string Name;
    [TextArea()]
    public string Description;
    public int item_id;
    public Sprite Icon;
    public List<ItemBuffData> buffDatas = new();

    public virtual void OnDraw()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
