using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour
{
    public static UIAnimationManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<UIAnimationContainer> containers = new List<UIAnimationContainer>();
    
    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="rect"></param>
    public void AnimationInvoke(RectTransform rect)
    {
        var target = containers.FindAll(t => t.target_transformed == rect);
        if (target.Count == 0)
        {
            Debug.LogWarning("����ʧ��===" + rect.gameObject.name);
            return;
        }
            
        foreach(var i in target)
        {
            InvokAnime(i);
        }

    }

    /// <summary>
    /// ���±����
    /// </summary>
    public void AnimationInvoke(int index)
    {
        var target = containers[index];
        if (target == null || index > containers.Count - 1)
        {
            Debug.LogWarning("����ʧ�ܣ��±겻���ڡ�");
            return;
        }
        InvokAnime(target);
    }

    /// <summary>
    /// �����ֲ��ң��ɶ����
    /// </summary>
    /// <param name="names"></param>
    public void AnimationInvoke(string names)
    {
        var target = containers.FindAll(t => t.AnimeName == names);
        if (target.Count == 0)
        {
            Debug.LogWarning("����ʧ��===" + names);
            return;
        }
        foreach (var i in target)
        {
            InvokAnime(i);
        }
    }

    private void InvokAnime(UIAnimationContainer target)
    {
        if (target != null)
        {
            switch (target.transform_type)
            {
                case UITransformType.move:
                    StartCoroutine(MoveTarget(target, target.target_transformed.anchoredPosition));
                    break;
                case UITransformType.scale:
                    StartCoroutine(ScaleTarget(target, target.target_transformed.localScale));
                    break;
                case UITransformType.fadeIn:
                    StartCoroutine(TargetFadeIn(target));
                    break;
                case UITransformType.fadeOut:
                    StartCoroutine(TargetFadeOut(target));
                    break;
            }
        }
    }

    IEnumerator MoveTarget(UIAnimationContainer target, Vector2 initialPos)
    {

        float elapsedTime = 0;

        while (elapsedTime < target.duration)
        {
            float t = elapsedTime / target.duration * 2;
            target.target_transformed.anchoredPosition = Vector2.Lerp(initialPos, target.target_pos_when_scalemode, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.target_transformed.anchoredPosition = target.target_pos_when_scalemode;
        target.on_anime_finish.Invoke();
    }

    private IEnumerator ScaleTarget(UIAnimationContainer target, Vector3 initialScale)
    {
        var duration = target.duration / 2;
        // �Ŵ�
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            target.target_transformed.localScale = Vector3.Lerp(initialScale, target.target_scale_when_movemode, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ȷ���ﵽ�Ŵ��Ŀ������ֵ
        target.target_transformed.localScale = target.target_scale_when_movemode;

        // �ȴ�һ��ʱ��
        yield return new WaitForSeconds(0.001f);

        // ��С
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            target.target_transformed.localScale = Vector3.Lerp(target.target_scale_when_movemode, initialScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ȷ���ﵽ��С��Ŀ������ֵ
        target.target_transformed.localScale = initialScale;
        target.on_anime_finish.Invoke();
    }

    public IEnumerator TargetFadeIn(UIAnimationContainer target)
    {
        var canvas_group = target.target_transformed.GetComponent<CanvasGroup>();
        if (canvas_group == null)
        {
            canvas_group = target.target_transformed.AddComponent<CanvasGroup>();
        }

        while (canvas_group.alpha < 1)
        {
            canvas_group.alpha += target.target_speed_when_fadeinmode * Time.deltaTime;
            yield return null;
        }
        target.on_anime_finish.Invoke();
    }

    public IEnumerator TargetFadeOut(UIAnimationContainer target)
    {
        var canvas_group = target.target_transformed.GetComponent<CanvasGroup>();
        if (canvas_group == null)
        {
            canvas_group = target.target_transformed.AddComponent<CanvasGroup>();
        }

        while (canvas_group.alpha > 0)
        {
            canvas_group.alpha -= target.target_speed_when_fadeinmode * Time.deltaTime;
            yield return null;
        }
        target.on_anime_finish.Invoke();
    }
}
