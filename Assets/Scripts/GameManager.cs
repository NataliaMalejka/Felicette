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
    public UnityEvent<bool> OnPuzzleHint;

    bool initialHintShown = false;
    bool secondHintShown = false;
    float halfMaxTime = 0f;
    float percent2MaxTime = 0f;

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
        gameState = GameState.Start;
        isPerspectiveOnWall = true;
        time = 0f;
        halfMaxTime = maxTime/2f;
        percent2MaxTime = maxTime*0.01f;

        if (LevelManager.Instance == null)
        {
            Debug.LogWarning("LevelManager instance is not set. Please ensure LevelManager is initialized. Start from Menu Level");
            return;
        }
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

    public void RestartLevel()
    {
        if (gameState == GameState.GameActive && gameState != GameState.GameLost && gameState != GameState.GameWon)
        {
            gameState = GameState.Pause;
            LevelManager.Instance.LoadCurrentLevel(true);
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
            if(!initialHintShown)
            {
                if(time == 0f)
                {
                    OnPuzzleHint?.Invoke(true);
                    initialHintShown = true;
                }
                // else if(time > 1.5f)
                // {
                //     OnPuzzleHint?.Invoke(false);
                //     initialHintShown = true;
                // }
            }
            // else if(!secondHintShown)
            // {
            //     if(time > halfMaxTime && time < halfMaxTime+percent2MaxTime)
            //     {
            //         OnPuzzleHint?.Invoke(true);
            //     }
            //     else if(time > (halfMaxTime+1.5f))
            //     {
            //         OnPuzzleHint?.Invoke(false);
            //         secondHintShown = true;
            //     }
            // }
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
