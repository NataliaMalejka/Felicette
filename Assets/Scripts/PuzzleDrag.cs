using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

public class PuzzleDrag : MonoBehaviour
{
    private GameObject selectedPiece;
    private GameObject pieceImage;
    private EquipmentManager equipment;

    private Vector3 offset;
    private float zPosition = 0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Puzzle puzzle;
    private Grid grid;
    [SerializeField] private GridOrientation gridOrientation;
    private GameObject player;

    private int startHorizontal;
    private int startVertical;

    private bool isDragging = false;
    private bool isHolding = false;
    private bool isInEquipment = false;

    private void Start()
    {
        grid = transform.root.GetComponentInChildren<Grid>();
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
                isInEquipment = true;
                puzzle = selectedPiece.GetComponent<Puzzle>();
                puzzle.isPlaced = false;
                puzzle.ExpandToDefaultSize();
            }
            else
            {
                isInEquipment = false;
                //Grid Position
                //startHorizontal = Mathf.RoundToInt(startPosition.x / grid.cellSize);
                //startVertical = Mathf.RoundToInt(startPosition.y / grid.cellSize);

                Vector2Int pos = grid.WorldToGridCoords(startPosition, gridOrientation);
                startHorizontal = pos.x;
                startVertical = pos.y;

                Debug.Log(startHorizontal + ", " + startVertical);

                //Get the puzzle and remove it from the grid for now
                puzzle = grid.GetPuzzleFromGrid(startHorizontal, startVertical );

                grid.SetPuzzle(null, startHorizontal, startVertical);

                puzzle.isPlaced = false;
            }
            //zmienic na y/z
            if ((puzzle.portable && (int)player.transform.position.x != (int)puzzle.transform.position.x)
                || (puzzle.portable && gridOrientation == GridOrientation.Wall && (int)player.transform.position.y != (int)puzzle.transform.position.y)
                || (puzzle.portable && gridOrientation == GridOrientation.Floor && (int)player.transform.position.z != (int)puzzle.transform.position.z))
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
                if(!isInEquipment)
                    grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startHorizontal, startVertical);
                selectedPiece = null;
            }
        }   
    }

    private void AttemptToPutPiece()
    {
        if (selectedPiece == null )
        {
            return;
        }

        Debug.Log("AttemptToPutPiece");

        endPosition = selectedPiece.transform.position;
        Debug.Log(endPosition + " endPosition");


        //CHECK UI FIRST
        if(equipment.CheckIfPointHoversOverSlot(selectedPiece.transform.position, checkForEmpty: true, previousTargetSlot: isInEquipment))
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

            Debug.Log(endHorizontal + " " + endVertical);

            //Check if already occupied, Check if the puzzle doesn't fit, Check if out of bounds
            if (grid.GetPuzzleFromGrid(endHorizontal, endVertical) != null
                || !selectedPiece.GetComponent<Puzzle>().FitsAt(endHorizontal, endVertical)
                || endHorizontal < 0 || endHorizontal >= grid.GridSizeHorizontal
                || endVertical < 0 || endVertical >= grid.GridSizeVertical)
            {
                selectedPiece.transform.position = startPosition;
                if(isInEquipment)
                    puzzle.Shrink();            
                else
                    grid.SetPuzzle(selectedPiece.GetComponent<Puzzle>(), startHorizontal, startVertical);
                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzleFail);
            }
            //Success
            else
            {
                if(isInEquipment)
                {
                    isInEquipment = false;
                    equipment.SetTargetSlotAsFree();
                }
                grid.SetPuzzle(puzzle, endHorizontal, endVertical);
                Debug.Log(endHorizontal + ", " + endVertical);
                //selectedPiece.transform.position = new Vector3(endHorizontal * grid.cellSize, endVertical * grid.cellSize, zPosition);
                selectedPiece.transform.position = grid.GridToWorldCoords(endHorizontal, endVertical, gridOrientation);

                Debug.Log(selectedPiece.transform.position);

                SoundsManager.Instance.PlaySound(SoundsManager.Sounds.PutPuzzle); 
            }
        }
        //Lower layer
        isHolding = false;
        SetOrderInLayer(selectedPiece, 0);
        SetOrderInLayer(pieceImage, 0);
        puzzle.isPlaced = true;
        selectedPiece = null;
        pieceImage = null;    
    }

    private void HandlePieceDragging()
    {
        //Move Piece
        if (selectedPiece != null)
        {
            Vector2 pointerPos = GetPressedPosition();
            if (pointerPos == Vector2.zero)
            {
                return;
            }
            // TO DO (?) - 2D -> //Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Ray ray;
            Plane plane;
            if (gridOrientation == GridOrientation.Wall)
            {
                ray = Camera.main.ScreenPointToRay(new Vector3(pointerPos.x, pointerPos.y, zPosition));
                plane = new Plane(Vector3.forward, new Vector3(0, 0, zPosition));
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(pointerPos);
                plane = new Plane(Vector3.up, new Vector3(0, this.gameObject.transform.position.y, 0));
            }
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                Vector3 mousePosition = ray.GetPoint(distance);

                Vector3 pos = mousePosition /*+ offset*/;

                //if (gridOrientation == GridOrientation.Floor)
                //    pos.y = this.gameObject.transform.position.y;

                if (gridOrientation == GridOrientation.Floor)
                {
                    pos.y = 1.5f;
                }

                selectedPiece.transform.position = pos;

                Debug.Log(pos);

            }
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

    private void SetOrderInLayer(GameObject piece, int order)
    {
        var spriteRenderer = piece.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }
}