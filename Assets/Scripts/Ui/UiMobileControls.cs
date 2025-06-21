using UnityEngine;

// In contrary to default UI Button, this button can be held down and released
public class UiMobileControls : MonoBehaviour
{
    [SerializeField] MobileButton interactButton;
    [SerializeField] MobileButton restartButton;
    [SerializeField] GameObject holoPreview;
    private GameObject player;
    void Start()
    {
        interactButton.OnButtonReleased.AddListener(onButtonInteract);
        restartButton.OnButtonReleased.AddListener(onButtonRestart);
        player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().OnInteractionPossibilityChanged.AddListener(interactButton.OnInteractionPossibilityChanged);

        player.GetComponent<Player>().OnPuzzlePickedUp.AddListener(ShowHoloPreview);
        player.GetComponent<Player>().OnPuzzlePutDown.AddListener(HideHoloPreview);
        holoPreview.SetActive(false);
    }

    public void onButtonInteract()
    {
        player.GetComponent<Player>().OnInteractFromUI();
    }

    public void onButtonRestart()
    {
        player.GetComponent<Player>().OnRestartFromUI();
    }

    public void ShowHoloPreview(Sprite previewSprite, Sprite previewChildSprite = null, float rotation = 0f)
    {
        holoPreview.SetActive(true);
        restartButton.gameObject.SetActive(false);
        holoPreview.GetComponent<HoloUIPreview>().ActivateHoloPreview(previewSprite, previewChildSprite, rotation);
    }

    public void HideHoloPreview()
    {
        holoPreview.SetActive(false);
        restartButton.gameObject.SetActive(true);
        holoPreview.GetComponent<HoloUIPreview>().DeactivateHoloPreview();
    }
}