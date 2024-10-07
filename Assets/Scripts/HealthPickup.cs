using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Required for UI elements
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Networking;  // For TextMeshPro components

public class HealthPickup : MonoBehaviour
{
    public int _minHealingAmount;
    public int MinHealingAmount
    {
        get { return _minHealingAmount; }
        private set
        {
            _minHealingAmount = value;
        }
    }
    public int _maxHealingAmount;
    public int MaxHealingAmount
    {
        get { return _maxHealingAmount; }
        private set
        {
            _maxHealingAmount = value;
        }
    }

    private List<QuestionDto> questionDtos = new List<QuestionDto>();
    private static readonly string questionsAPIstr = "https://fu-pru-game-server-apis.onrender.com/api/v1/questions";
    private static readonly string loginAPIstr = "https://fu-pru-game-server-apis.onrender.com/api/v1/login";//TODO
    private List<ResponseQuestion> responseQuestions = new List<ResponseQuestion>();
    private static readonly System.Random r = new System.Random();
    // public GameObject questionPanel;
    public GameObject subQuestionPanel;
    public TMP_Text questionText; // Use Text instead of GameObject for easier access to the text
    public Button AButton;
    public Button BButton;
    public Button CButton;
    public Button DButton;
    private QuestionDto currentQuestion;
    private ResponseQuestion currFetchedQuestion;
    private int healthRestore;
    private bool questionAnswered = false;
    private bool answerCorrect = false;
    private Coroutine questionTimerCoroutine; // To store the coroutine reference
    private static readonly HttpClient client = new HttpClient();

    async void Start()
    {
        StartCoroutine(GetQuestions());
        questionDtos.Add(new QuestionDto("What is the capital of France?",
            new Dictionary<char, string> { { 'A', "Paris" }, { 'B', "Berlin" }, { 'C', "Rome" }, { 'D', "Madrid" } }, 'A'));

        questionDtos.Add(new QuestionDto("Which planet is known as the Red Planet?",
            new Dictionary<char, string> { { 'A', "Earth" }, { 'B', "Mars" }, { 'C', "Venus" }, { 'D', "Jupiter" } }, 'B'));

        questionDtos.Add(new QuestionDto("What is the largest ocean on Earth?",
            new Dictionary<char, string> { { 'A', "Atlantic" }, { 'B', "Indian" }, { 'C', "Pacific" }, { 'D', "Southern" } }, 'C'));



    }
    IEnumerator GetQuestions()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(questionsAPIstr))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("HTTP Error: " + webRequest.error);
            }
            else
            {
                // Parse JSON response
                string json = webRequest.downloadHandler.text;
                responseQuestions = JsonUtility.FromJson<List<ResponseQuestion>>(json);
                HandleQuestions(responseQuestions);
            }
        }
    }
    void HandleQuestions(List<ResponseQuestion> questions)
    {
        if (questions != null && questions.Count > 0)
        {
            foreach (var question in questions)
            {
                Debug.Log($" Question: {question.QuestionText}, Answer: {question.AnswerChoice}");
            }
            // Process the data as needed
        }
        else
        {
            Debug.LogError("Failed to parse questions data or no data received");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            //RANDOM TO SHOW OR GAIN HEALTH IMMEDIATELY
            bool doShow = r.Next(0, 2) == 0;
            Debug.Log(">>> DIRECT HEAL OR QUESTION: " + (doShow ? "QUESTION IS SHOWING" : "NOPE GAIN NOW"));
            if (MinHealingAmount >= 0)
            {
                if (doShow)
                {
                    #region question handling

                    // Pause the game and show the question panel
                    Time.timeScale = 0;  // Pause the game
                    ShowRandomQuestion();

                    // Setup button click listeners
                    AButton.onClick.AddListener(() => AnswerQuestion('A', damageable));
                    BButton.onClick.AddListener(() => AnswerQuestion('B', damageable));
                    CButton.onClick.AddListener(() => AnswerQuestion('C', damageable));
                    DButton.onClick.AddListener(() => AnswerQuestion('D', damageable));

                    // Start the question timer
                    questionTimerCoroutine = StartCoroutine(QuestionTimer(damageable));

                    #endregion
                }
                else
                {
                    //GET THE HEALTH DIRECTLY
                    healthRestore = r.Next(MinHealingAmount, MaxHealingAmount);
                    damageable.Heal(healthRestore);

                    // Destroy the health pickup object
                    Destroy(gameObject);
                }
            }
            else
            {
                //GET THE HEALTH DIRECTLY
                healthRestore = r.Next(MinHealingAmount, MaxHealingAmount);
                damageable.Hit(-healthRestore, Vector2.zero);

                // Destroy the health pickup object
                Destroy(gameObject);

            }
        }
    }

    void ShowRandomQuestion()
    {
        // Select a random question from the list
        currentQuestion = questionDtos[r.Next(0, questionDtos.Count)];

        // Display the question and options
        // questionPanel.SetActive(true);  // Activate the question panel
        subQuestionPanel.SetActive(true); //Activate the sub question panel
        AButton.gameObject.SetActive(true);  // Activate the A button
        BButton.gameObject.SetActive(true);  // Activate the B button
        CButton.gameObject.SetActive(true);  // Activate the C button
        DButton.gameObject.SetActive(true);  // Activate the D button
        questionText.gameObject.SetActive(true);  // Activate the question text

        // // Show the full question with answer choices in the questionText
        // questionText.text = currentQuestion.QuestionTxt + "\n" +
        //                     "A: " + currentQuestion.AnswerChoices['A'] + "\n" +
        //                     "B: " + currentQuestion.AnswerChoices['B'] + "\n" +
        //                     "C: " + currentQuestion.AnswerChoices['C'] + "\n" +
        //                     "D: " + currentQuestion.AnswerChoices['D'];


        # region using question fetched from server
        Debug.Log(responseQuestions.Count);
        currFetchedQuestion = responseQuestions[0];
        questionText.text = currFetchedQuestion.QuestionText;
        #endregion

        // Set the button texts to only show 'A', 'B', 'C', 'D'
        TMP_Text aButtonText = AButton.GetComponentInChildren<TMP_Text>();
        TMP_Text bButtonText = BButton.GetComponentInChildren<TMP_Text>();
        TMP_Text cButtonText = CButton.GetComponentInChildren<TMP_Text>();
        TMP_Text dButtonText = DButton.GetComponentInChildren<TMP_Text>();

        aButtonText.text = "A";
        bButtonText.text = "B";
        cButtonText.text = "C";
        dButtonText.text = "D";

        // Set the font size
        aButtonText.fontSize = 32;  // Example size, adjust as needed
        bButtonText.fontSize = 32;
        cButtonText.fontSize = 32;
        dButtonText.fontSize = 32;
    }



    void AnswerQuestion(char answer, Damageable damageable)
    {
        // Stop the timer since the player has answered
        if (questionTimerCoroutine != null && this != null)
        {
            StopCoroutine(questionTimerCoroutine);
        }

        // Check if the answer is correct
        if (answer == currFetchedQuestion.AnswerChoice)
        {
            answerCorrect = true;
        }
        questionAnswered = true;

        ContinueGame(damageable);
    }

    IEnumerator QuestionTimer(Damageable damageable)
    {
        yield return new WaitForSecondsRealtime(10);  // Wait for 10 real-time seconds (ignores Time.timeScale)

        if (!questionAnswered)
        {
            // Time is up and no answer was given, so mark the answer as incorrect
            answerCorrect = false;
            questionAnswered = true;
            ContinueGame(damageable);
        }
    }

    void ContinueGame(Damageable damageable)
    {

        //Destroy the event listener
        AButton.onClick.RemoveAllListeners();
        BButton.onClick.RemoveAllListeners();
        CButton.onClick.RemoveAllListeners();
        DButton.onClick.RemoveAllListeners();

        // Close the question panel and resume the game
        // questionPanel.SetActive(false);
        subQuestionPanel.SetActive(false);
        AButton.gameObject.SetActive(false);
        BButton.gameObject.SetActive(false);
        CButton.gameObject.SetActive(false);
        DButton.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        Time.timeScale = 1;  // Resume the game

        // Apply health restoration if the answer was correct
        healthRestore = answerCorrect ? r.Next(MinHealingAmount, MaxHealingAmount) : 0;
        damageable.Heal(healthRestore);

        // Destroy the health pickup object
        Destroy(gameObject);
    }
}

