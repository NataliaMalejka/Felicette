using Unity.VisualScripting;
using UnityEngine;

public class Perspective : Interactive
{
    [SerializeField] Animator perspAnim;
    [SerializeField] GameObject gridWall;
    [SerializeField] GameObject gridFloor;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //perspAnim.OnWallAnimEnd.AddListener(OnWallAnimEnd);
        //perspAnim.OnFloorAnimEnd.AddListener(OnFloorAnimEnd);

        perspAnim.GetComponent<AnimatorEvent>().OnWallAnimEnd.AddListener(OnWallAnimEnd);
        perspAnim.GetComponent<AnimatorEvent>().OnFloorAnimEnd.AddListener(OnFloorAnimEnd);
    }

    private void OnWallAnimEnd()
    {
        
    }

    private void OnFloorAnimEnd()
    {

    }

    protected override void ActOnInteraction()
    {
        if (playerCollision)
        {
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.ChangePerspective);
            Debug.Log("Camera animation");

            if(GameManager.Instance.isPerspectiveOnWall)
            {
                GameManager.Instance.isPerspectiveOnWall = false;
                player.GetComponent<Player>().TogglePlayerInput();
                perspAnim.SetTrigger("ToFloor");
                //On animation finish:
                gridWall.SetActive(false);
                gridFloor.SetActive(true);
                player.GetComponent<Player>().TogglePlayerInput();
            }
            else
            {
                GameManager.Instance.isPerspectiveOnWall = true;
                player.GetComponent<Player>().TogglePlayerInput();
                perspAnim.SetTrigger("ToWall");
                gridWall.SetActive(true);
                // On animation finish:
                gridFloor.SetActive(false);
                player.GetComponent<Player>().TogglePlayerInput();
            }
        }
    }

    protected override void ActOnCollision()
    {
        
    }
}
