using UnityEngine;

public enum InteractiveType
{
    Gravity,
    Other
}

public abstract class Interactive : MonoBehaviour
{
    protected Player player;
    protected bool playerCollision = false;
    protected InteractiveType interactiveType = InteractiveType.Other;

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.OnPlayerInteraction.AddListener(OnInteract);
    }

    protected virtual void OnEnable()
    {
        if (player == null)
            return;
        player.OnPlayerInteraction.RemoveListener(OnInteract);
        player.OnPlayerInteraction.AddListener(OnInteract);
    }

    protected virtual void OnDisable()
    {
        if (player == null)
            return;
        player.OnPlayerInteraction.RemoveListener(OnInteract);
    }

    public void OnInteract()
    {
        if (GameManager.Instance.State != GameState.GameActive)
            return;
        ActOnInteraction();
    }

    protected abstract void ActOnInteraction();
    protected abstract void ActOnCollision();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.CompareTag("Player"))
        {
            playerCollision = true;
            player.SetInteractionPossibility(true, interactiveType);
            ActOnCollision();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.CompareTag("Player"))
        {
            playerCollision = false;
            player.SetInteractionPossibility(false, interactiveType);
        }
    }
}
