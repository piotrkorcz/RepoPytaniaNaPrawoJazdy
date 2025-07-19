using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class QuestionUIController : MonoBehaviour
{
    private const int SIMPLE_QUESTION_TRIPLE_POINTS_COUNT = 10;
    private const int SIMPLE_QUESTION_DOUBLE_POINTS_COUNT = 16;
    private const int SPECIALIZED_QUESTION_TRIPLE_POINTS_COUNT = 26;
    private const int SPECIALIZED_QUESTION_DOUBLE_POINTS_COUNT = 30;
    private const float SIMPLE_QUESTION_TIME = 20f;
    private const float SPECIALIZED_QUESTION_TIME = 50f;
    private const float AFTER_MEDIA_TIME = 15f;

    [SerializeField] private Slider questionProgressSlider;
    [SerializeField] private TextMeshProUGUI currentQuestionTextView;

    [Space]

    [SerializeField] private TextMeshProUGUI timeTextView;
    [SerializeField] private List<TextMeshProUGUI> timerTextViews;
    [SerializeField] private List<Slider> timerSliders;

    [Space]

    [SerializeField] private GameObject noMediaUIPanel;
    [SerializeField] private GameObject beforeMediaUIPanel;
    [SerializeField] private GameObject mediaUIPanel;
    [SerializeField] private GameObject afterMediaUIPanel;

    [Space]

    [SerializeField] private Image picture;
    [SerializeField] private RawImage video;
    [SerializeField] private VideoPlayer videoPlayer;

    [Space]

    [SerializeField] private TextMeshProUGUI questionTextView;
    [SerializeField] private RSPContentSizeFitterLimiter questionTextViewLimiter;

    [Space]

    [SerializeField] private List<AnswerButton> simpleAnswerButtons;
    [SerializeField] private List<AnswerButton> specializedAnswerButtons;

    [Space]
    
    [SerializeField] private Button startMediaButton;
    [SerializeField] private Button nextQuestionButton;

    [Space]

    [SerializeField] private SummaryUIController summaryUIController;

    private int currentQuestion;
    private List<QuestionData> questionData;
    private QuestionData currentQuestionData;
    private AnswerButton selectedButton;
    private int points;
    private int skippedAnswers;
    private int correctAnswers;
    private int incorrectAnswers;
    private float time;
    private float questionTime;
    private float currentMaximumQuestionTime;
    private bool waitingForVideo;

    public void Initialize(List<QuestionData> questionData)
    {
        this.questionData = questionData;
        currentQuestion = 0;
        points = 0;
        correctAnswers = 0;
        time = 0;

        gameObject.SetActive(true);
        NextQuestion();
    }

    private void Update()
    {
        time += Time.deltaTime;
        timeTextView.text = GetTimeString(time);
        
        if (gameObject.activeSelf)
        {
            questionTime -= Time.deltaTime;

            if (questionTime < 0 && startMediaButton.interactable)
            {
                StartMedia();
                return;
            }

            if (videoPlayer.isPlaying)
            {
                timerSliders.ForEach(slider => slider.value = (float)(videoPlayer.length - videoPlayer.time) / (float)videoPlayer.length);
                return;
            }

            if (waitingForVideo)
                return;

            if (questionTime < 0)
            {
                AcceptAnswer();
            }
            else
            {
                timerTextViews.ForEach(textView => textView.text = $"{(int)questionTime + 1}s");
                timerSliders.ForEach(slider => slider.value = questionTime / currentMaximumQuestionTime);
            }
        }
    }

    public void SelectButton(AnswerButton selectedButton)
    {
        if (currentQuestionData is SimpleQuestionData simpleQuestionData)
            simpleAnswerButtons.ForEach(button => button.Select(button == selectedButton));
        else if (currentQuestionData is SpecializedQuestionData specializedQuestionData)
            specializedAnswerButtons.ForEach(button => button.Select(button == selectedButton));

        this.selectedButton = selectedButton;
    }

    public void SkipAnswer()
    {
        currentQuestionData.sortingType = SortingType.Skipped;
        skippedAnswers++;
        currentQuestion++;
        NextQuestion();
    }

    public void AcceptAnswer()
    {
        if (selectedButton == null)
        {
            SkipAnswer();
            return;
        }

        if (currentQuestionData is SimpleQuestionData simpleQuestionData && selectedButton is SimpleAnswerButton simpleAnswerButton)
        {
            if (simpleQuestionData.answer == simpleAnswerButton.answer)
            {
                currentQuestionData.sortingType = SortingType.Correct;
                correctAnswers++;
                points++;
                if (currentQuestion < SIMPLE_QUESTION_TRIPLE_POINTS_COUNT)
                    points++;
                if (currentQuestion < SIMPLE_QUESTION_DOUBLE_POINTS_COUNT)
                    points++;
            }
            else
            {
                currentQuestionData.sortingType = SortingType.Incorrect;
                incorrectAnswers++;
            }
        }
        else if (currentQuestionData is SpecializedQuestionData specializedQuestionData && selectedButton is SpecializedAnswerButton specializedAnswerButton)
        {
            if (specializedQuestionData.answer == specializedAnswerButton.answer)
            {
                currentQuestionData.sortingType = SortingType.Correct;
                correctAnswers++;
                points++;
                if (currentQuestion < SPECIALIZED_QUESTION_TRIPLE_POINTS_COUNT)
                    points++;
                if (currentQuestion < SPECIALIZED_QUESTION_DOUBLE_POINTS_COUNT)
                    points++;
            }
            else
            {
                currentQuestionData.sortingType = SortingType.Incorrect;
                incorrectAnswers++;
            }
        }

        currentQuestion++;
        NextQuestion();
    }

    private void NextQuestion()
    {
        if (currentQuestion >= questionData.Count)
        {
            gameObject.SetActive(false);
            summaryUIController.Render(time, correctAnswers, incorrectAnswers, skippedAnswers, points);
            SaveSystem.AddAnsweredQuestions(DataLoader.Instance.examQuestions);
            return;
        }
        currentQuestionData = questionData[currentQuestion];
        
        currentMaximumQuestionTime = SIMPLE_QUESTION_TIME;
        if (currentQuestionData is SpecializedQuestionData)
            currentMaximumQuestionTime = SPECIALIZED_QUESTION_TIME;

        questionTime = currentMaximumQuestionTime;

        Render();
        SelectButton(null);
    }

    private void Render()
    {
        questionProgressSlider.value = (float)(currentQuestion + 1) / DataLoader.TOTAL_QUESTION_COUNT;
        currentQuestionTextView.text = (currentQuestion + 1).ToString();

        simpleAnswerButtons.ForEach(button => button.gameObject.SetActive(false));
        specializedAnswerButtons.ForEach(button => button.gameObject.SetActive(false));

        questionTextView.text = currentQuestionData.question;
        questionTextViewLimiter.Render();

        picture.gameObject.SetActive(false);
        video.enabled = false;
        startMediaButton.interactable = false;

        RenderMediaUIPanel(MediaUIPanelType.NoMedia);

        if (currentQuestionData.IsFileVideo())
        {
            videoPlayer.url = currentQuestionData.mediaLink;
            startMediaButton.interactable = true;

            RenderMediaUIPanel(MediaUIPanelType.BeforeMedia);
        }
        else
        {
            if (currentQuestionData.sprite != null)
                picture.sprite = currentQuestionData.sprite;

            if (currentQuestionData.IsSpecialized())
            {
                if (currentQuestionData.sprite != null)
                {
                    picture.gameObject.SetActive(true);
                    RenderMediaUIPanel(MediaUIPanelType.Media);
                }
            }
            else
            {
                startMediaButton.interactable = true;
                picture.gameObject.SetActive(false);

                RenderMediaUIPanel(MediaUIPanelType.BeforeMedia);
            }
        }

        if (currentQuestionData is SimpleQuestionData simpleQuestionData)
            simpleAnswerButtons.ForEach(button => button.gameObject.SetActive(true));
        else if (currentQuestionData is SpecializedQuestionData specializedQuestionData)
            specializedAnswerButtons.ForEach(button => {
                if (button is SpecializedAnswerButton specializedAnswerButton)
                    specializedAnswerButton.Render(specializedQuestionData);
            });
    }

    public void StartMedia()
    {
        if (currentQuestionData.IsFileVideo())
        {
            videoPlayer.Play();
            startMediaButton.interactable = false;
            videoPlayer.Prepare();

            nextQuestionButton.interactable = false;
            waitingForVideo = true;

            videoPlayer.prepareCompleted += (videoPlayer) => {
                video.enabled = true;
                RenderMediaUIPanel(MediaUIPanelType.Media);
                videoPlayer.Play();
            };

            videoPlayer.loopPointReached += (VideoPlayer) =>
            {
                nextQuestionButton.interactable = true;
                waitingForVideo = false;

                currentMaximumQuestionTime = AFTER_MEDIA_TIME;
                questionTime = currentMaximumQuestionTime;
                RenderMediaUIPanel(MediaUIPanelType.AfterMedia);
            };
        }
        else
        {
            startMediaButton.interactable = false;

            currentMaximumQuestionTime = AFTER_MEDIA_TIME;
            questionTime = currentMaximumQuestionTime;

            if (currentQuestionData.sprite != null)
            {
                picture.gameObject.SetActive(true);
                RenderMediaUIPanel(MediaUIPanelType.Media);
            }
            else
                RenderMediaUIPanel(MediaUIPanelType.NoMedia);
        }
    }

    public static string GetTimeString(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void RenderMediaUIPanel(MediaUIPanelType mediaPanelType)
    {
        noMediaUIPanel.SetActive(mediaPanelType == MediaUIPanelType.NoMedia);
        beforeMediaUIPanel.SetActive(mediaPanelType == MediaUIPanelType.BeforeMedia);
        mediaUIPanel.SetActive(mediaPanelType == MediaUIPanelType.Media);
        afterMediaUIPanel.SetActive(mediaPanelType == MediaUIPanelType.AfterMedia);
    }

}

public enum MediaUIPanelType
{
    NoMedia,
    BeforeMedia,
    Media,
    AfterMedia
}
