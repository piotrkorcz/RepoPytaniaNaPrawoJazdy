using TMPro;
using UnityEngine;

public class SpecializedAnswerButton : AnswerButton
{
    [SerializeField] private TextMeshProUGUI answerTextView;
    public Answer answer;

    public void Render(SpecializedQuestionData specializedQuestionData)
    {
        gameObject.SetActive(true);
        switch (answer)
        {
            case Answer.A:
                answerTextView.text = specializedQuestionData.A;
                break;
            case Answer.B:
                answerTextView.text = specializedQuestionData.B;
                break;
            case Answer.C:
                answerTextView.text = specializedQuestionData.C;
                break;
        }
    }
}
