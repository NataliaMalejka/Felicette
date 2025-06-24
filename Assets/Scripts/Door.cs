using UnityEngine;
using UnityEngine.VFX;
public class Door : Interactive //Teleport?
{
    [SerializeField] private GameObject connectedDoor;
    [SerializeField] private VisualEffect teleportEffect;

    protected override void Start()
    {
        base.Start();
        transform.rotation = Quaternion.identity;
        if (spriteRenderer != null)
            spriteRenderer.gameObject.transform.localPosition = interactiveLocation == GridOrientation.Wall ?
                new Vector3(spriteRenderer.gameObject.transform.localPosition.x,
                            spriteRenderer.gameObject.transform.localPosition.y - 0.75f,
                            spriteRenderer.gameObject.transform.localPosition.z)
                : spriteRenderer.gameObject.transform.localPosition;
    }

    protected override void ActOnInteraction()
    {
        if (playerCollision)
        {
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.Teleportation);
            teleportEffect?.Play();
            if (connectedDoor == null)
            {
                Debug.LogWarning("Connected door is not set for " + gameObject.name);
                return;
            }
            player.transform.position = connectedDoor.transform.position;
        }
    }

    protected override void ActOnCollision()
    {
    }
}
