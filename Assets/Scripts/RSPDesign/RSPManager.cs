using System;
using System.Collections;
using UnityEngine;

public class RSPManager : MonoBehaviour
{
    private static RSPManager instance;
    public static RSPManager Instance { get { return instance; } }

    public const int DEFAULT_WIDTH = 1440;
    public const int DEFAULT_HEIGHT = 3200;

    public static event Action<int, int> OnWindowResize;

    private int lastWidth;
    private int lastHeight;

    void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        instance = this;

        OnWindowResize?.Invoke(Screen.width, Screen.height);
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            OnWindowResize?.Invoke(lastWidth, lastHeight);
        }
    }

    public void DelayedRender(RSPObject obj)
    {
        StartCoroutine(RenderOnEndOfFrame(obj));
    }

    public IEnumerator RenderOnEndOfFrame(RSPObject obj)
    {
        yield return new WaitForEndOfFrame();

        obj.Render();
    }

}
