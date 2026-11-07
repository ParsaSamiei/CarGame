using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused, GameOver }

/// <summary>Singleton that owns the game state machine, speed/difficulty, score and high score.</summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<GameState> OnStateChanged;

    const string HighScoreKey = "HighScore";
    static bool skipMenuOnLoad;               // survives scene reload (used by Restart)

    [Header("Input (New Input System)")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Speed & Difficulty (m/s)")]
    public float startSpeed = 18f;
    public float maxSpeed = 45f;
    [Tooltip("Speed gained per metre travelled")]
    public float speedGainPerMeter = 0.008f;
    public float scorePerMeter = 0.5f;

    public GameState State { get; private set; } = GameState.Menu;
    public float Speed { get; private set; }
    public float Distance { get; private set; }
    public int Score => Mathf.FloorToInt(scoreFloat);
    public int HighScore { get; private set; }
    public float Speed01 => Mathf.Clamp01(Speed / maxSpeed);                       // for neon / audio / FOV
    public float Difficulty01 => Mathf.InverseLerp(startSpeed, maxSpeed, Speed);  // for spawn density

    float scoreFloat;
    InputAction pauseAction;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    void OnEnable()
    {
        var map = inputActions.FindActionMap("Player", true);
        pauseAction = map.FindAction("Pause", true);
        pauseAction.performed += OnPausePerformed;
        map.Enable();
    }

    void OnDisable()
    {
        if (pauseAction != null) pauseAction.performed -= OnPausePerformed;
    }

    void Start()
    {
        if (skipMenuOnLoad) { skipMenuOnLoad = false; StartGame(); }
        else SetState(GameState.Menu);
    }

    void Update()
    {
        if (State != GameState.Playing) return;
        Speed = Mathf.Min(maxSpeed, startSpeed + Distance * speedGainPerMeter);
        float step = Speed * Time.deltaTime;
        Distance += step;
        scoreFloat += step * scorePerMeter;
    }

    void OnPausePerformed(InputAction.CallbackContext _)
    {
        if (State == GameState.Playing) PauseGame();
        else if (State == GameState.Paused) ResumeGame();
    }

    void SetState(GameState s)
    {
        State = s;
        OnStateChanged?.Invoke(s);
    }

    // ---------- public API (called by UI buttons / gameplay) ----------
    public void StartGame()
    {
        Distance = 0; scoreFloat = 0; Speed = startSpeed;
        Time.timeScale = 1f; AudioListener.pause = false;
        SetState(GameState.Playing);
    }

    public void PauseGame()
    {
        if (State != GameState.Playing) return;
        Time.timeScale = 0f; AudioListener.pause = true;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        Time.timeScale = 1f; AudioListener.pause = false;
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        if (State == GameState.GameOver) return;
        Speed = 0f;
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
        SetState(GameState.GameOver);
    }

    public void AddScore(int amount) => scoreFloat += amount;

    public void RestartGame()
    {
        skipMenuOnLoad = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        skipMenuOnLoad = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
