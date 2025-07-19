using UnityEngine;

public class RSPFillWidthWithAspectRatio : RSPObject
{
    [SerializeField] private float aspectRatio;
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private RectTransform baseRectTransform;

    public override void Render()
    {
        targetRectTransform.sizeDelta = new Vector2(baseRectTransform.rect.width, baseRectTransform.rect.width / aspectRatio);
    }
}
