using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, GameOver }

    // Events
    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnScoreChanged;
    public event Action<float, float> OnDifficultyChanged;

    // Properties
    public GameState CurrentState { get; private set; } = GameState.Menu;
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int GamesPlayed { get; private set; }
    public float CurrentFallSpeed { get; private set; }
    public float CurrentGapSize { get; private set; }

    // Difficulty settings
    [Header("Difficulty Settings")]
    [SerializeField] private float baseFallSpeed = 3f;
    [SerializeField] private float maxFallSpeed = 8f;
    [SerializeField] private float initialGapSize = 2.0f;
    [SerializeField] private float minGapSize = 1.2f;
    [SerializeField] private int maxDifficultyScore = 100;

    // PlayerPrefs keys
    private const string HIGH_SCORE_KEY = "JumpyPro_HighScore";
    private const string GAMES_PLAYED_KEY = "JumpyPro_GamesPlayed";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSavedData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetState(GameState.Menu);
    }

    private void LoadSavedData()
    {
        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        GamesPlayed = PlayerPrefs.GetInt(GAMES_PLAYED_KEY, 0);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
        PlayerPrefs.SetInt(GAMES_PLAYED_KEY, GamesPlayed);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        Score = 0;
        ResetDifficulty();
        SetState(GameState.Playing);
        OnScoreChanged?.Invoke(Score);
    }

    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        GamesPlayed++;

        if (Score > HighScore)
        {
            HighScore = Score;
        }

        SaveData();
        SetState(GameState.GameOver);
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void ReturnToMenu()
    {
        SetState(GameState.Menu);
    }

    public void AddScore(int points = 1)
    {
        if (CurrentState != GameState.Playing) return;

        Score += points;
        OnScoreChanged?.Invoke(Score);
        UpdateDifficulty();
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Menu:
                Time.timeScale = 1f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.GameOver:
                Time.timeScale = 1f;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void ResetDifficulty()
    {
        CurrentFallSpeed = baseFallSpeed;
        CurrentGapSize = initialGapSize;
        OnDifficultyChanged?.Invoke(CurrentFallSpeed, CurrentGapSize);
    }

    private void UpdateDifficulty()
    {
        float progress = Mathf.Clamp01((float)Score / maxDifficultyScore);

        CurrentFallSpeed = Mathf.Lerp(baseFallSpeed, maxFallSpeed, progress);
        CurrentGapSize = Mathf.Lerp(initialGapSize, minGapSize, progress);

        OnDifficultyChanged?.Invoke(CurrentFallSpeed, CurrentGapSize);
    }

    public bool ShouldShowInterstitial()
    {
        return GamesPlayed > 0 && GamesPlayed % 3 == 0;
    }
}
