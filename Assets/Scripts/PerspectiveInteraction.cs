using UnityEngine.Events;
using UnityEngine;
public class PerspectiveInteraction : Interactive
{
    public UnityEvent OnPerspectiveInteraction;

    protected override void Start()
    {
        base.Start();
        player.OnPlayerInteraction.RemoveListener(OnInteract);
        player.OnGravityInteraction.AddListener(OnInteract);
        interactiveType = InteractiveType.Gravity;
    }

    // protected override void OnEnable() {
    //     if (player == null)
    //         return;
    //     player.OnGravityInteraction.RemoveListener(OnInteract);
    //     player.OnGravityInteraction.AddListener(OnInteract);
    // }

    // protected override void OnDisable() {
    //     if (player == null)
    //         return;
    //     player.OnGravityInteraction.RemoveListener(OnInteract);
    // }

    protected override void ActOnInteraction()
    {
        if (playerCollision)
        {
            player.LogicToGravityChange();
            OnPerspectiveInteraction?.Invoke();
        }
    }

    protected override void ActOnCollision()
    {

    }
}
