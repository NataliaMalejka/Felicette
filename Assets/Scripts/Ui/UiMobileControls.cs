using UnityEngine;

// In contrary to default UI Button, this button can be held down and released
public class UiMobileControls : MonoBehaviour
{
    [SerializeField] MobileButton moveLeftButton;
    [SerializeField] MobileButton moveRightButton;
    [SerializeField] MobileButton interactButton;
    [SerializeField] MobileButton restartButton;
    [SerializeField] MobileButton gravityButton;
    [SerializeField] GameObject holoPreview;
    private Player player;
    private bool moveLeftClicked = false;
    private bool moveRightClicked = false;
    void Start()
    {
        moveLeftButton.OnButtonPressed.AddListener(OnMoveLeftPressed);
        moveLeftButton.OnButtonReleased.AddListener(OnMoveLeftReleased);
        moveRightButton.OnButtonPressed.AddListener(OnMoveRightPressed);
        moveRightButton.OnButtonReleased.AddListener(OnMoveRightReleased);

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        interactButton.OnButtonReleased.AddListener(() => player.OnInteractFromUI());
        restartButton.OnButtonReleased.AddListener(() => player.OnRestartFromUI());
        gravityButton.OnButtonReleased.AddListener(() => player.OnGravityFromUI());
        
        // player.GetComponent<Player>().OnInteractionPossibilityChanged.AddListener(interactButton.OnInteractionPossibilityChanged);
        player.OnInteractionPossibilityChanged.AddListener(OnInteractionPossibilityChanged);

        player.OnPuzzlePickedUp.AddListener(ShowHoloPreview);
        player.OnPuzzlePutDown.AddListener(HideHoloPreview);
        holoPreview.SetActive(false);
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

    public void OnInteractionPossibilityChanged(bool canInteract, InteractiveType interactiveType)
    { 
        switch (interactiveType)
        {
            case InteractiveType.Gravity:
                gravityButton.OnInteractionPossibilityChanged(canInteract);
                break;
            case InteractiveType.Other:
                interactButton.OnInteractionPossibilityChanged(canInteract);
                break;
        }
    }

    public void OnMoveLeftPressed()
    {
        if (moveRightClicked)
        {
            return;
        }
        moveLeftClicked = true;
        player.OnMoveFromUI(moveLeftClicked, true);
    }

    public void OnMoveLeftReleased()
    {
        if (moveRightClicked)
        {
            return;
        }
        moveLeftClicked = false;
        player.OnMoveFromUI(moveLeftClicked, true);
    }

    public void OnMoveRightPressed()
    {
        if (moveLeftClicked)
        {
            return;
        }
        moveRightClicked = true;
        player.OnMoveFromUI(moveRightClicked, false);
    }

    public void OnMoveRightReleased()
    {
        if (moveLeftClicked)
        {
            return;
        }
        moveRightClicked = false;
        player.OnMoveFromUI(moveRightClicked, false);
    }
}