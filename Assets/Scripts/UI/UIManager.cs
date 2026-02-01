using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Menu UI")]
    [SerializeField] private TextMeshProUGUI menuHighScoreText;
    [SerializeField] private Button playButton;

    [Header("Gameplay UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalHighScoreText;
    [SerializeField] private GameObject newRecordObject;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    private int previousHighScore;

    private void Start()
    {
        SetupButtons();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            GameManager.Instance.OnScoreChanged += HandleScoreChanged;

            previousHighScore = GameManager.Instance.HighScore;
            UpdateMenuHighScore();
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            GameManager.Instance.OnScoreChanged -= HandleScoreChanged;
        }
    }

    private void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryButtonClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButtonClicked);
        }
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        menuPanel?.SetActive(newState == GameManager.GameState.Menu);
        gameplayPanel?.SetActive(newState == GameManager.GameState.Playing);
        gameOverPanel?.SetActive(newState == GameManager.GameState.GameOver);

        switch (newState)
        {
            case GameManager.GameState.Menu:
                UpdateMenuHighScore();
                break;

            case GameManager.GameState.Playing:
                previousHighScore = GameManager.Instance.HighScore;
                UpdateScore(0);
                break;

            case GameManager.GameState.GameOver:
                ShowGameOverScreen();
                break;
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        UpdateScore(newScore);
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void UpdateMenuHighScore()
    {
        if (menuHighScoreText != null && GameManager.Instance != null)
        {
            menuHighScoreText.text = "Best: " + GameManager.Instance.HighScore;
        }
    }

    private void ShowGameOverScreen()
    {
        if (GameManager.Instance == null) return;

        int finalScore = GameManager.Instance.Score;
        int highScore = GameManager.Instance.HighScore;
        bool isNewRecord = finalScore > previousHighScore && finalScore == highScore;

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + finalScore;
        }

        if (finalHighScoreText != null)
        {
            finalHighScoreText.text = "Best: " + highScore;
        }

        if (newRecordObject != null)
        {
            newRecordObject.SetActive(isNewRecord);
        }
    }

    private void OnPlayButtonClicked()
    {
        GameManager.Instance?.StartGame();
    }

    private void OnRetryButtonClicked()
    {
        GameManager.Instance?.RestartGame();
    }

    private void OnMenuButtonClicked()
    {
        GameManager.Instance?.ReturnToMenu();
    }
}
