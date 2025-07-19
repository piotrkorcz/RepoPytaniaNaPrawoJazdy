using PolyAndCode.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DBCell : MonoBehaviour, ICell
{
    public Image image;

    public TextMeshProUGUI questionTextView;
    public TextMeshProUGUI answerTextView;

    public Image resultIcon;

    public Sprite correctAnswerSprite;
    public Sprite incorrectAnswerSprite;
    public Sprite skippedAnswerSprite;

    private QuestionData questionData;

    public void ConfigureCell(QuestionData questionData, int cellIndex)
    {
        if (questionData.sprite != null)
        {
            image.sprite = questionData.sprite;
            image.gameObject.SetActive(true);
        }
        else 
            image.gameObject.SetActive(false);

        questionTextView.text = questionData.question;

        if (questionData is SimpleQuestionData simpleQuestionData)
        {
            answerTextView.text = simpleQuestionData.answer ? DataLoader.TRUE_STRING_VALUE : DataLoader.FALSE_STRING_VALUE;
        }
        else if (questionData is SpecializedQuestionData specializedQuestionData)
        {
            switch (specializedQuestionData.answer)
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

        if (questionData.sortingType == SortingType.Correct)
            resultIcon.sprite = correctAnswerSprite;
        else if (questionData.sortingType == SortingType.Incorrect || (questionData.sortingType == SortingType.Skipped && !DBRecyclableScrollRectDataSource.Instance.AreExamQuestions))
            resultIcon.sprite = incorrectAnswerSprite;
        else if (questionData.sortingType == SortingType.Skipped)
            resultIcon.sprite = skippedAnswerSprite;

        resultIcon.gameObject.SetActive(questionData.sortingType != SortingType.Unanswered);

        this.questionData = questionData;

        if (!questionData.loaded)
        {
            StartCoroutine(ConfigureCellAfterMedia(questionData));
        }
    }

    private IEnumerator ConfigureCellAfterMedia(QuestionData questionData)
    {
        yield return DataLoader.Instance.StartCoroutine(DataLoader.Instance.LoadPictureOrVideoThumbnail(questionData));

        if (this.questionData.loaded)
        {
            if (this.questionData.sprite != null)
            {
                image.sprite = this.questionData.sprite;
                image.gameObject.SetActive(true);
            }
            else
                image.gameObject.SetActive(false);
        }
    }

}
