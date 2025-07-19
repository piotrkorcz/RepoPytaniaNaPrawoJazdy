using UnityEngine;

public class RSPCopySizeFromObject : RSPObject
{
    [SerializeField] private bool width;
    [SerializeField] private bool height;
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private RectTransform baseRectTransform;

    public override void Render()
    {
        targetRectTransform.sizeDelta = new Vector2(
            width ? baseRectTransform.rect.width : targetRectTransform.sizeDelta.x,
            height ? baseRectTransform.rect.height : targetRectTransform.sizeDelta.y
        );    
    }

}
