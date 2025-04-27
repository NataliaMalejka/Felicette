using UnityEngine;

public class UiMobileControls : MonoBehaviour
{
    [SerializeField] MobileButton moveLeftButton;
    [SerializeField] MobileButton moveRightButton;
    [SerializeField] MobileButton interactButton;
    [SerializeField] MobileButton pauseButton;
    private GameObject player;
    private bool moveLeftClicked = false;
    private bool moveRightClicked = false;
    void Start()
    {
        moveLeftButton.OnButtonPressed.AddListener(OnMoveLeftPressed);
        moveLeftButton.OnButtonReleased.AddListener(OnMoveLeftReleased);
        moveRightButton.OnButtonPressed.AddListener(OnMoveRightPressed);
        moveRightButton.OnButtonReleased.AddListener(OnMoveRightReleased);
        interactButton.OnButtonReleased.AddListener(onButtonInteract);
        // pauseButton.OnButtonPressed.AddListener(GameManager.Instance.PauseGame);
        player = GameObject.FindWithTag("Player");
    }

    public void OnMoveLeftPressed()
    {
        if (moveRightClicked)
        {
            return;
        }
        moveLeftClicked = true;
        player.GetComponent<Player>().OnMoveFromUI(moveLeftClicked, true);
    }
    public void OnMoveLeftReleased()
    {
        if (moveRightClicked)
        {
            return;
        }
        moveLeftClicked = false;
        player.GetComponent<Player>().OnMoveFromUI(moveLeftClicked, true);
    }
    public void OnMoveRightPressed()
    {
        // TO DO - Error - when moving max to right, app still registers clicks but player does not move
        // Maybe see Player.cs - rightBorder
        if (moveLeftClicked)
        {
            return;
        }
        moveRightClicked = true;
        player.GetComponent<Player>().OnMoveFromUI(moveRightClicked, false);
    }
    public void OnMoveRightReleased()
    {
        if (moveLeftClicked)
        {
            return;
        }
        moveRightClicked = false;
        player.GetComponent<Player>().OnMoveFromUI(moveRightClicked, false);
    }
    public void onButtonInteract()
    {
        player.GetComponent<Player>().OnInteractFromUI();
    }
}