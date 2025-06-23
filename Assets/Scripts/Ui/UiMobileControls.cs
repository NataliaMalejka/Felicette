using System.Collections.Generic;
using UnityEngine;

enum UiMoveDirection
{
    N,
    S,
    E,
    W,
    NE,
    NW,
    SE,
    SW,
    None
}

// In contrary to default UI Button, this button can be held down and released
public class UiMobileControls : MonoBehaviour
{
    [SerializeField] MobileButton moveLeftButton;
    [SerializeField] MobileButton moveRightButton;
    [SerializeField] MobileButton moveUpButton;
    [SerializeField] MobileButton moveDownButton;
    [SerializeField] MobileButton interactButton;
    [SerializeField] MobileButton restartButton;
    [SerializeField] MobileButton gravityButton;
    [SerializeField] GameObject holoPreview;
    [SerializeField] List<GameObject> arrowsToggleObjects = new List<GameObject>();
    private Player player;
    private bool visibleAllArrows = false;
    private UiMoveDirection moveDirection = UiMoveDirection.None;
    void Start()
    {
        moveLeftButton.OnButtonPressed.AddListener(OnMoveLeftPressed);
        moveLeftButton.OnButtonReleased.AddListener(OnMoveLeftReleased);
        moveRightButton.OnButtonPressed.AddListener(OnMoveRightPressed);
        moveRightButton.OnButtonReleased.AddListener(OnMoveRightReleased);
        moveUpButton.OnButtonPressed.AddListener(OnMoveUpPressed);
        moveUpButton.OnButtonReleased.AddListener(OnMoveUpReleased);
        moveDownButton.OnButtonPressed.AddListener(OnMoveDownPressed);
        moveDownButton.OnButtonReleased.AddListener(OnMoveDownReleased);

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        interactButton.OnButtonReleased.AddListener(() => player.OnInteractFromUI());
        restartButton.OnButtonReleased.AddListener(() => player.OnRestartFromUI());
        gravityButton.OnButtonReleased.AddListener(() => player.OnGravityFromUI());

        // player.GetComponent<Player>().OnInteractionPossibilityChanged.AddListener(interactButton.OnInteractionPossibilityChanged);
        player.OnInteractionPossibilityChanged.AddListener(OnInteractionPossibilityChanged);

        player.OnPuzzlePickedUp.AddListener(ShowHoloPreview);
        player.OnPuzzlePutDown.AddListener(HideHoloPreview);
        player.OnGravityInteraction.AddListener(ToggleArrows);
        holoPreview.SetActive(false);
        foreach (GameObject obj in arrowsToggleObjects)
        {
            obj.SetActive(visibleAllArrows);
        }
    }

    private void ToggleArrows()
    {
        visibleAllArrows = !visibleAllArrows;
        foreach (GameObject obj in arrowsToggleObjects)
        {
            obj.SetActive(visibleAllArrows);
        }
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
        switch (moveDirection)
        {
            case UiMoveDirection.N:
                moveDirection = UiMoveDirection.NW;
                break;
            case UiMoveDirection.S:
                moveDirection = UiMoveDirection.SW;
                break;
            case UiMoveDirection.E:
            case UiMoveDirection.W:
            case UiMoveDirection.NE:
            case UiMoveDirection.NW:
            case UiMoveDirection.SE:
            case UiMoveDirection.SW:
                break;
            case UiMoveDirection.None:
                moveDirection = UiMoveDirection.W;
                break;
        }
        player.OnMoveFromUI(MoveDirectionToVector());
    }

    private Vector2 MoveDirectionToVector()
    {
        switch (moveDirection)
        {
            case UiMoveDirection.N:
                return new Vector2(0, 1);
            case UiMoveDirection.S:
                return new Vector2(0, -1);
            case UiMoveDirection.E:
                return new Vector2(1, 0);
            case UiMoveDirection.W:
                return new Vector2(-1, 0);
            case UiMoveDirection.NE:
                return new Vector2(1, 1);
            case UiMoveDirection.NW:
                return new Vector2(-1, 1);
            case UiMoveDirection.SE:
                return new Vector2(1, -1);
            case UiMoveDirection.SW:
                return new Vector2(-1, -1);
            case UiMoveDirection.None:
            default:
                return Vector2.zero;
        }
    }

    public void OnMoveLeftReleased()
    {
        switch (moveDirection)
        {
            case UiMoveDirection.SW:
                moveDirection = UiMoveDirection.S;
                break;
            case UiMoveDirection.W:
                moveDirection = UiMoveDirection.None;
                break;
            case UiMoveDirection.NW:
                moveDirection = UiMoveDirection.N;
                break;
            case UiMoveDirection.N:
            case UiMoveDirection.S:
            case UiMoveDirection.E:
            case UiMoveDirection.NE:
            case UiMoveDirection.SE:
            case UiMoveDirection.None:
                break;
        }
        player.OnMoveFromUI(MoveDirectionToVector());
    }

    public void OnMoveRightPressed()
    {
        switch (moveDirection)
        {
            case UiMoveDirection.N:
                moveDirection = UiMoveDirection.NE;
                break;
            case UiMoveDirection.S:
                moveDirection = UiMoveDirection.SE;
                break;
            case UiMoveDirection.E:
            case UiMoveDirection.W:
            case UiMoveDirection.NE:
            case UiMoveDirection.NW:
            case UiMoveDirection.SE:
            case UiMoveDirection.SW:
                break;
            case UiMoveDirection.None:
                moveDirection = UiMoveDirection.E;
                break;
        }
        player.OnMoveFromUI(MoveDirectionToVector());
    }

    public void OnMoveRightReleased()
    {
        switch (moveDirection)
        {
            case UiMoveDirection.SE:
                moveDirection = UiMoveDirection.S;
                break;
            case UiMoveDirection.E:
                moveDirection = UiMoveDirection.None;
                break;
            case UiMoveDirection.NE:
                moveDirection = UiMoveDirection.N;
                break;
            case UiMoveDirection.N:
            case UiMoveDirection.S:
            case UiMoveDirection.W:
            case UiMoveDirection.NW:
            case UiMoveDirection.SW:
            case UiMoveDirection.None:
                break;
        }
        player.OnMoveFromUI(MoveDirectionToVector());
    }

public void OnMoveUpPressed()
{
    switch (moveDirection)
    {
        case UiMoveDirection.E:
            moveDirection = UiMoveDirection.NE;
            break;
        case UiMoveDirection.W:
            moveDirection = UiMoveDirection.NW;
            break;
        case UiMoveDirection.N:
        case UiMoveDirection.S:
        case UiMoveDirection.NE:
        case UiMoveDirection.NW:
        case UiMoveDirection.SE:
        case UiMoveDirection.SW:
            break;
        case UiMoveDirection.None:
            moveDirection = UiMoveDirection.N;
            break;
    }
    player.OnMoveFromUI(MoveDirectionToVector());
}

public void OnMoveUpReleased()
{
    switch (moveDirection)
    {
        case UiMoveDirection.NE:
            moveDirection = UiMoveDirection.E;
            break;
        case UiMoveDirection.N:
            moveDirection = UiMoveDirection.None;
            break;
        case UiMoveDirection.NW:
            moveDirection = UiMoveDirection.W;
            break;
        case UiMoveDirection.S:
        case UiMoveDirection.E:
        case UiMoveDirection.W:
        case UiMoveDirection.SE:
        case UiMoveDirection.SW:
        case UiMoveDirection.None:
            break;
    }
    player.OnMoveFromUI(MoveDirectionToVector());
}

public void OnMoveDownPressed()
{
    switch (moveDirection)
    {
        case UiMoveDirection.E:
            moveDirection = UiMoveDirection.SE;
            break;
        case UiMoveDirection.W:
            moveDirection = UiMoveDirection.SW;
            break;
        case UiMoveDirection.N:
        case UiMoveDirection.S:
        case UiMoveDirection.NE:
        case UiMoveDirection.NW:
        case UiMoveDirection.SE:
        case UiMoveDirection.SW:
            break;
        case UiMoveDirection.None:
            moveDirection = UiMoveDirection.S;
            break;
    }
    player.OnMoveFromUI(MoveDirectionToVector());
    }

    public void OnMoveDownReleased()
    {
        switch (moveDirection)
        {
            case UiMoveDirection.SE:
                moveDirection = UiMoveDirection.E;
                break;
            case UiMoveDirection.S:
                moveDirection = UiMoveDirection.None;
                break;
            case UiMoveDirection.SW:
                moveDirection = UiMoveDirection.W;
                break;
            case UiMoveDirection.N:
            case UiMoveDirection.E:
            case UiMoveDirection.W:
            case UiMoveDirection.NE:
            case UiMoveDirection.NW:
            case UiMoveDirection.None:
                break;
        }
        player.OnMoveFromUI(MoveDirectionToVector());
    }
}