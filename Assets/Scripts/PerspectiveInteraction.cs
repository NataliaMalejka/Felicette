using UnityEngine.Events;
public class PerspectiveInteraction : Interactive
{
    public UnityEvent OnPerspectiveInteraction;
    protected override void ActOnInteraction()
    {
        if (playerCollision)
        {
            OnPerspectiveInteraction?.Invoke();
        }
    }

    protected override void ActOnCollision()
    {

    }
}
