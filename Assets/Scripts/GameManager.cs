using UnityEngine;
using UnityEngine.Events;
public enum GameState{
    Start,
    GameActive,
    GameLost,
    Pause,
    GameWon,
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private float maxTime = 100f;
    private float time = 0f;
    [SerializeField] private UiLevelController uiLevelController;
    private GameState gameState = GameState.Start;
    public GameState State
    {
        get
        {
            return gameState;
        }
    }

    public bool isPerspectiveOnWall = true;

    public UnityEvent OnGameStarted;
    public UnityEvent OnGameWon;
    public UnityEvent OnGameLost;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameState = GameState.GameActive;
        isPerspectiveOnWall = true;
        time = 0f;
    }

    public void PauseGame()
    {
        if (gameState == GameState.GameActive)
        {
            gameState = GameState.Pause;
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TimerStop);
            uiLevelController.ShowGameFinishedPanel(false);
            OnGamePaused?.Invoke();
            Debug.LogWarning("Not implemented yet: PauseGame() in GameManager");
        }
    }

    public void ResumeGame()
    {
        if (gameState == GameState.Pause)
        {
            gameState = GameState.GameActive;
            OnGameResumed?.Invoke();
            uiLevelController.ShowGameFinishedPanel(false);
            Debug.LogWarning("Not implemented yet: ResumeGame() in GameManager");
        }
    }

    public void ActOnGameOver()
    {
        if (gameState == GameState.GameActive)
        {
            gameState = GameState.GameLost;
            OnGameLost?.Invoke();
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TimerStop);
            uiLevelController.ShowGameFinishedPanel(false, maxTime <= time);
        }
    }

    public void ActOnGameWon()
    {
       if (gameState == GameState.GameActive)
        {
            gameState = GameState.GameWon;
            OnGameWon?.Invoke();
            LevelManager.Instance.BeginLoadingScene(LevelManager.Instance.GetNextLevelId());
            uiLevelController.ShowGameFinishedPanel(true);
        }
    }

    private void Update()
    {
        if (gameState == GameState.Start)
        {
            gameState = GameState.GameActive;
            OnGameStarted?.Invoke();
        }

        if (gameState == GameState.GameActive && maxTime > time)
        {
            time += Time.deltaTime;
            uiLevelController.UpdateTime(time, maxTime);
        }
        else if (gameState == GameState.GameActive && maxTime <= time)
        {
            ActOnGameOver();
        }
    }

    public UiLevelController GetUiControllerHandle() { return uiLevelController; }
}
