using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiGamefFinishedController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI subText;

    [SerializeField] private string GameWonMessage = "Mission Accomplished!";
    [SerializeField] private string GameWonMessageSubText = "Pad your way towards the Bridge";
    [SerializeField] private string GameLostMessage = "Catastrophe! Mission Failed!";
    [SerializeField] private string GameLostMessageSubText1 = "Oxygen Depleted";
    [SerializeField] private string GameLostMessageSubText2 = "Head to Medical Bay immediately";
    [SerializeField] private Button buttonNextLevel;

    public void SetGameFinished(bool isWin, bool timeRanOut = false)
    {
        buttonNextLevel.interactable = isWin;
        if (isWin)
        {
            infoText.text = GameWonMessage;
            subText.text = GameWonMessageSubText;
        }
        else
        {
            infoText.text = GameLostMessage;
            subText.text = timeRanOut ? GameLostMessageSubText1 : GameLostMessageSubText2;
        }
    }

    public void GoToNextLevel()
    {
        LevelManager.Instance.ActivateScene();
    }

    public void GoToMainMenu()
    {
        LevelManager.Instance.LoadMenu(true);
    }

    public void RestartLevel()
    {
        LevelManager.Instance.LoadCurrentLevel(true);
    }
}
