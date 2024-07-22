using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemBuffData
{
    public ItemBuffType buff_type;
    public float hp_influence;
    public float vemp_influence;
    public float cd_influence;
    public float critical_rate;
    public float critical_damage;
}
public enum ItemBuffType
{
    hp, vemp, cd, critical_rate, critical_damage,
}