using UnityEngine;
using UnityEngine.VFX;
public class Door : Interactive //Teleport?
{
    [SerializeField] private GameObject connectedDoor;
    [SerializeField] private VisualEffect teleportEffect;
    [SerializeField] public bool isOnWall = true;
    private PerspectiveManager perspectiveManager;

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
        perspectiveManager = FindObjectOfType<PerspectiveManager>();
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
            player.PlayTpAnimation(true);
            player.TogglePlayerInput();
            player.OnTp_1AnimEnd.AddListener(OnTp_1AnimEnd);

            if (connectedDoor.GetComponent<Door>().isOnWall != isOnWall)
            {
                perspectiveManager.SwitchPerspective();
            }
        }
    }

    private void OnTp_1AnimEnd()
    {
        player.transform.position = connectedDoor.transform.position;
        if(connectedDoor.GetComponent<Door>().isOnWall)
            player.WallYPosition = player.transform.position.y;
        player.TogglePlayerInput();
        player.ForceEnebleInput();
        player.PlayTpAnimation(false);
        player.OnTp_1AnimEnd.RemoveListener(OnTp_1AnimEnd);
        if (connectedDoor.GetComponent<Door>().isOnWall != isOnWall)
            player.LogicToGravityChange();
    }

    protected override void ActOnCollision()
    {
    }
}
