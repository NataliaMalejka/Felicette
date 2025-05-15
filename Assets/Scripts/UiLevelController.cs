using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiLevelController : MonoBehaviour
{
    [SerializeField] private GameObject generalUi;
    [SerializeField] private GameObject gameOverPanel; //TO DO - Or pause panel?
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject androidUi;
    [SerializeField] private TextMeshProUGUI timeText;

    //TO DO - represent time as for instance oxygen level
    [SerializeField] private Slider TankSlider;
    //TO DO - Equipment panel UI
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    
    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        if (generalUi != null)
        {
            generalUi.SetActive(true);
        }
        if (equipmentPanel != null)
        {
            equipmentPanel.SetActive(true);
            equipmentPanel.GetComponent<EquipmentManager>().SetGraphicRaycaster(graphicRaycaster);
        }
        if (Application.platform == RuntimePlatform.Android && androidUi != null)
        {
            androidUi.SetActive(true);
        }
    }

    public void ShowGameOverPanel(bool hide = false)
    {
        timeText.text = "";
        gameOverPanel.SetActive(!hide);
    }

    public void UpdateTime(float time, float maxTime)
    {
        timeText.text = string.Format("Time: {0} / {1}",
            Mathf.Clamp(Mathf.CeilToInt(time), 0, int.MaxValue).ToString(),
            Mathf.Clamp(Mathf.CeilToInt(maxTime), 0, int.MaxValue).ToString());
        UpdateTankSlider(time, maxTime);
    }

    public void ShowWinPanel(float timeLeft)
    {
        winPanel.SetActive(true);
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