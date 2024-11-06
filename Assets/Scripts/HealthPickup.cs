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
    private static readonly string questionsAPIstr = "http://localhost:8080/api/v1/questions";
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
        // StartCoroutine(GetQuestions());
        questionDtos.AddRange(new List<QuestionDto>
    {
        new QuestionDto("What is the smallest planet in the Solar System?", new Dictionary<char, string> { { 'A', "Earth" }, { 'B', "Venus" }, { 'C', "Mercury" }, { 'D', "Mars" } }, 'C'),
        new QuestionDto("What is the capital of Canada?", new Dictionary<char, string> { { 'A', "Toronto" }, { 'B', "Ottawa" }, { 'C', "Vancouver" }, { 'D', "Montreal" } }, 'B'),
        new QuestionDto("Who painted the Mona Lisa?", new Dictionary<char, string> { { 'A', "Vincent van Gogh" }, { 'B', "Michelangelo" }, { 'C', "Leonardo da Vinci" }, { 'D', "Raphael" } }, 'C'),
        new QuestionDto("Which element has the atomic number 1?", new Dictionary<char, string> { { 'A', "Helium" }, { 'B', "Oxygen" }, { 'C', "Hydrogen" }, { 'D', "Nitrogen" } }, 'C'),
        new QuestionDto("How many continents are there?", new Dictionary<char, string> { { 'A', "5" }, { 'B', "6" }, { 'C', "7" }, { 'D', "8" } }, 'C'),
        new QuestionDto("What is the boiling point of water?", new Dictionary<char, string> { { 'A', "90°C" }, { 'B', "100°C" }, { 'C', "80°C" }, { 'D', "120°C" } }, 'B'),
        new QuestionDto("What is the hardest natural substance on Earth?", new Dictionary<char, string> { { 'A', "Gold" }, { 'B', "Iron" }, { 'C', "Diamond" }, { 'D', "Quartz" } }, 'C'),
        new QuestionDto("What is the capital of Japan?", new Dictionary<char, string> { { 'A', "Tokyo" }, { 'B', "Kyoto" }, { 'C', "Osaka" }, { 'D', "Nagoya" } }, 'A'),
        new QuestionDto("Which ocean is the largest?", new Dictionary<char, string> { { 'A', "Indian Ocean" }, { 'B', "Arctic Ocean" }, { 'C', "Pacific Ocean" }, { 'D', "Atlantic Ocean" } }, 'C'),
        new QuestionDto("What organ pumps blood through the human body?", new Dictionary<char, string> { { 'A', "Lungs" }, { 'B', "Heart" }, { 'C', "Kidney" }, { 'D', "Liver" } }, 'B'),
        new QuestionDto("What is the chemical symbol for water?", new Dictionary<char, string> { { 'A', "CO2" }, { 'B', "O2" }, { 'C', "H2O" }, { 'D', "N2" } }, 'C'),
        new QuestionDto("How many days are there in a leap year?", new Dictionary<char, string> { { 'A', "364" }, { 'B', "365" }, { 'C', "366" }, { 'D', "367" } }, 'C'),
        new QuestionDto("Who discovered penicillin?", new Dictionary<char, string> { { 'A', "Marie Curie" }, { 'B', "Albert Einstein" }, { 'C', "Alexander Fleming" }, { 'D', "Isaac Newton" } }, 'C'),
        new QuestionDto("What is the largest mammal?", new Dictionary<char, string> { { 'A', "Elephant" }, { 'B', "Blue Whale" }, { 'C', "Giraffe" }, { 'D', "Polar Bear" } }, 'B'),
        new QuestionDto("How many planets are in the Solar System?", new Dictionary<char, string> { { 'A', "7" }, { 'B', "8" }, { 'C', "9" }, { 'D', "10" } }, 'B'),
        new QuestionDto("Which continent is known as the birthplace of humanity?", new Dictionary<char, string> { { 'A', "Europe" }, { 'B', "Asia" }, { 'C', "Africa" }, { 'D', "Australia" } }, 'C'),
        new QuestionDto("What is the longest river in the world?", new Dictionary<char, string> { { 'A', "Amazon River" }, { 'B', "Yangtze River" }, { 'C', "Mississippi River" }, { 'D', "Nile River" } }, 'D'),
        new QuestionDto("How many bones are in the human body?", new Dictionary<char, string> { { 'A', "206" }, { 'B', "210" }, { 'C', "250" }, { 'D', "300" } }, 'A'),
        new QuestionDto("What is the powerhouse of the cell?", new Dictionary<char, string> { { 'A', "Nucleus" }, { 'B', "Ribosome" }, { 'C', "Mitochondria" }, { 'D', "Cytoplasm" } }, 'C'),
        new QuestionDto("Who wrote 'Romeo and Juliet'?", new Dictionary<char, string> { { 'A', "Charles Dickens" }, { 'B', "J.K. Rowling" }, { 'C', "William Shakespeare" }, { 'D', "George Orwell" } }, 'C')
    });




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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            //RANDOM TO SHOW OR GAIN HEALTH IMMEDIATELY
            bool doShow = r.Next(0, 2) == 0;
            doShow = true;
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
        // Select a random question from the local `questionDtos` list
        currentQuestion = questionDtos[r.Next(0, questionDtos.Count)];

        // Activate the UI elements
        subQuestionPanel.SetActive(true);
        AButton.gameObject.SetActive(true);
        BButton.gameObject.SetActive(true);
        CButton.gameObject.SetActive(true);
        DButton.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);

        // Display the question text and answer options
        questionText.text = currentQuestion.QuestionTxt;

        // Update button labels with the answer options
        TMP_Text aButtonText = AButton.GetComponentInChildren<TMP_Text>();
        TMP_Text bButtonText = BButton.GetComponentInChildren<TMP_Text>();
        TMP_Text cButtonText = CButton.GetComponentInChildren<TMP_Text>();
        TMP_Text dButtonText = DButton.GetComponentInChildren<TMP_Text>();

        aButtonText.text = currentQuestion.AnswerChoices['A'];
        bButtonText.text = currentQuestion.AnswerChoices['B'];
        cButtonText.text = currentQuestion.AnswerChoices['C'];
        dButtonText.text = currentQuestion.AnswerChoices['D'];

        // Adjust font size if necessary
        aButtonText.fontSize = 32;
        bButtonText.fontSize = 32;
        cButtonText.fontSize = 32;
        dButtonText.fontSize = 32;

        SetButtonBackgroundColor(AButton, Color.white);
        SetButtonBackgroundColor(BButton, Color.white);
        SetButtonBackgroundColor(CButton, Color.white);
        SetButtonBackgroundColor(DButton, Color.white);
    }

    void SetButtonBackgroundColor(Button button, Color color)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = color;
        colorBlock.highlightedColor = color;
        colorBlock.pressedColor = color;
        colorBlock.selectedColor = color;
        colorBlock.disabledColor = color;
        button.colors = colorBlock;
    }


    void AnswerQuestion(char answer, Damageable damageable)
    {
        if (currentQuestion == null)
        {
            Debug.LogError("currentQuestion is null!");
            return;
        }

        // existing code to check the answer
        if (answer == currentQuestion.CorrectAnswer)
        {
            answerCorrect = true;
        }
        questionAnswered = true;

        ContinueGame(damageable);
    }

    IEnumerator QuestionTimer(Damageable damageable)
    {
        yield return new WaitForSecondsRealtime(5);  // Wait for 10 real-time seconds (ignores Time.timeScale)

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

