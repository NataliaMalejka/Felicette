using UnityEngine;
public class Door : Interactive //Teleport?
{
    [SerializeField] private GameObject connectedDoor;

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
