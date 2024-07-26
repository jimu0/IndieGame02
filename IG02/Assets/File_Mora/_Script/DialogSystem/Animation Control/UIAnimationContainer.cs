using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UIAnimationContainer
{
    public string AnimeName;
    [Header("放入目标组件")]
    public RectTransform target_transformed;
    [Header("持续时间")]
    public float duration;
    [Header("移动时调整此项")]
    public Vector2 target_scale_when_movemode;
    [Header("缩放时调整此项")]
    public Vector2 target_pos_when_scalemode;

    [Header("渐变时调整此项")]
    public float target_speed_when_fadeinmode = 1;
    [Header("变换种类")]
    public UITransformType transform_type;

    public UnityEvent on_anime_finish = new UnityEvent();
}
public enum UITransformType
{
    move, scale, fadeIn, fadeOut
}