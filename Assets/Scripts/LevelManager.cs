using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

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

    public void LoadLevel(int level)
    {
        if (loadSceneCoroutine != null)
        {
            return;
        }
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public void LoadCurrentLevel()
    {
        if (loadSceneCoroutine != null)
        {
            return;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        if (loadSceneCoroutine != null)
        {
            loadSceneCoroutine = null;
            asyncLoad = null;
        }
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
