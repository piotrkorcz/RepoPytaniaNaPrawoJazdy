using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static string FilePath => Application.persistentDataPath + "/answers.json";
    private static AnsweredQuestionList answeredQuestionList;

    private void Start()
    {
        Debug.Log(FilePath);
        LoadAnswers();
    }

    public static void SaveAnswers()
    {
        string json = JsonUtility.ToJson(answeredQuestionList, true);
        File.WriteAllText(FilePath, json);
    }

    private static void LoadAnswers()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            answeredQuestionList = JsonUtility.FromJson<AnsweredQuestionList>(json);

            DataLoader.Instance.answeredQuestions = new();
            answeredQuestionList.answeredQuestions.OrderBy(aQ => aQ.questionNumber);
            answeredQuestionList.answeredQuestions.ForEach(aQ => DataLoader.Instance.answeredQuestions.Add(aQ.GetQuestionData()));
        }
        else
        {
            answeredQuestionList = new();
            answeredQuestionList.answeredQuestions = new();
        }
    }

    public static void AddAnsweredQuestions(List<QuestionData> questionData)
    {
        questionData.ForEach(qD =>
        {
            AnsweredQuestion answeredQuestion = answeredQuestionList.answeredQuestions.Find(aQ => aQ.questionNumber == qD.questionNumber);

            if (answeredQuestion != null)
                answeredQuestion.sortingType = qD.sortingType;
            else
                answeredQuestionList.answeredQuestions.Add(new AnsweredQuestion(qD));
        });

        DataLoader.Instance.answeredQuestions = new();
        answeredQuestionList.answeredQuestions.OrderBy(aQ => aQ.questionNumber);
        answeredQuestionList.answeredQuestions.ForEach(aQ => DataLoader.Instance.answeredQuestions.Add(aQ.GetQuestionData()));

        SaveAnswers();
    }

}

[System.Serializable]
public class AnsweredQuestionList
{
    public List<AnsweredQuestion> answeredQuestions = new();
}

[System.Serializable]
public class AnsweredQuestion
{
    public bool isSpecialized;
    public int id;
    public int questionNumber;
    public string question;
    public string mediaLink;
    public string frameImage;

    public SortingType sortingType = SortingType.Unanswered;

    public bool simpleAnswer;

    public string A;
    public string B;
    public string C;
    public Answer specializedAnswer;

    public AnsweredQuestion(QuestionData questionData)
    {
        if (questionData is SimpleQuestionData simpleQuestionData)
        {
            isSpecialized = false;

            simpleAnswer = simpleQuestionData.answer;
        }
        else if (questionData is SpecializedQuestionData specializedQuestionData)
        {
            isSpecialized = true;

            A = specializedQuestionData.A;
            B = specializedQuestionData.B;
            C = specializedQuestionData.C;
            specializedAnswer = specializedQuestionData.answer;
        }

        sortingType = questionData.sortingType;

        if (questionData.sortingType == SortingType.Skipped)
            sortingType = SortingType.Incorrect;

        id = questionData.id;
        questionNumber = questionData.questionNumber;
        question = questionData.question;
        mediaLink = questionData.mediaLink;
        frameImage = questionData.frameImage;
    }

    public QuestionData GetQuestionData()
    {
        QuestionData questionData;

        if (isSpecialized)
        {
            questionData = new SpecializedQuestionData();
            SpecializedQuestionData specializedQuestionData = (SpecializedQuestionData)questionData;

            specializedQuestionData.A = A;
            specializedQuestionData.B = B;
            specializedQuestionData.C = C;
            specializedQuestionData.answer = specializedAnswer;
        }
        else
        {
            questionData = new SimpleQuestionData();
            SimpleQuestionData simpleQuestionData = (SimpleQuestionData)questionData;

            simpleQuestionData.answer = simpleAnswer;
        }

        questionData.sortingType = sortingType;

        questionData.id = id;
        questionData.questionNumber = questionNumber;
        questionData.question = question;
        questionData.mediaLink = mediaLink;
        questionData.frameImage = frameImage;

        return questionData;
    }

}