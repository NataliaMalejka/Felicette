using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactive : MonoBehaviour
{
    protected bool playerCollision = false;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.State != GameState.GameActive || !context.performed)
            return;
        ActOnInteraction(context);
    }

    protected abstract void ActOnInteraction(InputAction.CallbackContext context);
    protected abstract void ActOnCollision();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.CompareTag("Player"))
        {
            playerCollision = true;
            ActOnCollision();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.CompareTag("Player"))
        {
            playerCollision = false;
        }
    }
}
