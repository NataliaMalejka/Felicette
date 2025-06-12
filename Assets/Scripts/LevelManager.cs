using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
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
        LoadLevel(1, true);
    }

    public void LoadLevel(int level, bool forceReload = false)
    {
        if (loadSceneCoroutine != null && !forceReload)
        {
            return;
        }
        loadSceneCoroutine = null;
        asyncLoad = null;
        if (level < 0 || level >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Scene with index {level} does not exist. There is {SceneManager.sceneCountInBuildSettings} scenes total");
            return;
        }
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public void LoadCurrentLevel(bool forceReload = false)
    {
        if (loadSceneCoroutine != null && !forceReload)
        {
            return;
        }
        loadSceneCoroutine = null;
        asyncLoad = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu(bool forceReload = false)
    {
        if (loadSceneCoroutine != null && !forceReload)
        {
            return;
        }
        loadSceneCoroutine = null;
        asyncLoad = null;
        SceneManager.LoadScene(0);
    }

    public int GetNextLevelId()
    {
        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevel >= SceneManager.sceneCountInBuildSettings)
        {
            return 0; // No more levels available, return to menu
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
    }
}
