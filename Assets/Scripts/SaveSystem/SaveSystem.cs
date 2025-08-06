using SimpleJSON;
using System;
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

    public static void SaveJsonToFile(string jsonContent, string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(jsonContent, true);
        File.WriteAllText(filePath, json);
        try
        {
            File.WriteAllText(filePath, jsonContent);
            Debug.Log($"JSON saved to: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save JSON to {filePath}: {e.Message}");
        }
    }

    public static JSONNode LoadJsonFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"File '{fileName}' not found at {filePath}.");
            return null;
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            JSONNode node = JSON.Parse(jsonContent);
            Debug.Log($"Successfully loaded JSON from: {filePath}");
            return node;
        }
        catch (FileNotFoundException)
        {
            // This case should ideally be caught by File.Exists, but good for robustness.
            Debug.LogError($"File not found during read: {filePath}");
            return null;
        }
        catch (IOException e)
        {
            Debug.LogError($"Error reading file {filePath}: {e.Message}");
            return null;
        }
        catch (Exception e) // Catch parsing errors from SimpleJSON
        {
            Debug.LogError($"Error parsing JSON from {filePath}: {e.Message}");
            return null;
        }
    }

    public static List<JSONNode> GetRandomRecordsFromJsonNode(JSONNode sourceNode, string arrayKey, int count)
    {
        List<JSONNode> randomRecords = new List<JSONNode>();

        if (sourceNode == null || sourceNode[arrayKey] == null || !sourceNode[arrayKey].IsArray || sourceNode[arrayKey].Count == 0)
        {
            Debug.LogWarning($"Source JSONNode does not contain a valid array under key '{arrayKey}' or is empty.");
            return randomRecords;
        }

        JSONNode recordsArray = sourceNode[arrayKey];
        List<JSONNode> allRecords = new List<JSONNode>();

        // Copy all JSONNode elements into a mutable C# List
        foreach (JSONNode record in recordsArray)
        {
            allRecords.Add(record);
        }

        // Shuffle the list using Fisher-Yates algorithm
        System.Random rng = new System.Random();
        int n = allRecords.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            JSONNode value = allRecords[k];
            allRecords[k] = allRecords[n];
            allRecords[n] = value;
        }

        // Select the first 'count' elements
        for (int i = 0; i < Math.Min(count, allRecords.Count); i++)
        {
            randomRecords.Add(allRecords[i]);
        }

        return randomRecords;
    }

    public static Sprite LoadSpriteFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found at: {filePath}");
            return null;
        }

        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                return sprite;
            }
            else
            {
                Debug.LogError($"Failed to load image data from: {filePath}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"An error occurred while loading the file: {e.Message}");
            return null;
        }
    }

    public static void DeleteLocallySavedData()
    {
        string mediaFolder = Path.Combine(Application.persistentDataPath, DataLoader.LOCAL_MEDIA_FOLDER_NAME);

        Directory.Delete(mediaFolder, true);

        string filePath = Path.Combine(Application.persistentDataPath, DataLoader.LOCAL_SIMPLE_DATABASE_FILENAME);
        File.Delete(filePath);

        filePath = Path.Combine(Application.persistentDataPath,DataLoader.LOCAL_SPECIALIZED_DATABASE_FILENAME) ;
        File.Delete(filePath);
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