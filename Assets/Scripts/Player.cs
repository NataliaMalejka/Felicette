using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private Vector3 direction;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    public UnityEvent OnPlayerInteraction;
    public UnityEvent<bool> OnPlayerDrag;
    public UnityEvent<Vector3> OnPlayerMoved;
    public UnityEvent<bool, InteractiveType> OnInteractionPossibilityChanged;
    public UnityEvent OnGravityInteraction;

    public UnityEvent<Sprite, Sprite, float> OnPuzzlePickedUp;
    public UnityEvent OnPuzzlePutDown;

    private bool isDragging = false;
    private PlayerInput inputComponent;
    private BoxCollider boxCollider;
    private bool upDownMovementEnabled = false;
    private Vector3 oldPosition;
    private Vector2 moveInput;
    public float WallYPosition = 0;
    private Vector3 realBoxColliderSize
    {
        get
        {
            return new Vector3(boxCollider.size.x * transform.localScale.x,
                               boxCollider.size.y * transform.localScale.y,
                               boxCollider.size.z * transform.localScale.z);
        }
    }

    private void Start()
    {
        inputComponent = GetComponent<PlayerInput>();
        boxCollider = GetComponent<BoxCollider>();
        if (animator == null || spriteRenderer == null)
            Debug.LogError("Missing SerializeField components in Player");
        GameManager.Instance.OnGameLost.AddListener(() =>
        {
            animator.SetBool("IsRun", false);
        });
        WallYPosition = transform.position.y;
    }

    void OnInteract(InputValue value)
    {
        if (!value.isPressed)
            return;
        HandleInteractInput();
    }

    void OnRestart(InputValue value)
    {
        if (!value.isPressed)
            return;
        HandleRestartInput();
    }

    void OnGravity(InputValue value)
    {
        if (!value.isPressed)
            return;
        HandleGravityInput();
    }

    public void SetInteractionPossibility(bool canInteract, InteractiveType interactiveType)
    {
        OnInteractionPossibilityChanged?.Invoke(canInteract, interactiveType);
    }

    public void TriggerOnPuzzlePickedUp(Sprite previewSprite, Sprite previewChildSprite = null, float rotation = 0f)
    {
        OnPuzzlePickedUp?.Invoke(previewSprite, previewChildSprite, rotation);
    }

    public void TriggerOnPuzzlePutDown()
    {
        OnPuzzlePutDown?.Invoke();
    }

    public void TogglePlayerInput()
    {
        if (inputComponent != null)
        {
            inputComponent.enabled = !inputComponent.enabled;
        }
    }

    //Fires event 2 times, once when pressed and once when released
    void OnDrag(InputValue value)
    {
        if (GameManager.Instance.State != GameState.GameActive)
            return;
        isDragging = false;
        if(value.isPressed)
            isDragging = true;
        OnPlayerDrag?.Invoke(isDragging);
    }

    public void OnInteractFromUI()
    {
        HandleInteractInput();
    }

    public void OnRestartFromUI()
    {
        HandleRestartInput();
    }

    public void OnGravityFromUI()
    {
       HandleGravityInput();
    }

    private void HandleRestartInput()
    {
        if (GameManager.Instance.State == GameState.GameActive)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    private void HandleInteractInput()
    {
        if (GameManager.Instance.State != GameState.GameActive || isDragging)
            return;
        OnPlayerInteraction?.Invoke();
    }

    private void HandleGravityInput()
    {
        if (GameManager.Instance.State != GameState.GameActive || isDragging)
            return;
        OnGravityInteraction?.Invoke();
        ToggleUpDownMovement();
        animator.SetBool("IsRun", false);
        animator.SetTrigger("Presp");
        if (!upDownMovementEnabled)
            spriteRenderer.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void ToggleUpDownMovement()
    {
        upDownMovementEnabled = !upDownMovementEnabled;
    }

    private List<Collider> CheckCollidingTiles()
    {
        List<Collider> hits = Physics.OverlapBox(transform.position, realBoxColliderSize, transform.rotation).ToList();

        List<Collider> pieces = new List<Collider>();
        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject && hit.CompareTag("Piece"))
            {
                if (hit.gameObject.GetComponent<Puzzle>() != null &&
                    !hit.gameObject.GetComponent<Puzzle>().isPlaced)
                {
                    continue; // Skip pieces that are currently being dragged
                }
                pieces.Add(hit);
            }
        }
        return pieces;
    }

    bool IsPlayerFullyInsideAnyCell(List<Collider> pieceColliders)
    {
        foreach (Collider cell in pieceColliders)
        {
            Bounds cellBounds = cell.bounds;

            if (cellBounds.Contains(transform.position))
            {
                return true;
            }
        }
        return false;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnMoveFromUI(Vector2 dir)
    {
        moveInput = dir;
    }

    public void CheckIfNothingToStandOn()
    {
        List<Collider> pieceColliders = CheckCollidingTiles();
        if (pieceColliders.Count == 0)
        {
            animator.SetBool("IsRun", false);
            GameManager.Instance.ActOnGameOver();
            return;
        }
        if (!IsPlayerFullyInsideAnyCell(pieceColliders) ||
            (pieceColliders.Count == 2 &&
            pieceColliders[0].transform.position.x != pieceColliders[1].transform.position.x &&
            pieceColliders[0].transform.position.y != pieceColliders[1].transform.position.y))
        {
            animator.SetBool("IsRun", false);
            GameManager.Instance.ActOnGameOver();
            return;
        }
    }

    private void MovePlayer()
    {
        if (moveInput == Vector2.zero)
        {
            moveInput = inputComponent.actions["Move"].ReadValue<Vector2>();
            if (moveInput == Vector2.zero)
            {
                animator.SetBool("IsRun", false);
                return;
            }
        }
        if (moveInput.x == 0 && !upDownMovementEnabled)
        {
            return;
        }

        animator.SetBool("IsRun", true);

        float timeVelocity = velocity * Time.deltaTime;
        oldPosition = transform.position;
        transform.Translate(moveInput.x != 0 ? moveInput.x * timeVelocity : 0,
                            upDownMovementEnabled && moveInput.y != 0 ? moveInput.y * timeVelocity : 0,
                            0);
        List<Collider> pieceColliders = CheckCollidingTiles();
        if (pieceColliders.Count == 0)
        {
            Debug.LogWarning("Player out of bounds.");
            transform.position = oldPosition;
            return;
        }
        // Check bounds and corner cutting
        if (!IsPlayerFullyInsideAnyCell(pieceColliders) ||
            (pieceColliders.Count == 2 &&
            pieceColliders[0].transform.position.x != pieceColliders[1].transform.position.x &&
            pieceColliders[0].transform.position.y != pieceColliders[1].transform.position.y))
        {
            transform.position = oldPosition;
            animator.SetBool("IsRun", false);
            return;
        }

        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
            if (upDownMovementEnabled)
                spriteRenderer.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (moveInput.y != 0)
        {
            spriteRenderer.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90 * (moveInput.y > 0 ? 1 : -1));
            spriteRenderer.flipX = false;
        }
        if (GameManager.Instance.isPerspectiveOnWall)
        {
            WallYPosition = transform.position.y;
        }
        OnPlayerMoved?.Invoke(transform.position);
    }

    public void HideSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.State == GameState.GameActive)
        {
            MovePlayer();
        }
    }
}