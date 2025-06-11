using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleDrag : MonoBehaviour
{
    private GameObject selectedPiece;
    // private GameObject pieceImage;
    private EquipmentManager equipment;

    private Vector3 offset;
    private static float zPosition = 0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Puzzle puzzle;
    private Grid grid;
    private GameObject player;

    private int startX;
    private int startY;
    private static readonly Plane followPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, zPosition));
    private Vector2 lastPointerPos = Vector2.zero;

    private bool isDragging = false;
    private bool isHolding = false;
    private bool isInEquipment = false;

    private void Start()
    {
        grid = transform.root.GetComponentInChildren<Grid>();
        // zPosition = grid.GetDefaultZPosition();
        player = GameObject.FindWithTag("Player");
        equipment = GameManager.Instance.GetUiControllerHandle().GetEquipmentManager();
        player.GetComponent<Player>().OnPlayerDrag.AddListener(OnDrag);
    }
    
    void OnEnable() {
        if (player == null)
            return;
        player.GetComponent<Player>().OnPlayerDrag.RemoveListener(OnDrag);
        player.GetComponent<Player>().OnPlayerDrag.AddListener(OnDrag);
    }

    void OnDisable() {
        if (player == null)
            return;
        player.GetComponent<Player>().OnPlayerDrag.RemoveListener(OnDrag);
    }

    private void OnDrag(bool dragActive)
    {
        isDragging = dragActive;
        if (isDragging)
        {
            AttemptToPickUpPiece();
        }
        else
        {
            AttemptToPutPiece();
        }
    }

    private void Update()
    {
        if (isDragging)
        {   
            HandlePieceDragging();
        }
    }

    private void AttemptToPickUpPiece()
    {
        if (isHolding)
        {
            return;
        }

        Vector2 pointerPos = GetPressedPosition();
        if (pointerPos == Vector2.zero)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(pointerPos.x, pointerPos.y, zPosition));
        // TO DO (? rather not) 2D -> //Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        //Collision Check
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Piece"))
        {
            selectedPiece = hit.collider.gameObject;
            offset = selectedPiece.transform.position - hit.point;

            startPosition = selectedPiece.transform.position;

            // Check if the piece is in equipment and not in the grid
            if (equipment.CheckIfPointHoversOverSlot(selectedPiece.transform.position, checkForEmpty: false))
            {
                //Puzzle pick up from equipment
                isInEquipment = true;
                puzzle = selectedPiece.GetComponent<Puzzle>();
                puzzle.isPlaced = false;
                puzzle.ExpandToDefaultSize();
            }
            else
            {
                //Puzzle pick up from grid
                isInEquipment = false;
                //Grid Position
                startX = Mathf.RoundToInt(startPosition.x / grid.cellSize);
                startY = Mathf.RoundToInt(startPosition.y / grid.cellSize);

                //Get the puzzle and remove it from the grid for now
                puzzle = grid.GetPuzzleFromGrid(startX, startY);
                grid.SetPuzzle(null, startX, startY);
                puzzle.isPlaced = false;
            }

            if ((puzzle.portable && (int)player.transform.position.x != (int)puzzle.transform.position.x)
                || (puzzle.portable && (int)player.transform.position.y != (int)puzzle.transform.position.y))
            {
                isHolding = true;
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TakePuzzle);
                puzzle.ActOnPuzzlePicked();

                // //Select Piece Image
                // pieceImage = selectedPiece.transform.GetChild(0).gameObject;
                // //Higher layer
                // SetOrderInLayer(selectedPiece, 1);
                // SetOrderInLayer(pieceImage, 1);
            }
            else
            {
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TakePuzzleFail);
                selectedPiece.transform.position = startPosition;
                puzzle.isPlaced = true;
                if (!isInEquipment)
                    grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startX, startY);
                selectedPiece = null;
            }
        }   
    }

    private void AttemptToPutPiece()
    {
        if (selectedPiece == null)
        {
            return;
        }

        endPosition = selectedPiece.transform.position;

        //CHECK UI FIRST
        if (equipment.CheckIfPointHoversOverSlot(selectedPiece.transform.position, checkForEmpty: true, previousTargetSlot: isInEquipment))
        {
            selectedPiece.transform.position = equipment.SetTargetSlotAsOccupied(startPosition);
            puzzle.Shrink();
            SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzle);
        }
        else
        {
            //Grid Position
            int endX = Mathf.RoundToInt(endPosition.x / grid.cellSize);
            int endY = Mathf.RoundToInt(endPosition.y / grid.cellSize);

            if (!CanBePlaced(endX, endY))
            {
                selectedPiece.transform.position = startPosition;
                if (isInEquipment)
                    puzzle.Shrink();
                else
                    grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startX, startY);
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzleFail);
            }
            //Success
            else
            {
                if (isInEquipment)
                {
                    isInEquipment = false;
                    equipment.SetTargetSlotAsFree();
                }
                grid.SetPuzzle(puzzle, endX, endY);
                selectedPiece.transform.position = new Vector3(endX * grid.cellSize, endY * grid.cellSize, zPosition);
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzle);
            }
        }
        //Lower layer
        isHolding = false;
        // SetOrderInLayer(selectedPiece, 0);
        // SetOrderInLayer(pieceImage, 0);
        puzzle.isPlaced = true;
        selectedPiece = null;
        // pieceImage = null;    
        puzzle.ActOnPuzzlePlaced();
    }

    private bool CanBePlaced(int intX, int intY)
    {
        //Check if already occupied, Check if the puzzle doesn't fit, Check if out of bounds
        return grid.GetPuzzleFromGrid(intX, intY) == null
            && selectedPiece.GetComponent<Puzzle>().FitsAt(intX, intY)
            && intX >= 0 && intX < grid.GridSizeX
            && intY >= 0 && intY < grid.GridSizeY;
    }

    private void HandlePieceDragging()
    {
        //Move Piece
        if (selectedPiece == null)
        {
            return;
        }
        Vector2 pointerPos = GetPressedPosition();
        if (pointerPos == Vector2.zero || pointerPos == lastPointerPos)
        {
            return;
        }
        lastPointerPos = pointerPos;

        Ray ray = Camera.main.ScreenPointToRay(pointerPos);
        if (followPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPos = ray.GetPoint(distance);
            float zOffset = Camera.main.transform.position.z > 0 ? 0.5f : -0.5f;
            if (!GameManager.Instance.isPerspectiveOnWall)
            {
                Debug.LogWarning("[!] PuzzleDrag: Camera perspective is on floor, CHECK CUSTOM zOffset.");
                zOffset = offset.z;
            }
            selectedPiece.transform.position = new Vector3(targetPos.x + offset.x, targetPos.y + offset.y, targetPos.z + zOffset);
            int intX = Mathf.RoundToInt(selectedPiece.transform.position.x / grid.cellSize);
            int intY = Mathf.RoundToInt(selectedPiece.transform.position.y / grid.cellSize);
            puzzle.ChangeShaderColor(CanBePlaced(intX, intY));
        }
    }
    
    private Vector2 GetPressedPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }
    //Obsolete method, kept for reference
    private void SetOrderInLayer(GameObject piece, int order)
    {
        var spriteRenderer = piece.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}