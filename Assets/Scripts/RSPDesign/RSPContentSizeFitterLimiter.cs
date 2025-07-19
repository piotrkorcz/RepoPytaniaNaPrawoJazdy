using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RSPContentSizeFitterLimiter : RSPObject
{
    [SerializeField] private bool width;
    [SerializeField] private bool height;
    [SerializeField] private float maximumWidth;
    [SerializeField] private float maximumHeight;
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    public override void Render()
    {
        if (gameObject.activeSelf)
            StartCoroutine(DelayedFit());
    }

    private IEnumerator DelayedFit()
    {
        contentSizeFitter.enabled = true;
        
        yield return null;
        
        if (targetRectTransform.rect.height > maximumHeight)
        {
            contentSizeFitter.enabled = false;
            targetRectTransform.sizeDelta = new Vector2(targetRectTransform.sizeDelta.x, maximumHeight);
        }

        verticalLayoutGroup.childForceExpandHeight = false;
        yield return null;
        verticalLayoutGroup.childForceExpandHeight = true;
    }

}
