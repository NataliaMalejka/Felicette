using UnityEngine;

// In contrary to default UI Button, this button can be held down and released
public class UiMobileControls : MonoBehaviour
{
    [SerializeField] MobileButton interactButton;
    [SerializeField] MobileButton restartButton;
    private GameObject player;
    void Start()
    {
        interactButton.OnButtonReleased.AddListener(onButtonInteract);
        restartButton.OnButtonReleased.AddListener(onButtonRestart);
        player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().OnInteractionPossibilityChanged.AddListener(interactButton.OnInteractionPossibilityChanged);
    }

    public void onButtonInteract()
    {
        player.GetComponent<Player>().OnInteractFromUI();
    }

    public void onButtonRestart()
    {
        player.GetComponent<Player>().OnRestartFromUI();
    }
}