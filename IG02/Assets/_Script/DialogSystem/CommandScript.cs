using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Data;
using static UnityEngine.GraphicsBuffer;

public class CommandScript : MonoBehaviour
{
    public static CommandScript Instance;
    public Image PanelBg;
    public Image PanelRole;
    public Image BtnGroup;
    public GameObject prefab;
    public GameObject BtnsPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowCho(List<Turn> turns)
    {
        var q = Instantiate(BtnsPrefab);
        q.transform.SetParent(BtnGroup.rectTransform);
        q.GetComponent<RectTransform>().localPosition = Vector3.zero;
        var chis = q.GetComponentsInChildren<Text>();
        for(int i = 0; i < chis.Length; i++)
        {
            if (i >= turns.Count)
            {
                Destroy(chis[i].transform.parent.gameObject);
                continue;
            }
            chis[i].text = turns[i].talk;
            chis[i].transform.parent.GetComponent<ChoTurn>().ScriptText = turns[i].TurnAsset;
        }
    }

    public void ShowImage(string target, float x = 0.5f, float y = 0.5f)
    {
        Texture2D tex = Resources.Load("Images/" + target) as Texture2D;
        if (!tex)
        {
            Debug.LogError("严重错误，无法读取图片---" + target);
        }
        
        if (target.StartsWith("role_"))
        {
            var role = GameObject.Find(target.Split(' ')[0]);
            if (role == null)
            {
                GameObject imgObj = Instantiate(prefab);
                imgObj.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                imgObj.GetComponent<Image>().SetNativeSize();
                imgObj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                imgObj.GetComponent<RectTransform>().SetParent(PanelRole.rectTransform);
                imgObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 * x - 1920 / 2, 1080 * y);
                imgObj.gameObject.name = target.Split(' ')[0];
                UIAnimationManager.instance.containers.Add(new UIAnimationContainer()
                {
                    duration = 0.2f,
                    AnimeName = target.Split(' ')[0],
                    target_transformed = imgObj.GetComponent<RectTransform>(),
                    transform_type = UITransformType.fadeIn,
                    target_speed_when_fadeinmode = 5f,
                });
            }
            else
            {
                role.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                role.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1920 * x - 1920 / 2, 1080 * y), 0.2f);
            }
            UIAnimationManager.instance.AnimationInvoke(target.Split(' ')[0]);
        }
        else
        {
            GameObject imgObj = Instantiate(prefab);
            imgObj.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            imgObj.GetComponent<Image>().SetNativeSize();
            imgObj.GetComponent<RectTransform>().SetParent(PanelBg.rectTransform);
            imgObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920 * x, 1080 * y);

            UIAnimationManager.instance.containers.Add(new UIAnimationContainer()
            {
                duration = 0.2f,
                AnimeName = target,
                target_transformed = imgObj.GetComponent<RectTransform>(),
                transform_type = UITransformType.fadeIn,
                target_speed_when_fadeinmode = 1.5f,
                
            });
            UIAnimationManager.instance.AnimationInvoke(target);
        }
    }
}
