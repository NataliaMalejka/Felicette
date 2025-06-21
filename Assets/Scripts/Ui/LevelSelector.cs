using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels = new List<GameObject>();
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private GameObject logoText;
    [SerializeField] private GameObject levelSelectionText;

    private int currentPanelIdx = 0;

    public void LoadLevel(int level)
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.FirstLevelSceneIndex + level, true);
    }

    public void LoadTutorial()
    {
        LevelManager.Instance.LoadTutorial(true);
    }
    public void LoadMenu()
    {
        LevelManager.Instance.LoadMenu(true);
    }
    public void GameQuit()
    {
        LevelManager.Instance.ExitGame();
    }


    public void GoFromMenuToLevelSelection()
    {
        if (panels.Count > 0)
        {
            menuPanel.SetActive(false);
            logoText.SetActive(false);
            levelSelectionText.SetActive(true);
            levelSelectionPanel.SetActive(true);
            foreach (GameObject panel in panels)
            {
                panel.SetActive(false);
            }
            currentPanelIdx = 0;
            panels[currentPanelIdx].SetActive(true);
        }
    }

    public void GoFromLevelSelectionToMenu()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
        menuPanel.SetActive(true);
        logoText.SetActive(true);
        levelSelectionText.SetActive(false);
        levelSelectionPanel.SetActive(false);
        currentPanelIdx = 0;
    }

    public void ChangeSlide(bool changeLeft)
    {
        panels[currentPanelIdx].SetActive(false);
        currentPanelIdx += changeLeft ? -1 : 1;
        if (currentPanelIdx < 0)
        {
            currentPanelIdx = panels.Count - 1;
        }
        else if (currentPanelIdx >= panels.Count)
        {
            currentPanelIdx = 0;
        }
        panels[currentPanelIdx].SetActive(true);
    }
}
