using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiLevelController : MonoBehaviour
{
    [SerializeField] private GameObject generalUi;
    [SerializeField] private GameObject gameFinishedPanel;
    [SerializeField] private GameObject androidUi;
    [SerializeField] private TextMeshProUGUI timeText;

    //TO DO - represent time as for instance oxygen level
    [SerializeField] private Slider TankSlider;
    //TO DO - Equipment panel UI
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    
    private void Start()
    {
        if (gameFinishedPanel != null)
        {
            gameFinishedPanel.SetActive(false);
        }
        if (generalUi != null)
            {
                generalUi.SetActive(true);
            }
        if (equipmentPanel != null)
        {
            equipmentPanel.GetComponent<EquipmentManager>().SetGraphicRaycaster(graphicRaycaster);
            equipmentPanel.SetActive(false); //TO DO - Decide what to do with Equipment System
        }
        if (Application.platform == RuntimePlatform.Android && androidUi != null)
        {
            androidUi.SetActive(true);
        }
    }

    public void ShowGameFinishedPanel(bool won, bool timeRanOut = false)
    {
        timeText.text = "";
        gameFinishedPanel.SetActive(true);
        gameFinishedPanel.GetComponent<UiGamefFinishedController>().SetGameFinished(won, timeRanOut);
    }

    public void UpdateTime(float time, float maxTime)
    {
        timeText.text = string.Format("{0} / {1}",
            Mathf.Clamp(Mathf.CeilToInt(maxTime-time), 0, int.MaxValue).ToString(),
            Mathf.Clamp(Mathf.CeilToInt(maxTime), 0, int.MaxValue).ToString());
        UpdateTankSlider(time, maxTime);
    }

    public void UpdateTankSlider(float value, float maxValue)
    {
        TankSlider.value = Mathf.Clamp((maxValue - value)/maxValue,0,1);
    }

    public EquipmentManager GetEquipmentManager()
    {
        return equipmentPanel.GetComponent<EquipmentManager>();
    }
}