using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RSPForceContentSizeFitter : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;

    private void OnEnable()
    {
        StartCoroutine(DelayedFit());
    }

    private IEnumerator DelayedFit()
    {
        horizontalLayoutGroup.childForceExpandWidth = false;
        yield return null;
        horizontalLayoutGroup.childForceExpandWidth = true;
    }

}
