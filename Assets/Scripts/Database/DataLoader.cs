using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataLoader : MonoBehaviour
{
    private static DataLoader instance;
    public static DataLoader Instance { get { return instance; } }

    public const int POINTS_REQUIRED_COUNT = 68;
    public const int TOTAL_QUESTION_COUNT = 32;
    public const int TOTAL_SIMPLE_QUESTION_COUNT = 20;
    public const int TOTAL_SPECIALIZED_QUESTION_COUNT = 12;

    public const string TRUE_STRING_VALUE = "Tak";
    public const string FALSE_STRING_VALUE = "Nie";

    public const string PICTURE_FILE_FORMAT = ".jpg";
    public const string VIDEO_FILE_FORMAT = ".mp4";

    private const int QUESTIONS_PER_DATA_SET = 32;

    private const string API_URL = "https://admin.aplikacjaszkolajazdy.pl/api/";

    private const string GET_ALL_SIMPLE = "getAllSimple.php";
    private const string GET_ALL_SIMPLE_RANDOM_QUESTION = "getAllSimpleRandom.php";
    private const string GET_ALL_SPECIALIZED_RANDOM_QUESTION = "getAllSpecializedRandom.php";

    private const string TOKEN = "?token=qbTon8Hk1dUX02mp";
    private const string START = "&start=";
    private const string LIMIT = "&limit=";
    private const string CATEGORY = "&category=B";

    public List<QuestionData> examQuestions;
    public List<QuestionData> databaseQuestions = new();
    public List<QuestionData> answeredQuestions = new();

    [SerializeField] private QuestionUIController questionUIController;
    [SerializeField] private GameObject loading;

    [SerializeField] private Button accessExam;
    [SerializeField] private Button accessDatabase;

    private int currentDataSet;
    private bool isLoading;
    public bool IsLoading { get { return isLoading; } }
    public event Action OnLoad;

    private void Awake()
    {
        instance = this;
    }

    public void LoadExam()
    {
        StartCoroutine(LoadExamData());
    }
    
    public void LoadNewDataSetInTheBackground()
    {
        EnableAccessess(false);
        StartCoroutine(LoadData(false));
    }
    public void LoadNewDataSet()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData(bool shouldActivateLoadingCurtine = true)
    {
        isLoading = true;
        if(shouldActivateLoadingCurtine)
            loading.SetActive(true);

        string url = API_URL + GET_ALL_SIMPLE + TOKEN + START + currentDataSet * QUESTIONS_PER_DATA_SET + LIMIT + QUESTIONS_PER_DATA_SET;

        currentDataSet++;

        UnityWebRequest request = UnityWebRequest.Get(url);
        
        request.SetRequestHeader("Content-Type", "application/json");


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Loading error: " + request.error);
        }
        else
        {
            JSONNode node = JSON.Parse(request.downloadHandler.text);

            foreach (JSONNode simpleQuestion in node["data"])
                databaseQuestions.Add(GetQuestionData(simpleQuestion, false));

            foreach (QuestionData questionData in databaseQuestions)
                yield return StartCoroutine(LoadPictureOrVideoThumbnail(questionData));
        }

        isLoading = false;
        loading.SetActive(false);
        EnableAccessess(true);
        OnLoad?.Invoke();

    }

    private IEnumerator LoadExamData()
    {
        isLoading = true;
        loading.SetActive(true);

        string simpleURL = API_URL + GET_ALL_SIMPLE_RANDOM_QUESTION + TOKEN + LIMIT + TOTAL_SIMPLE_QUESTION_COUNT + CATEGORY;

        UnityWebRequest simpleRequest = UnityWebRequest.Get(simpleURL);

        simpleRequest.SetRequestHeader("Content-Type", "application/json");

        yield return simpleRequest.SendWebRequest();

        examQuestions = new();

        if (simpleRequest.result == UnityWebRequest.Result.ConnectionError ||
            simpleRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Loading error: " + simpleRequest.error);
        }
        else
        {
           
            JSONNode node = JSON.Parse(simpleRequest.downloadHandler.text);

            foreach (JSONNode simpleQuestion in node["data"])
                examQuestions.Add(GetQuestionData(simpleQuestion, false));
        }

        string specializedURL = API_URL + GET_ALL_SPECIALIZED_RANDOM_QUESTION + TOKEN + LIMIT + TOTAL_SPECIALIZED_QUESTION_COUNT + CATEGORY;

        UnityWebRequest specializedRequest = UnityWebRequest.Get(specializedURL);

        specializedRequest.SetRequestHeader("Content-Type", "application/json");

        yield return specializedRequest.SendWebRequest();

        if (specializedRequest.result == UnityWebRequest.Result.ConnectionError ||
            specializedRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Loading error: " + specializedRequest.error);
        }
        else
        {
            
            JSONNode node = JSON.Parse(specializedRequest.downloadHandler.text);

            foreach (JSONNode specializedQuestion in node["data"])
                examQuestions.Add(GetQuestionData(specializedQuestion, true));
            
        }

        foreach (QuestionData questionData in examQuestions)
            yield return StartCoroutine(LoadPictureOrVideoThumbnail(questionData));

        isLoading = false;
        loading.SetActive(false);
        OnLoad?.Invoke();

        questionUIController.Initialize(examQuestions);
    }

    private void EnableAccessess(bool should = true)
    {
        accessDatabase.interactable = should;
        accessExam.interactable = should;
    }
    private QuestionData GetQuestionData(JSONNode node, bool isSpecialized)
    {
        if (isSpecialized)
        {
            SpecializedQuestionData specializedQuestionData = new();

            specializedQuestionData.id = node["id"];
            specializedQuestionData.questionNumber = node["question_number"];
            specializedQuestionData.question = node["question"];
            specializedQuestionData.mediaLink = node["media_link"];
            if (specializedQuestionData.IsFileVideo())
                specializedQuestionData.frameImage = node["frame_image"];

            specializedQuestionData.A = node["option_a"];
            specializedQuestionData.B = node["option_b"];
            specializedQuestionData.C = node["option_c"];

            Enum.TryParse(node["answer"], out Answer answer);

            specializedQuestionData.answer = answer;

            QuestionData questionData = answeredQuestions.Find(aQ => aQ.questionNumber == specializedQuestionData.questionNumber);

            if (questionData != null)
                specializedQuestionData.sortingType = questionData.sortingType; 

            return specializedQuestionData;
        }
        else
        {
            SimpleQuestionData simpleQuestionData = new();

            simpleQuestionData.id = node["id"];
            simpleQuestionData.questionNumber = node["question_number"];
            simpleQuestionData.question = node["question"];
            simpleQuestionData.mediaLink = node["media_link"];
            if (simpleQuestionData.IsFileVideo())
                simpleQuestionData.frameImage = node["frame_image"];
            simpleQuestionData.answer = node["answer"] == 1;

            QuestionData questionData = answeredQuestions.Find(aQ => aQ.questionNumber == simpleQuestionData.questionNumber);

            if (questionData != null)
                simpleQuestionData.sortingType = questionData.sortingType;

            return simpleQuestionData;
        }
    }

    public IEnumerator LoadPictureOrVideoThumbnail(QuestionData questionData, bool changeLoadingVisibility = false)
    {
        string url = !questionData.IsFileVideo() ? questionData.mediaLink : questionData.frameImage;

        Sprite sprite = MediaLibrary.GetSpriteIfExists(url);

        if (sprite != null)
        {
            questionData.sprite = sprite;
            yield break;
        }

        if (changeLoadingVisibility)
            loading.SetActive(true);

        if (url == null || !url.Contains(PICTURE_FILE_FORMAT))
        {
            questionData.loaded = true;
            yield break;
        }

        UnityWebRequest pictureRequest = UnityWebRequestTexture.GetTexture(url);

        yield return pictureRequest.SendWebRequest();

        if (pictureRequest.result == UnityWebRequest.Result.ConnectionError ||
            pictureRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Loading error: " + pictureRequest.error + " URL: " + url);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(pictureRequest);

            questionData.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            MediaLibrary.AddSprite(url, questionData.sprite);
        }

        questionData.loaded = true;

        if (changeLoadingVisibility)
            loading.SetActive(false);
    }

}

public class QuestionData
{
    public int id;
    public int questionNumber;
    public string question;
    public string mediaLink;
    public string frameImage;
    public Sprite sprite;
    public SortingType sortingType = SortingType.Unanswered;
    public bool loaded;

    public bool IsSpecialized()
    {
        return this is SpecializedQuestionData;
    }

    public bool IsFileVideo()
    {
        return mediaLink.Contains(DataLoader.VIDEO_FILE_FORMAT);
    }

}

public class SimpleQuestionData : QuestionData
{
    public bool answer;
}

public class SpecializedQuestionData : QuestionData
{
    public string A;
    public string B;
    public string C;
    public Answer answer;
}

public enum Answer
{
    A,
    B,
    C
}
