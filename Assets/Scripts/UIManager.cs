using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject hudPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;

    [Header("HUD")]
    [SerializeField] PlayerController player;
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] Slider fuelBar;
    [SerializeField] Image fuelFill;
    [SerializeField] Image[] lifeIcons;           // 3 icons

    [Header("Menus")]
    [SerializeField] TMP_Text menuHighScoreText;
    [SerializeField] TMP_Text finalScoreText;
    [SerializeField] TMP_Text finalHighScoreText;
    [SerializeField] TMP_Text gameOverReasonText; // optional

    [SerializeField] Color fuelOk = new Color(0.2f, 1f, 0.5f);
    [SerializeField] Color fuelLow = new Color(1f, 0.2f, 0.2f);

    GameManager gm;

    void Start()
    {
        gm = GameManager.Instance;
        gm.OnStateChanged += Apply;
        Apply(gm.State);                           // in case the event already fired
    }

    void OnDestroy()
    {
        if (gm != null) gm.OnStateChanged -= Apply;
    }

    void Apply(GameState s)
    {
        mainMenuPanel.SetActive(s == GameState.Menu);
        hudPanel.SetActive(s == GameState.Playing || s == GameState.Paused);
        pausePanel.SetActive(s == GameState.Paused);
        gameOverPanel.SetActive(s == GameState.GameOver);

        if (menuHighScoreText) menuHighScoreText.text = $"Best: {gm.HighScore}";
        if (s == GameState.GameOver)
        {
            finalScoreText.text = $"Score: {gm.Score}";
            finalHighScoreText.text = $"Best: {gm.HighScore}";
            if (gameOverReasonText)
                gameOverReasonText.text = player.Fuel <= 0f ? "Out of fuel!" : "You crashed!";
        }
    }

    void Update()
    {
        if (gm.State != GameState.Playing && gm.State != GameState.Paused) return;

        speedText.text = $"{Mathf.RoundToInt(gm.Speed * 3.6f)} km/h";
        scoreText.text = $"Score: {gm.Score}";
        highScoreText.text = $"Best: {Mathf.Max(gm.HighScore, gm.Score)}";

        fuelBar.value = player.Fuel01;
        if (fuelFill) fuelFill.color = Color.Lerp(fuelLow, fuelOk, Mathf.InverseLerp(0.15f, 0.4f, player.Fuel01));

        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].enabled = i < player.Lives;
    }

    // ---------- Button callbacks (hook these up in the Inspector) ----------
    public void OnStartClicked() => gm.StartGame();
    public void OnResumeClicked() => gm.ResumeGame();
    public void OnRestartClicked() => gm.RestartGame();
    public void OnMainMenuClicked() => gm.ReturnToMenu();
    public void OnQuitClicked() => gm.QuitGame();
    public void OnPauseClicked() => gm.PauseGame();
}
