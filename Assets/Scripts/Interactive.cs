using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    protected Player player;
    protected bool playerCollision = false;
    
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.OnPlayerInteraction.AddListener(OnInteract);
    }

    void OnEnable() {
        if (player == null)
            return;
        player.OnPlayerInteraction.RemoveListener(OnInteract);
        player.OnPlayerInteraction.AddListener(OnInteract);
    }

    void OnDisable() {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.CompareTag("Player"))
        {
            playerCollision = true;
            player.SetInteractionPossibility(true);
            ActOnCollision();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.CompareTag("Player"))
        {
            playerCollision = false;
            player.SetInteractionPossibility(false);
        }
    }
}
