using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using NUnit.Framework;

public class Player : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private int leftBorder; //TO DO
    [SerializeField] private int rightBorder; //TO DO
    [SerializeField] private Vector3 direction;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 oldPosition;
    private Vector2 moveInput;

    public UnityEvent OnPlayerInteraction;
    public UnityEvent<bool> OnPlayerDrag;
    private bool isDragging = false;

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnInteract(InputValue value)
    {
        if (GameManager.Instance.State != GameState.GameActive || !value.isPressed)
            return;
        OnPlayerInteraction?.Invoke();
    }

    //Fires event 2 times, once when pressed and once when released
    private void OnDrag(InputValue value)
    {
        if (GameManager.Instance.State != GameState.GameActive)
            return;
        isDragging = false;
        if(value.isPressed)
            isDragging = true;
        OnPlayerDrag?.Invoke(isDragging);
    }

    void FixedUpdate()
    {
        if(moveInput != Vector2.zero)
        {
            float horizontalInput = moveInput.x;
            if (horizontalInput != 0 && GameManager.Instance.State == GameState.GameActive)
            {
                Ray ray = new Ray(transform.position, new Vector3(direction.x * horizontalInput, direction.y, direction.z));
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (raycastHit.transform.gameObject.CompareTag("Piece") 
                        || raycastHit.transform.gameObject.CompareTag("Door") 
                        || raycastHit.transform.gameObject.CompareTag("Rocket")) //TO DO Replace Tag with "Interactive" (?)
                    {
                        oldPosition = transform.position;
                        transform.Translate(horizontalInput * velocity * Time.deltaTime, 0, 0);
                        
                        if (transform.position.x < leftBorder)
                            transform.position = new Vector3(leftBorder, transform.position.y, transform.position.z);

                        if (transform.position.x > rightBorder)
                            transform.position = new Vector3(rightBorder, transform.position.y, transform.position.z);

                        transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftBorder, rightBorder), transform.position.y, transform.position.z);
                        spriteRenderer.flipX = horizontalInput < 0;

                        if (!raycastHit.transform.gameObject.GetComponent<Puzzle>().isPlaced)
                            transform.position = oldPosition;
                    }
                }
            }
        }
    }
}