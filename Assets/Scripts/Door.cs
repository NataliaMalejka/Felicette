using UnityEngine;
public class Door : Interactive //Teleport?
{
    [SerializeField] private GameObject connectedDoor;

    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void ActOnInteraction()
    {
        if(playerCollision)
        {
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.OpenDoor);
            player.transform.position = connectedDoor.transform.position;
        }
    }

    protected override void ActOnCollision()
    {
    }
}
