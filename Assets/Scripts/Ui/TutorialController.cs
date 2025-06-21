using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel_1;
    [SerializeField] private GameObject tutorialPanel_2;
    [SerializeField] private GameObject tutorialPanel_3;
    [SerializeField] private GameObject ui_UniversalPanel;
    [SerializeField] private GameObject ui_AndroidPanel;
    [SerializeField] private GameObject tutorialPuzzle;
    [SerializeField] private GameObject holoPanel;

    private int currentPanelIndex = 0;
    void Start()
    {
        tutorialPanel_1.SetActive(true);
        tutorialPanel_2.SetActive(false);
        tutorialPanel_3.SetActive(false);
        ui_UniversalPanel.SetActive(true);
        ui_AndroidPanel.SetActive(true);
        tutorialPuzzle.SetActive(false);
        holoPanel.SetActive(false);
    }

    public void OnNextTutorialPanel()
    {
        switch (currentPanelIndex)
        {
            case 0:
                tutorialPanel_1.SetActive(false);
                tutorialPanel_2.SetActive(true);
                ui_UniversalPanel.SetActive(false);
                foreach(RectTransform child in ui_AndroidPanel.GetComponentInChildren<RectTransform>())
                {
                        child.gameObject.SetActive(false);
                }
                holoPanel.SetActive(true);
                // ui_AndroidPanel.SetActive(false);
                tutorialPuzzle.SetActive(true);
                tutorialPuzzle.transform.position = new Vector3(4.3f, 0.9f, -0.5f);  
                // tutorialPuzzle.GetComponent<Puzzle>().ActOnPuzzlePicked();
                // tutorialPuzzle.GetComponent<Puzzle>().ChangeShaderColor(true);
                currentPanelIndex++;
                break;
            case 1:
                holoPanel.SetActive(false);
                tutorialPanel_2.SetActive(false);
                tutorialPanel_3.SetActive(true);
                tutorialPuzzle.SetActive(false);
                currentPanelIndex++;
                break;
            default:
                LevelManager.Instance.LoadMenu(true);
                break;
        }
    }
}
