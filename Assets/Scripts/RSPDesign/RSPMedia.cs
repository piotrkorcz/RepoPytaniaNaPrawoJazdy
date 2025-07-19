using UnityEngine;

public class RSPMedia : RSPObject
{
    private const float ASPECT_RATIO = 16f / 9f;

    [SerializeField] private float maximumHeight;
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private RectTransform baseRectTransform;

    public override void Render()
    {
        Vector2 sizeDelta = new Vector2(maximumHeight * ASPECT_RATIO, maximumHeight);

        if (sizeDelta.x > baseRectTransform.rect.width)
            sizeDelta = new Vector2(baseRectTransform.rect.width, baseRectTransform.rect.width / ASPECT_RATIO);

        targetRectTransform.sizeDelta = sizeDelta;
    }
}
