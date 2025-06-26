using UnityEngine;
using System.Collections;

public class PerspectiveManager : MonoBehaviour
{
    [SerializeField] Animator perspAnim;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject gridWall;
    [SerializeField] GameObject gridFloor;
    [SerializeField] PerspectiveInteraction perspectiveInteractionWall;
    [SerializeField] PerspectiveInteraction perspectiveInteractionFloor;
    [SerializeField] bool playPerspectiveAnimations = true;

    private GameObject player;

    //Principle -> Only one grid should be active at a time, with an exception of the 
    // perspective animation when other logic is blocked and interaction is not possible.
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // if (playPerspectiveAnimations)
        // {
        //     perspAnim.GetComponent<AnimatorEvent>().OnWallAnimEnd.AddListener(OnWallAnimEnd);
        //     perspAnim.GetComponent<AnimatorEvent>().OnFloorAnimEnd.AddListener(OnFloorAnimEnd);
        // }

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

    private void OnWallAnimEnd()
    {
        mainCamera.transform.position = new Vector3(
            mainCamera.transform.position.x + gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
            mainCamera.transform.position.y,
            mainCamera.transform.position.z);

        // gridWall.SetActive(false);

        // player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
        player.GetComponent<Player>().TogglePlayerInput();
    }

    private void OnFloorAnimEnd()
    {
        mainCamera.transform.position = new Vector3(
            mainCamera.transform.position.x - gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
            mainCamera.transform.position.y,
            mainCamera.transform.position.z);
        // gridFloor.SetActive(false);

        // player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z);
        player.GetComponent<Player>().TogglePlayerInput();
    }

    private void ActOnPerspectiveInteraction()
    {
        // gridWall.SetActive(true);
        // gridFloor.SetActive(true);
        // player.GetComponent<Player>().TogglePlayerInput();
        player.GetComponent<Player>().TogglePlayerInput();
        Debug.Log("ActPerspective");
        if (GameManager.Instance.isPerspectiveOnWall)
        {
            GameManager.Instance.isPerspectiveOnWall = false;
            if (playPerspectiveAnimations)
                perspAnim.SetTrigger("ToFloor");
            // gridWall.SetActive(false);
            // gridFloor.SetActive(true);
            // player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
            player.transform.position = new Vector3(
                player.transform.position.x + gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                gridFloor.GetComponent<Grid>().startingYPosition,
                player.transform.position.z);
            // mainCamera.transform.position = new Vector3(
            //     mainCamera.transform.position.x + gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
            //     mainCamera.transform.position.y,
            //     mainCamera.transform.position.z);
            player.GetComponent<Player>().CheckIfNothingToStandOn();
            StartCoroutine(ExecuteAfterDelayW());
            if (!playPerspectiveAnimations)
            {
                OnWallAnimEnd();
            }
        }
        else
        {
            GameManager.Instance.isPerspectiveOnWall = true;
            if (playPerspectiveAnimations)
                perspAnim.SetTrigger("ToWall");
            // gridWall.SetActive(true);
            // gridFloor.SetActive(false);
            // player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
            player.transform.position = new Vector3(
                player.transform.position.x - gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                player.GetComponent<Player>().WallYPosition,
                player.transform.position.z);
            // mainCamera.transform.position = new Vector3(
            //     mainCamera.transform.position.x - gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
            //     mainCamera.transform.position.y,
            //     mainCamera.transform.position.z);
            player.GetComponent<Player>().CheckIfNothingToStandOn();
            StartCoroutine(ExecuteAfterDelayF());
            if (!playPerspectiveAnimations)
            {
                OnFloorAnimEnd();
            }
        }

        SoundsManager.Instance.PlaySound(SoundsManager.Sounds.ChangePerspective);
    }

    public void SwitchPerspective() //It will only switch perspective and player inputs - it will not change player's position!!!!!
    {
        player.GetComponent<Player>().TogglePlayerInput();
        if (GameManager.Instance.isPerspectiveOnWall)
        {
            GameManager.Instance.isPerspectiveOnWall = false;

            FindAnyObjectByType<Camera>().transform.position = new Vector3(
                FindAnyObjectByType<Camera>().transform.position.x + gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                FindAnyObjectByType<Camera>().transform.position.y,
                FindAnyObjectByType<Camera>().transform.position.z);

        }
        else
        {
            GameManager.Instance.isPerspectiveOnWall = true;

            FindAnyObjectByType<Camera>().transform.position = new Vector3(
                FindAnyObjectByType<Camera>().transform.position.x - gridFloor.GetComponent<Grid>().bottomLeftCornerPosition.x,
                FindAnyObjectByType<Camera>().transform.position.y,
                FindAnyObjectByType<Camera>().transform.position.z);
        }
    }

    IEnumerator ExecuteAfterDelayW()
    {
        yield return new WaitForSeconds(0.2f);
        OnWallAnimEnd();
    }

    IEnumerator ExecuteAfterDelayF()
    {
        yield return new WaitForSeconds(0.2f);
        OnFloorAnimEnd();
    }
}
