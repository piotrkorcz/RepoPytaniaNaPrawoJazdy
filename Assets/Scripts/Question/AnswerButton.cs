using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    private const string ANIMATOR_CONDITION = "SELECTED";
    [SerializeField] private Animator animator;

    public void Select(bool selected)
    {
        animator.SetBool(ANIMATOR_CONDITION, selected);
    }
}
