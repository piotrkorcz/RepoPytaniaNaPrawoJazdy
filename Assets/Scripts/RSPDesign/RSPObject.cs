using System.Collections;
using UnityEngine;

public class RSPObject : MonoBehaviour
{
    private void Start()
    {
        Refresh();
        RSPManager.OnWindowResize += Refresh;
    }

    private void OnDestroy()
    {
        RSPManager.OnWindowResize -= Refresh;
    }

    private void Refresh(int width = RSPManager.DEFAULT_WIDTH, int height = RSPManager.DEFAULT_HEIGHT)
    {
        Render();

        if (RSPManager.Instance != null)
            RSPManager.Instance.DelayedRender(this);
    }

    public virtual void Render() {}

}
