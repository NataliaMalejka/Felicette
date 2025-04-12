using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    protected bool playerCollision = false;

    private void OnEnable()
    {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.OnPlayerInteraction.AddListener(OnInteract);
    }

    public void OnInteract()
    {
        if (GameManager.Instance.State != GameState.GameActive)
            return;
        ActOnInteraction();
    }

    protected abstract void ActOnInteraction();
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
