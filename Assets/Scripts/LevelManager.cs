using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Tooltip("The index of the main menu scene in the build settings.")]
    [SerializeField]
    private int mainMenuSceneIndex = 0;

    [Tooltip("The index of the tutorial level scene in the build settings.")]
    [SerializeField]
    private int tutorialLevelSceneIndex = 1;

    [Tooltip("The index of the first level scene in the build settings. Next indexes must be used for subsequent levels.")]
    [SerializeField]
    private int firstLevelSceneIndex = 2;
    public int FirstLevelSceneIndex => firstLevelSceneIndex;

    private AsyncOperation asyncLoad;
    private Coroutine loadSceneCoroutine = null;
    private static LevelManager instance;
    public static LevelManager Instance
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadFirstLevel()
    {
        LoadLevel(firstLevelSceneIndex, true);
    }

    public void LoadCurrentLevel(bool forceReload = false)
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex, forceReload);
    }

    public void LoadMenu(bool forceReload = false)
    {
        LoadLevel(mainMenuSceneIndex, forceReload);
    }

    public void LoadTutorial(bool forceReload = false)
    {
        LoadLevel(tutorialLevelSceneIndex, forceReload);
    }

    public void LoadLevel(int level, bool forceReload = false)
    {
        EnsureForcedLoad(forceReload);
        if (level < 0 || level >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Scene with index {level} does not exist. There is {SceneManager.sceneCountInBuildSettings} scenes total");
            return;
        }
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public int GetNextLevelId()
    {
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevel >= SceneManager.sceneCountInBuildSettings)
        {
            return mainMenuSceneIndex; // No more levels available, return to menu
        }
        return nextLevel;
    }

    public void BeginLoadingScene(int level)
    {
        if (loadSceneCoroutine != null)
        {
            return;
        }
        if (level < 0 || level >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Scene with index {level} does not exist. There is {SceneManager.sceneCountInBuildSettings} scenes total");
            return;
        }
        loadSceneCoroutine = StartCoroutine(LoadSceneAsync(level));
    }

    private IEnumerator LoadSceneAsync(int level)
    {
        asyncLoad = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        Debug.Log("Scene loaded and ready to activate.");
    }

    private void EnsureForcedLoad(bool forceReload)
    {
        if (loadSceneCoroutine != null && !forceReload)
        {
            return;
        }
        loadSceneCoroutine = null;
        asyncLoad = null;
    }

    public void ActivateScene()
    {
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = true;
            asyncLoad.completed += (AsyncOperation op) =>
            {
                Debug.Log("Scene activated.");
                asyncLoad = null;
            };
            loadSceneCoroutine = null;
        }
        else
        {
            Debug.LogWarning("Scene not loaded yet.");
        }
    }

    public void ExitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
