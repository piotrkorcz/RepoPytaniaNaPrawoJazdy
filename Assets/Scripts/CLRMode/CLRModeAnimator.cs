using UnityEngine;

public class CLRModeAnimator : CLRModeObject
{
    private const string ANIMATOR_CONDITION = "LIGHT";
    [SerializeField] private Animator animator;

    protected override void Render(bool isLight)
    {
        animator.SetBool(ANIMATOR_CONDITION, isLight);
    }

}
