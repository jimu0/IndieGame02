using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UIAnimationContainer
{
    public string AnimeName;
    [Header("����Ŀ�����")]
    public RectTransform target_transformed;
    [Header("����ʱ��")]
    public float duration;
    [Header("�ƶ�ʱ��������")]
    public Vector2 target_scale_when_movemode;
    [Header("����ʱ��������")]
    public Vector2 target_pos_when_scalemode;

    [Header("����ʱ��������")]
    public float target_speed_when_fadeinmode = 1;
    [Header("�任����")]
    public UITransformType transform_type;

    public UnityEvent on_anime_finish = new UnityEvent();
}
public enum UITransformType
{
    move, scale, fadeIn, fadeOut
}