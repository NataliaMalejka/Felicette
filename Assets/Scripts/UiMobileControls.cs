using UnityEngine;

// In contrary to default UI Button, this button can be held down and released
public class UiMobileControls : MonoBehaviour
{
    [SerializeField] MobileButton interactButton;
    [SerializeField] MobileButton pauseButton;
    private GameObject player;
    void Start()
    {
        interactButton.OnButtonReleased.AddListener(onButtonInteract);
        // pauseButton.OnButtonPressed.AddListener(GameManager.Instance.PauseGame);
        player = GameObject.FindWithTag("Player");
    }

    public void onButtonInteract()
    {
        player.GetComponent<Player>().OnInteractFromUI();
    }
}