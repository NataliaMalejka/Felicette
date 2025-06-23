using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleDrag : MonoBehaviour
{
    private GameObject selectedPiece;
    // private GameObject pieceImage;
    private EquipmentManager equipment;

    // private Vector3 offset;
    private static float zPosition = 0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Puzzle puzzle;
    [SerializeField] private Grid grid;
    [SerializeField] private GridOrientation gridOrientation;
    private GameObject player;

    private static readonly Plane followPlaneA = new Plane(Vector3.forward, new Vector3(0f, 0f, zPosition));
    private Plane followPlaneB;

    private Vector2 lastPointerPos = Vector2.zero;
    private int startHorizontal;
    private int startVertical;

    private bool isDragging = false;
    private bool isHolding = false;
    private bool isInEquipment = false;
    private static readonly int defaultLayerMask = 1 << 0;

    private void Start()
    {
        // grid = transform.root.GetComponentInChildren<Grid>();
        if (grid == null)
        {
            Debug.LogError($"Grid component not attached to {gridOrientation}");
            return;
        }

        player = GameObject.FindWithTag("Player");
        equipment = GameManager.Instance.GetUiControllerHandle().GetEquipmentManager();
        player.GetComponent<Player>().OnPlayerDrag.AddListener(OnDrag);
        followPlaneB = new Plane(Vector3.up, new Vector3(0f, this.gameObject.transform.position.y, 0f));
        GameManager.Instance.OnGameLost.AddListener(AttemptToPutPiece);
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
        if (GameManager.Instance.isPerspectiveOnWall && gridOrientation == GridOrientation.Floor
        || !GameManager.Instance.isPerspectiveOnWall && gridOrientation == GridOrientation.Wall)
        {
            return;
        }
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
        if (Physics.Raycast(ray, out hit, 100, defaultLayerMask) && hit.collider.CompareTag("Piece"))
        {
            selectedPiece = hit.collider.gameObject;
            // offset = selectedPiece.transform.position - hit.point;

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
                //startHorizontal = Mathf.RoundToInt(startPosition.x / grid.cellSize);
                //startVertical = Mathf.RoundToInt(startPosition.y / grid.cellSize);

                Vector2Int pos = grid.WorldToGridCoords(startPosition, gridOrientation);
                startHorizontal = pos.x;
                startVertical = pos.y;

                //Get the puzzle and remove it from the grid for now
                puzzle = grid.GetPuzzleFromGrid(startHorizontal, startVertical);

                grid.SetPuzzle(null, startHorizontal, startVertical);

                puzzle.isPlaced = false;
            }

            Collider playerCollider = player.GetComponent<Collider>();
            Collider puzzleCollider = selectedPiece.GetComponent<Collider>();
            if (puzzle.portable && !playerCollider.bounds.Intersects(puzzleCollider.bounds))
            // //zmienic na y/z
            // if ((puzzle.portable && (int)player.transform.position.x != (int)puzzle.transform.position.x)
            //     || (puzzle.portable && gridOrientation == GridOrientation.Wall && (int)player.transform.position.y != (int)puzzle.transform.position.y)
            //     || (puzzle.portable && gridOrientation == GridOrientation.Floor && (int)player.transform.position.z != (int)puzzle.transform.position.z))
            {
                isHolding = true;
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TakePuzzle);
                puzzle.ActOnPuzzlePicked();
                player.GetComponent<Player>().TriggerOnPuzzlePickedUp(
                    selectedPiece.GetComponent<Puzzle>().ShapeImage,
                    selectedPiece.GetComponent<Puzzle>().ObjectImage,
                    selectedPiece.transform.rotation.eulerAngles.z);
                // //Select Piece Image
                // pieceImage = selectedPiece.transform.GetChild(0).gameObject;
                // //Higher layer
                // SetOrderInLayer(selectedPiece, 1);
                // SetOrderInLayer(pieceImage, 1);
            }
            else
            {
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.TakePuzzleFail);
                puzzle.ActOnPuzzleFailedDrag();
                selectedPiece.transform.position = startPosition;
                puzzle.isPlaced = true;
                if(!isInEquipment)
                    grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startHorizontal, startVertical);
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
            //int endX = Mathf.RoundToInt(endPosition.x / grid.cellSize);
            //int endY = Mathf.RoundToInt(endPosition.y / grid.cellSize);

            Vector2Int endPos = grid.WorldToGridCoords(endPosition, gridOrientation);
            int endHorizontal = endPos.x;
            int endVertical = endPos.y;

            if (!CanBePlaced(endHorizontal, endVertical))
            {
                selectedPiece.transform.position = startPosition;
                if (isInEquipment)
                    puzzle.Shrink();
                else
                    grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startHorizontal, startVertical);
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzleFail);
                puzzle.ActOnPuzzleFailedDrag();
            }
            //Success
            else
            {
                if (isInEquipment)
                {
                    isInEquipment = false;
                    equipment.SetTargetSlotAsFree();
                }
                grid.SetPuzzle(puzzle, endHorizontal, endVertical);
                selectedPiece.transform.position = grid.GridToWorldCoords(endHorizontal, endVertical, gridOrientation);
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
        player.GetComponent<Player>().TriggerOnPuzzlePutDown();
    }

    private bool CanBePlaced(int endHorizontal, int endVertical)
    {           
            //Check if already occupied, Check if the puzzle doesn't fit, Check if out of bounds
            return grid.GetPuzzleFromGrid(endHorizontal, endVertical) == null
                && selectedPiece.GetComponent<Puzzle>().FitsAt(endHorizontal, endVertical)
                && endHorizontal >= 0 && endHorizontal < grid.GridSizeHorizontal
                && endVertical >= 0 && endVertical < grid.GridSizeVertical;
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

        // Plane followPlane = gridOrientation == GridOrientation.Wall ? followPlaneA : followPlaneB;
        Ray ray = Camera.main.ScreenPointToRay(gridOrientation == GridOrientation.Wall ? new Vector3(pointerPos.x, pointerPos.y, zPosition) : pointerPos);

        if (followPlaneA.Raycast(ray, out float distance))
        {
            Vector3 mousePosition = ray.GetPoint(distance);
            Vector3 pos = mousePosition;

            // if (gridOrientation == GridOrientation.Floor)
            // {
            //     pos.y = 1.5f;
            // }

            // Z offset added so that the piece appears above the floor or wall, other pieces and visual effects
            // Z offset has to move piece closer to the camera
            float zOffset = Camera.main.transform.position.z > 0 ? 0.5f : -0.5f;
            // if (!GameManager.Instance.isPerspectiveOnWall)
            // {
            //     Debug.LogWarning("[!] PuzzleDrag: Camera perspective is on floor, CHECK CUSTOM zOffset.");
            //     zOffset = offset.z;
            // }
            selectedPiece.transform.position = new Vector3(pos.x, pos.y, pos.z + zOffset);
            // selectedPiece.transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, mousePosition.z + zOffset);
            int intX = Mathf.RoundToInt((selectedPiece.transform.position.x - grid.bottomLeftCornerPosition.x )/ grid.cellSize);
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