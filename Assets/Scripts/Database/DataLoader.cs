using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    private const int QUESTIONS_PER_DATA_SET = 9999999;

    private const string API_URL = "https://admin.aplikacjaszkolajazdy.pl/api/";

    private const string GET_ALL_SIMPLE = "getAllSimple.php";
    private const string GET_ALL_SPECIALIZED = "getAllSpecialized.php";
    private const string GET_ALL_SIMPLE_RANDOM_QUESTION = "getAllSimpleRandom.php";
    private const string GET_ALL_SPECIALIZED_RANDOM_QUESTION = "getAllSpecializedRandom.php";

    private const string TOKEN = "?token=qbTon8Hk1dUX02mp";
    private const string START = "&start=";
    private const string LIMIT = "&limit=";
    private const string CATEGORY = "&category=B";

    public const string LOCAL_SIMPLE_DATABASE_FILENAME = "savedSimpleQuestions.json";
    public const string LOCAL_SPECIALIZED_DATABASE_FILENAME = "savedSpecializedQuestions.json";

    public List<QuestionData> examQuestions;
    public List<QuestionData> databaseQuestions = new();
    public List<QuestionData> answeredQuestions = new();

    [SerializeField] private QuestionUIController questionUIController;
    [SerializeField] private GameObject loading;

    [SerializeField]
    public GameObject loadingInformation;

    [SerializeField] private Button accessExam;
    [SerializeField] private Button accessDatabase;
    //[SerializeField] private Button settingsButton;

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

        StartCoroutine(FetchAllDatabaseData());
    }
    public void LoadNewDataSet()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator FetchAllDatabaseData()
    {
            EnableAccessess(false);
            Debug.Log("Loading from internet data");
            isLoading = true;

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

                SaveSystem.SaveJsonToFile(node.ToString(), LOCAL_SIMPLE_DATABASE_FILENAME);
            }


            string specializedURL = API_URL + GET_ALL_SPECIALIZED + TOKEN + LIMIT + QUESTIONS_PER_DATA_SET + CATEGORY;

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
                    databaseQuestions.Add(GetQuestionData(specializedQuestion, true));

                foreach (JSONNode specializedQuestion in node["data"])
                {
                    yield return StartCoroutine(LoadPictureOrVideoThumbnail(GetQuestionData(specializedQuestion, true)));
                }

                SaveSystem.SaveJsonToFile(node.ToString(), LOCAL_SPECIALIZED_DATABASE_FILENAME);
            }

            yield return StartCoroutine(UnityAsyncExtentions.WaitForTask(StartMediaDownload(databaseQuestions)));



        isLoading = false;
        loading.SetActive(false);
        EnableAccessess(true);
        OnLoad?.Invoke();
    }
    private IEnumerator LoadData(bool shouldActivateLoadingCurtine = true)
    {
        string finalPath = Path.Combine(Application.persistentDataPath, LOCAL_SIMPLE_DATABASE_FILENAME);
        Debug.Log(finalPath);

        if (!File.Exists(finalPath))
        {
            Debug.Log("Loading from internet data");
            isLoading = true;
            if (shouldActivateLoadingCurtine)
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
                {
                    yield return StartCoroutine(LoadPictureOrVideoThumbnail(questionData));
                }
 
                SaveSystem.SaveJsonToFile(node.ToString(), LOCAL_SIMPLE_DATABASE_FILENAME);
            }


        }
        else
        {
            Debug.Log("Loading from local data");
            JSONNode localData = SaveSystem.LoadJsonFromFile(LOCAL_SIMPLE_DATABASE_FILENAME);
            foreach (JSONNode simpleQuestion in localData["data"])
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

        string simpleFinalPath = Path.Combine(Application.persistentDataPath, LOCAL_SIMPLE_DATABASE_FILENAME);
        string specializedFinalPath = Path.Combine(Application.persistentDataPath, LOCAL_SPECIALIZED_DATABASE_FILENAME);
        if (!File.Exists(simpleFinalPath) || !File.Exists(specializedFinalPath))
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


        }

        else
        {
            examQuestions = new();

            JSONNode localData = SaveSystem.LoadJsonFromFile(simpleFinalPath);

            List<JSONNode> deJsonedQuestionList = SaveSystem.GetRandomRecordsFromJsonNode(localData, "data", 20);
            foreach(JSONNode simpleQuestion in deJsonedQuestionList)
            {
                examQuestions.Add(GetQuestionData(simpleQuestion, false));
            }

            localData = SaveSystem.LoadJsonFromFile(specializedFinalPath);

            deJsonedQuestionList = SaveSystem.GetRandomRecordsFromJsonNode(localData, "data", 12);

            foreach (JSONNode specializedQuestion in deJsonedQuestionList)
            {
                examQuestions.Add(GetQuestionData(specializedQuestion, true));
            }

            Debug.Log(examQuestions.Count);
        }

        isLoading = false;
        loading.SetActive(false);
        OnLoad?.Invoke();

        questionUIController.Initialize(examQuestions);

    }

    private void EnableAccessess(bool should = true)
    {
        accessDatabase.interactable = should;
        accessExam.interactable = should;
        //settingsButton.interactable = should;
        loadingInformation.gameObject.SetActive(!should);
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

    public async Task StartMediaDownload(List<QuestionData> questionsToDownload)
    {
        Debug.Log($"Starting media download for {questionsToDownload.Count} questions...");

        await DownloadAndSaveAllMedia(questionsToDownload);

        Debug.Log("Media download process finished.");
    }

    private async Task DownloadAndSaveAllMedia(List<QuestionData> questions)
    {
        string mediaSavePath = Path.Combine(Application.persistentDataPath, "MediaCache");
        Debug.Log(mediaSavePath);
        if (!Directory.Exists(mediaSavePath))
        {
            Directory.CreateDirectory(mediaSavePath);
        }

        var allUrls = new HashSet<string>();
        foreach (var question in questions)
        {
            if (!string.IsNullOrEmpty(question.mediaLink))
            {
                allUrls.Add(question.mediaLink);
            }
            if (question.IsFileVideo() && !string.IsNullOrEmpty(question.frameImage))
            {
                allUrls.Add(question.frameImage);
            }
        }

        List<Task> downloadTasks = allUrls.Select(url => DownloadAndSaveFileAsync(url, mediaSavePath)).ToList();

        await Task.WhenAll(downloadTasks);
    }

    private async Task DownloadAndSaveFileAsync(string url, string saveDirectory)
    {
        try
        {
            string fileName = Path.GetFileName(new Uri(url).LocalPath);
            string savePath = Path.Combine(saveDirectory, fileName);

            if (File.Exists(savePath))
            {
                Debug.Log($"File already exists, skipping: {fileName}");
                return;
            }

            using (var request = UnityWebRequest.Get(url))
            {
                 await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    byte[] fileData = request.downloadHandler.data;

                    await File.WriteAllBytesAsync(savePath, fileData);

                    Debug.Log($"Successfully downloaded and saved: {fileName} to {savePath}");
                }
                else
                {
                    Debug.LogError($"Failed to download {url}: {request.error}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An exception occurred while downloading {url}: {ex.Message}");
        }
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
