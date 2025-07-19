using EditorAttributes;
using UnityEngine;
using UnityEngine.UI;

public class CLRModeImage : CLRModeObject
{
    [SerializeField, ShowField(nameof(useSprite))] private Sprite lightSprite;
    [SerializeField, ShowField(nameof(useSprite))] private Sprite darkSprite;
    [SerializeField] private Image image;

    protected override void Render(bool isLight)
    {
        if (useSprite)
            image.sprite = isLight ? lightSprite : darkSprite;
        else
            image.color = isLight ? lightColor : darkColor;
    }

}
