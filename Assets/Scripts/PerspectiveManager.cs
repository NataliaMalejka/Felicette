using UnityEngine;

public class PerspectiveManager : MonoBehaviour
{
    // [SerializeField] Animator perspAnim;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject gridWall;
    [SerializeField] GameObject gridFloor;
    [SerializeField] PerspectiveInteraction perspectiveInteractionWall;
    [SerializeField] PerspectiveInteraction perspectiveInteractionFloor;

    private GameObject player;

    //Principle -> Only one grid should be active at a time, with an exception of the 
    // perspective animation when other logic is blocked and interaction is not possible.
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // perspAnim.GetComponent<AnimatorEvent>().OnWallAnimEnd.AddListener(OnWallAnimEnd);
        // perspAnim.GetComponent<AnimatorEvent>().OnFloorAnimEnd.AddListener(OnFloorAnimEnd);

        //Listen to every perspective interaction event from PerspectiveInteraction objects
        // foreach (PerspectiveInteraction perspectiveInteraction in FindObjectsByType<PerspectiveInteraction>(FindObjectsSortMode.None))
        // {
        //     perspectiveInteraction.OnPerspectiveInteraction.AddListener(ActOnPerspectiveInteraction);
        // }
        perspectiveInteractionWall.OnPerspectiveInteraction.AddListener(ActOnPerspectiveInteraction);
        perspectiveInteractionFloor.OnPerspectiveInteraction.AddListener(ActOnPerspectiveInteraction);

        // if (GameManager.Instance.isPerspectiveOnWall)
        // {
        //     gridFloor.SetActive(false);
        //     gridWall.SetActive(true);
        // }
        // else
        // {
        //     gridWall.SetActive(false);
        //     gridFloor.SetActive(true);
        // }
    }

    // private void OnWallAnimEnd()
    // {
    //     gridWall.SetActive(false);

    //     player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
    //     player.GetComponent<Player>().TogglePlayerInput();
    // }

    // private void OnFloorAnimEnd()
    // {
    //     gridFloor.SetActive(false);

    //     player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);
    //     player.GetComponent<Player>().TogglePlayerInput();
    // }

    private void ActOnPerspectiveInteraction()
    {
        // gridWall.SetActive(true);
        // gridFloor.SetActive(true);
        // player.GetComponent<Player>().TogglePlayerInput();
        if (GameManager.Instance.isPerspectiveOnWall)
        {
            GameManager.Instance.isPerspectiveOnWall = false;
            // perspAnim.SetTrigger("ToFloor");
            // gridWall.SetActive(false);
            // gridFloor.SetActive(true);
            // player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
            player.transform.position = new Vector3(
                player.transform.position.x + gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                gridFloor.GetComponent<Grid>().startingYPosition,
                player.transform.position.z);
            mainCamera.transform.position = new Vector3(
                mainCamera.transform.position.x + gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                mainCamera.transform.position.y,
                mainCamera.transform.position.z);
            player.GetComponent<Player>().CheckIfNothingToStandOn();
        }
        else
        {
            GameManager.Instance.isPerspectiveOnWall = true;
            // perspAnim.SetTrigger("ToWall");
            // gridWall.SetActive(true);
            // gridFloor.SetActive(false);
            // player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
            player.transform.position = new Vector3(
                player.transform.position.x - gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                player.GetComponent<Player>().WallYPosition,
                player.transform.position.z);
            mainCamera.transform.position = new Vector3(
                mainCamera.transform.position.x - gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                mainCamera.transform.position.y,
                mainCamera.transform.position.z);
            player.GetComponent<Player>().CheckIfNothingToStandOn();
        }

        SoundsManager.Instance.PlaySound(SoundsManager.Sounds.ChangePerspective);
    }
}
