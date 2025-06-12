using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels = new List<GameObject>();
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject levelSelectionPanel;
    private int currentPanelIdx = 0;

    public void LoadLevel(int level)
    {
        LevelManager.Instance.LoadLevel(level, true);
    }

    public void GoFromMenuToLevelSelection()
    {
        if (panels.Count > 0)
        {
            menuPanel.SetActive(false);
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
