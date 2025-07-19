using TMPro;
using UnityEngine;

public class SummaryUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI generalResultTextView;
    [SerializeField] private TextMeshProUGUI timeTextView;
    [SerializeField] private string testPassedInfo;
    [SerializeField] private string testFailedInfo;
    [SerializeField] private Color32 testPassedColor;
    [SerializeField] private Color32 testFailedColor;

    [Space]

    [SerializeField] private RectTransform maximumGraph;
    [SerializeField] private RectTransform minimiumGraph;

    [Space]

    [SerializeField] private RectTransform skippedGraph;
    [SerializeField] private RectTransform incorrectGraph;
    [SerializeField] private RectTransform correctGraph;

    [Space]

    [SerializeField] private TextMeshProUGUI correctAnswersCountTextView;
    [SerializeField] private TextMeshProUGUI collectedPointsCountTextView;

    [Space]

    [SerializeField] private GameObject failedIcon;
    [SerializeField] private GameObject passedIcon;

    public void Render(float time, int correctAnswers, int incorrectAnswers, int skippedAnswers, int points)
    {
        gameObject.SetActive(true);

        timeTextView.text = QuestionUIController.GetTimeString(time);

        if (points < DataLoader.POINTS_REQUIRED_COUNT)
        {
            generalResultTextView.text = testFailedInfo;
            generalResultTextView.color = testFailedColor;
            failedIcon.SetActive(true);
            passedIcon.SetActive(false);
        }
        else
        {
            generalResultTextView.text = testPassedInfo;
            generalResultTextView.color = testPassedColor;
            failedIcon.SetActive(false);
            passedIcon.SetActive(true);
        }

        float minimumHeight = minimiumGraph.rect.height;
        float maximumHeight = maximumGraph.rect.height;
        float difference = maximumHeight - minimumHeight;

        if (Mathf.Max(correctAnswers, incorrectAnswers, skippedAnswers) == correctAnswers)
        {
            correctGraph.sizeDelta = new Vector2(correctGraph.sizeDelta.x, maximumHeight);
            incorrectGraph.sizeDelta = new Vector2(incorrectGraph.sizeDelta.x, minimumHeight + (float)incorrectAnswers / correctAnswers * difference);
            skippedGraph.sizeDelta = new Vector2(skippedGraph.sizeDelta.x, minimumHeight + (float)skippedAnswers / correctAnswers * difference);
        }
        else if (Mathf.Max(correctAnswers, incorrectAnswers, skippedAnswers) == incorrectAnswers)
        {
            incorrectGraph.sizeDelta = new Vector2(incorrectGraph.sizeDelta.x, maximumHeight);
            correctGraph.sizeDelta = new Vector2(correctGraph.sizeDelta.x, minimumHeight + (float)correctAnswers / incorrectAnswers * difference);
            skippedGraph.sizeDelta = new Vector2(skippedGraph.sizeDelta.x, minimumHeight + (float)skippedAnswers / incorrectAnswers * difference);
        }
        else
        {
            skippedGraph.sizeDelta = new Vector2(skippedGraph.sizeDelta.x, maximumHeight);
            correctGraph.sizeDelta = new Vector2(correctGraph.sizeDelta.x, minimumHeight + (float)correctAnswers / skippedAnswers * difference);
            incorrectGraph.sizeDelta = new Vector2(incorrectGraph.sizeDelta.x, minimumHeight + (float)incorrectAnswers / skippedAnswers * difference);
        }

        correctAnswersCountTextView.text = correctAnswers.ToString();
        collectedPointsCountTextView.text = points.ToString();
    }

}
