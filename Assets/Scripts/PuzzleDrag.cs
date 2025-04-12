using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleDrag : MonoBehaviour
{
    private GameObject selectedPiece;
    private GameObject pieceImage;

    private Vector3 offset;
    private float zPosition = 0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Puzzle puzzle;
    private Grid grid;
    private GameObject player;

    private int startX;
    private int startY;

    private bool isDragging = false;
    private bool isHolding = false;
    private InputAction.CallbackContext context;

    private void Start()
    {
        Debug.LogWarning("PuzzleDrag: Start() - Only one Grid object should be present in the scene."); 
        grid = GameObject.FindWithTag("Grid").GetComponent<Grid>();
        player = GameObject.FindWithTag("Player");
    }

   private void OnDrag(InputAction.CallbackContext ctx)
   {
        isDragging = true;
        context = ctx;
   }

    private void Update()
    {
        if (context.performed && isDragging)
        {
            isDragging = false;
        }
        if (isDragging)
        {   
            HandleDrag();
        }
    }

    //TO DO - Needs rework
    private void HandleDrag()
    {
        if (isHolding)
        {
            Vector2 pointerPos = Mouse.current != null
                ? Mouse.current.position.ReadValue()
                : Touchscreen.current.primaryTouch.position.ReadValue();
            //Mouse Position To World Position
            Debug.Log("Mouse Position: " + pointerPos + " Only accounts X and Y, Z is set to " + zPosition);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(pointerPos.x, pointerPos.y, zPosition));
            RaycastHit hit;

            //Collision Check
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Piece"))
            {
                selectedPiece = hit.collider.gameObject;
                offset = selectedPiece.transform.position - hit.point;

                startPosition = selectedPiece.transform.position;

                //Grid Position
                startX = Mathf.RoundToInt(startPosition.x / grid.cellSize);
                startY = Mathf.RoundToInt(startPosition.y / grid.cellSize);

                //Get the puzzle and remove it from the grid for now
                puzzle = grid.GetPuzzleFromGrid(startX, startY);
                grid.SetPuzzle(null, startX, startY);
                puzzle.isPlaced = false;

                if ((puzzle.portable && (int)player.transform.position.x != (int)puzzle.transform.position.x)
                    || (puzzle.portable && (int)player.transform.position.y != (int)puzzle.transform.position.y))
                {
                    isHolding = true;
                    SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TakePuzzle);
                    //Select Piece Image
                    pieceImage = selectedPiece.transform.GetChild(0).gameObject;

                    //Higher layer
                    SetOrderInLayer(selectedPiece, 1);
                    SetOrderInLayer(pieceImage, 1);
                }
                else
                {
                    SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TakePuzzleFail);
                    selectedPiece.transform.position = startPosition;
                    puzzle.isPlaced = true;
                    grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startX, startY);
                    selectedPiece = null;
                }
            }
        }
        //Move Piece
        if (selectedPiece != null )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, zPosition)); 
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                Vector3 mousePosition = ray.GetPoint(distance);
                selectedPiece.transform.position = mousePosition + offset;
            }
        }

        //Put Piece
        if (Input.GetMouseButtonUp(0) && selectedPiece != null )
        {
            endPosition = selectedPiece.transform.position;

            //Grid Position
            int endX = Mathf.RoundToInt(endPosition.x / grid.cellSize);
            int endY = Mathf.RoundToInt(endPosition.y / grid.cellSize);

            //Check if already occupied
            if (grid.GetPuzzleFromGrid(endX, endY) != null)
            {
                selectedPiece.transform.position = startPosition;
                grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startX, startY);
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzleFail);
            }
            //Check if the puzzle doesn't fit
            else if(!selectedPiece.GetComponent<Puzzle>().FitsAt(endX, endY))
            {
                selectedPiece.transform.position = startPosition;
                grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startX, startY);
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzleFail);
            }
            //Success
            else
            {
                grid.SetPuzzle(puzzle, endX, endY);
                selectedPiece.transform.position = new Vector3(endX * grid.cellSize, endY * grid.cellSize, zPosition);
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzle); 
            }

            //Lower layer
            isHolding = false;
            SetOrderInLayer(selectedPiece, 0);
            SetOrderInLayer(pieceImage, 0);
            puzzle.isPlaced = true;
            selectedPiece = null;
            pieceImage = null;
        }
    }
    
    private void SetOrderInLayer(GameObject piece, int order)
    {
        var spriteRenderer = piece.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}