using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBtnShown : MonoBehaviour
{
    [SerializeField] private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
    private int groupIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Shown());
    }

    // Update is called once per frame
    IEnumerator Shown()
    {
        while (groupIndex < canvasGroups.Count) 
        {
            while (canvasGroups[groupIndex].alpha < 1)
            {
                canvasGroups[groupIndex].alpha += 2.6f * Time.deltaTime;
                yield return null;
            }
            groupIndex++;
            yield return null;
        }
    }
}
