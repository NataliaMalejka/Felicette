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
            if (connectedDoor == null)
            {
                Debug.LogWarning("Connected door is not set for " + gameObject.name);
                return;
            }
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.Teleportation);
            teleportEffect?.Play();
            player.PlayTpAnimation();
            player.TogglePlayerInput();
            player.OnTp_1AnimEnd.AddListener(OnTp_1AnimEnd);
        }
    }

    private void OnTp_1AnimEnd()
    {
        player.transform.position = connectedDoor.transform.position;
        player.TogglePlayerInput();
        player.OnTp_1AnimEnd.RemoveListener(OnTp_1AnimEnd);
    }

    protected override void ActOnCollision()
    {
    }
}
