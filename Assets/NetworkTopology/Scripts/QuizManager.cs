
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string question;
        public string[] answers;
        public int correctAnswer;

        public QuizQuestion(string q, string a, string b,
                            string c, string d, int correct)
        {
            question = q;
            answers = new string[] { a, b, c, d };
            correctAnswer = correct;
        }
    }

    [Header("Panels")]
    public GameObject quizPanel;
    public GameObject resultPanel;

    [Header("Quiz Text")]
    public TMP_Text questionText;
    public TMP_Text progressText;
    public TMP_Text scoreText;

    [Header("Answer Buttons")]
    public Button[] answerButtons = new Button[4];

    [Header("Other Buttons")]
    public Button nextButton;

    [Header("Result Text")]
    public TMP_Text finalScoreText;
    public TMP_Text resultMessageText;

    private List<QuizQuestion> questions =
        new List<QuizQuestion>();

    private int currentQuestion = 0;
    private int score = 0;
    private bool answered = false;

    private Color normalColor =
        new Color32(37, 99, 235, 255);

    private Color correctColor =
        new Color32(22, 163, 74, 255);

    private Color wrongColor =
        new Color32(239, 68, 68, 255);

    void Awake()
    {
        LoadQuestions();
    }

    void Start()
    {
        quizPanel.SetActive(false);
        resultPanel.SetActive(false);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(
                () => SelectAnswer(index));
        }

        nextButton.onClick.AddListener(NextQuestion);
    }

    public void OpenQuiz()
    {
        currentQuestion = 0;
        score = 0;

        quizPanel.SetActive(true);
        resultPanel.SetActive(false);

        ShowQuestion();
    }

    public void CloseQuiz()
    {
        quizPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    void ShowQuestion()
    {
        answered = false;

        QuizQuestion q = questions[currentQuestion];

        questionText.text = q.question;

        progressText.text =
            "Question " + (currentQuestion + 1) +
            " / " + questions.Count;

        scoreText.text = "Score: " + score;

        for (int i = 0; i < 4; i++)
        {
            Button button = answerButtons[i];

            button.interactable = true;

            TMP_Text label =
                button.GetComponentInChildren<TMP_Text>();

            label.text =
                ((char)('A' + i)) + ". " + q.answers[i];

            SetButtonColor(button, normalColor);
        }

        nextButton.interactable = false;
    }

    public void SelectAnswer(int selectedIndex)
    {
        if (answered)
            return;

        answered = true;

        QuizQuestion q = questions[currentQuestion];

        bool correct =
            selectedIndex == q.correctAnswer;

        if (correct)
            score++;

        scoreText.text = "Score: " + score;

        for (int i = 0; i < 4; i++)
        {
            

            if (i == q.correctAnswer)
            {
                SetButtonColor(
                    answerButtons[i], correctColor);
            }
            else if (i == selectedIndex)
            {
                SetButtonColor(
                    answerButtons[i], wrongColor);
            }
        }

        nextButton.interactable = true;
    }

    void NextQuestion()
    {
        if (!answered)
            return;

        currentQuestion++;

        if (currentQuestion >= questions.Count)
        {
            ShowResults();
        }
        else
        {
            ShowQuestion();
        }
    }

    void ShowResults()
    {
        resultPanel.SetActive(true);

        finalScoreText.text =
            "Your Score: " + score +
            " / " + questions.Count;

        float percentage =
            (float)score / questions.Count * 100f;

        if (percentage >= 80)
            resultMessageText.text =
                "Excellent! Great job!";
        else if (percentage >= 50)
            resultMessageText.text =
                "Good work! Keep learning!";
        else
            resultMessageText.text =
                "Keep practicing network topologies!";
    }

    public void RestartQuiz()
    {
        OpenQuiz();
    }

    
void SetButtonColor(Button button, Color color)
{
    Image image = button.GetComponent<Image>();

    if (image != null)
    {
        image.color = color;
    }

    ColorBlock colors = button.colors;

    colors.normalColor = Color.white;
    colors.highlightedColor = Color.white;
    colors.pressedColor = Color.white;
    colors.selectedColor = Color.white;
    colors.disabledColor = Color.white;

    button.colors = colors;
}


    void LoadQuestions()
    {
        questions.Add(new QuizQuestion(
            "Which topology uses a central switch?",
            "Bus", "Star", "Ring", "Mesh", 1));

        questions.Add(new QuizQuestion(
            "Which topology uses one main cable?",
            "Star", "Tree", "Bus", "Mesh", 2));

        questions.Add(new QuizQuestion(
            "Which topology connects devices in a circle?",
            "Ring", "Star", "Bus", "Tree", 0));

        questions.Add(new QuizQuestion(
            "Which topology provides multiple paths?",
            "Bus", "Ring", "Star", "Mesh", 3));

        questions.Add(new QuizQuestion(
            "Which topology has a hierarchical structure?",
            "Mesh", "Tree", "Ring", "Bus", 1));

        questions.Add(new QuizQuestion(
            "What happens if the central switch fails in Star?",
            "All devices lose connectivity",
            "Nothing happens",
            "Only one PC fails",
            "Network becomes Mesh", 0));

        questions.Add(new QuizQuestion(
            "Which topology is most affected by backbone failure?",
            "Mesh", "Star", "Bus", "Tree", 2));

        questions.Add(new QuizQuestion(
            "Which topology has the most direct links in full Mesh?",
            "Bus", "Ring", "Mesh", "Star", 2));

        questions.Add(new QuizQuestion(
            "Which topology is usually easiest to expand?",
            "Star", "Ring", "Bus", "Full Mesh", 0));

        questions.Add(new QuizQuestion(
            "What is the main advantage of Mesh topology?",
            "Low cable usage",
            "Multiple alternative paths",
            "Simple wiring",
            "One main cable", 1));

        questions.Add(new QuizQuestion(
            "Which topology commonly uses a backbone cable?",
            "Ring", "Mesh", "Bus", "Star", 2));

        questions.Add(new QuizQuestion(
            "What is a disadvantage of full Mesh?",
            "No alternative paths",
            "High cabling cost",
            "Needs one cable only",
            "Cannot connect computers", 1));

        questions.Add(new QuizQuestion(
            "Which topology connects every PC to a central device?",
            "Star", "Bus", "Ring", "Tree", 0));

        questions.Add(new QuizQuestion(
            "Which topology can use parent-child connections?",
            "Mesh", "Bus", "Tree", "Ring", 2));

        questions.Add(new QuizQuestion(
            "In a basic Ring, how are devices connected?",
            "To one central switch",
            "In a closed loop",
            "To one backbone only",
            "Every device to every device", 1));

        questions.Add(new QuizQuestion(
            "What does rerouting mean in a network?",
            "Deleting a PC",
            "Changing the network name",
            "Using another available path",
            "Turning off every cable", 2));

        questions.Add(new QuizQuestion(
            "Which topology has a central point of failure?",
            "Star", "Full Mesh", "Bus", "Ring", 0));

        questions.Add(new QuizQuestion(
            "What is a benefit of Tree topology?",
            "No switches required",
            "Supports hierarchical expansion",
            "Every PC connects directly",
            "No cables needed", 1));

        questions.Add(new QuizQuestion(
            "What happens when a link fails in full Mesh?",
            "All PCs always disconnect",
            "Alternative paths may be available",
            "All switches shut down",
            "The topology becomes Bus", 1));

        questions.Add(new QuizQuestion(
            "How many topologies are in this AR application?",
            "Three", "Four", "Five", "Six", 2));
    }
}
