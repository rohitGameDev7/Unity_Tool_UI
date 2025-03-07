using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public QuestionData[] categories; // All categories (Scriptable Objects)
    private QuestionData selectedCategory;
    private int currentQuestionIndex = 0;
    private bool isAnswering = false;

    [Header("UI Elements")]
    public TMP_Text questionText;
    public Image questionImage;
    public Button[] replyButtons;

    [Header("Score")]
    public Score score;
    public int correctReplyPoints = 10;
    public int wrongReplyPoints = 5;
    public TextMeshProUGUI scoreText;

    [Header("Game Finished Panel")]
    public GameObject gameFinishedPanel;

    private int correctReplies;

    void Start()
    {
        int selectedCategoryIndex = PlayerPrefs.GetInt("SelectedCategory", 0);
        gameFinishedPanel.SetActive(false);
        SelectCategory(selectedCategoryIndex);
        LoadProgress(selectedCategoryIndex);

        // Attach listeners once to avoid multiple re-additions
        for (int i = 0; i < replyButtons.Length; i++)
        {
            int index = i; // Fix closure issue
            replyButtons[i].onClick.AddListener(() => OnReplySelected(index));
        }
    }

    public void SelectCategory(int categoryIndex)
    {
        if (categoryIndex < 0 || categoryIndex >= categories.Length)
        {
            Debug.LogError("Invalid category index.");
            return;
        }

        selectedCategory = categories[categoryIndex];
        currentQuestionIndex = 0;
        isAnswering = false;

        EnableButtons();
        DisplayQuestion();
    }

    private void DisplayQuestion()
    {
        if (selectedCategory == null || currentQuestionIndex >= selectedCategory.questions.Length)
        {
            Debug.Log("Quiz Finished!");
            ShowGameFinishedPanel();
            return;
        }

        ResetButtons();
        isAnswering = false;

        var question = selectedCategory.questions[currentQuestionIndex];
        questionText.text = question.questionText;

        if (question.questionImage != null)
        {
            questionImage.gameObject.SetActive(true);
            questionImage.sprite = question.questionImage;
        }
        else
        {
            questionImage.gameObject.SetActive(false);
        }

        for (int i = 0; i < replyButtons.Length; i++)
        {
            if (i < question.replies.Length)
            {
                replyButtons[i].gameObject.SetActive(true);
                replyButtons[i].GetComponentInChildren<TMP_Text>().text = question.replies[i];
            }
            else
            {
                replyButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnReplySelected(int replyIndex)
    {
        if (isAnswering) return;
        isAnswering = true;

        if (selectedCategory == null || currentQuestionIndex >= selectedCategory.questions.Length)
        {
            Debug.Log("Quiz Finished!");
            return;
        }

        DisableButtons();

        bool isCorrect = replyIndex == selectedCategory.questions[currentQuestionIndex].correctReplyIndex;
        if (isCorrect)
        {
            score.AddScore(correctReplyPoints);
            correctReplies++;
            HighlightButton(replyButtons[replyIndex], Color.green);
            Debug.Log("Correct Answer!");
        }
        else
        {
            score.SubtractScore(wrongReplyPoints);
            HighlightButton(replyButtons[replyIndex], Color.red);
            Debug.Log("Wrong Answer!");
        }

        StartCoroutine(NextQuestion());
        SaveProgress();
    }

    private IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(0.5f);

        currentQuestionIndex++;

        if (currentQuestionIndex < selectedCategory.questions.Length)
        {
            DisplayQuestion();
        }
        else
        {
            ShowGameFinishedPanel();
        }
    }

    private void HighlightButton(Button button, Color color)
    {
        Image buttonImage = button.GetComponent<Image>();
        Color originalColor = buttonImage.color;
        buttonImage.color = color;

        StartCoroutine(ResetButtonColor(buttonImage, originalColor, 0.5f));
    }

    private IEnumerator ResetButtonColor(Image buttonImage, Color originalColor, float delay)
    {
        yield return new WaitForSeconds(delay);
        buttonImage.color = originalColor;
    }

    private void DisableButtons()
    {
        foreach (var button in replyButtons)
        {
            button.interactable = false;
        }
    }

    private void EnableButtons()
    {
        foreach (var button in replyButtons)
        {
            button.interactable = true;
        }
    }

    private void ResetButtons()
    {
        foreach (var button in replyButtons)
        {
            button.interactable = true;
            button.GetComponent<Image>().color = Color.white; // Reset button color
        }
    }

    public void ShowCorrectReply()
    {
        int correctIndex = selectedCategory.questions[currentQuestionIndex].correctReplyIndex;

        for (int i = 0; i < replyButtons.Length; i++)
        {
            replyButtons[i].interactable = (i == correctIndex);
        }
    }

    public void ShowGameFinishedPanel()
    {
        gameFinishedPanel.SetActive(true);
        scoreText.text = $"{correctReplies} / {selectedCategory.questions.Length}";
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("LastQuestionIndex_" + selectedCategory.name, currentQuestionIndex);
        PlayerPrefs.Save();
    }

    private void LoadProgress(int categoryIndex)
    {
        string categoryName = categories[categoryIndex].name;
        currentQuestionIndex = PlayerPrefs.GetInt("LastQuestionIndex_" + categoryName, 0);
        DisplayQuestion();
    }
}
