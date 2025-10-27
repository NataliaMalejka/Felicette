using UnityEngine;

public enum GridOrientation
{
    Wall,
    Floor
}

public class Grid : MonoBehaviour
{
    public Puzzle[,] puzzles;
    public Vector3 bottomLeftCornerPosition;
    [SerializeField] public float startingYPosition = 0.0f;

    [SerializeField, Tooltip("Size of the puzzle grid.")]
    private int gridSizeHorizontal;
    public int GridSizeHorizontal
    {
        get { return gridSizeHorizontal; }
    }

    [SerializeField, Tooltip("Size of the puzzle grid.")]
    private int gridSizeVertical;
    public int GridSizeVertical
    {
        get { return gridSizeVertical; }
    }

    [SerializeField, Tooltip("Size of individual cell.")]
    public float cellSize;

    [SerializeField, Tooltip("PuzzleMap gameobject. This is very temporary solution.")]
    GameObject puzzleMap;

    [SerializeField, Tooltip("Orientation of the grid.")]
    private GridOrientation gridOrientation;

    void Start()
    {
        // Create array and set all values to null
        puzzles = new Puzzle[gridSizeHorizontal, gridSizeVertical];
        for (int i = 0; i < gridSizeHorizontal; i++)
        {
            for (int j = 0; j < gridSizeVertical; j++)
            {
                puzzles[i, j] = null;
            }
        }
        bottomLeftCornerPosition = transform.position;

        Puzzle[] pieces = puzzleMap.GetComponentsInChildren<Puzzle>();
        foreach (Puzzle piece in pieces)
        {
            Vector2Int pos = WorldToGridCoords(piece.transform.position, gridOrientation);
            if (IsValidGridPosition(pos.x, pos.y))
            {
                puzzles[pos.x, pos.y] = piece;
            }
        }

    }

    public Vector2Int WorldToGridCoords(Vector3 worldPosition, GridOrientation grid)
    {
        //int vertical = gridOrientation == GridOrientation.Wall
        //    ? Mathf.RoundToInt(worldPosition.y / cellSize)
        //    : Mathf.RoundToInt(worldPosition.z / cellSize);

        //int horizontal = Mathf.RoundToInt(worldPosition.x / cellSize);
        //return new Vector2Int(horizontal, vertical);

        int horizontal, vertical;

        horizontal = Mathf.RoundToInt((worldPosition.x - bottomLeftCornerPosition.x) / cellSize);
        vertical = Mathf.RoundToInt(worldPosition.y / cellSize);
        // if (grid == GridOrientation.Wall)
        // {
        //     horizontal = Mathf.RoundToInt(worldPosition.x / cellSize);
        //     vertical = Mathf.RoundToInt(worldPosition.y / cellSize);
        //     Debug.Log("Grid orientation is Wall, horizontal: " + horizontal + ", vertical: " + vertical);
        // }
        // else
        // {
        //     Debug.Log("Grid orientation is Floor, worldPosition: " + worldPosition + ", bottomLeftCornerPosition: " + bottomLeftCornerPosition + ", cellSize: " + cellSize);
        //     horizontal = Mathf.RoundToInt((worldPosition.x - bottomLeftCornerPosition.x) / cellSize);
        //     vertical = Mathf.RoundToInt(worldPosition.z / cellSize);
        //     Debug.Log("Grid orientation is Floor, horizontal: " + horizontal + ", vertical: " + vertical);
        // }

        return new Vector2Int(horizontal, vertical);
    }

    // Get the puzzle at the specified grid cell
    //public Puzzle GetPuzzleFromGrid(int x, int y)
    //{
    //    if (x < 0 || x >= gridSizeHorizontal || y < 0 || y >= gridSizeVertical)
    //        return null;
    //    return puzzles[x, y];
    //}

    public Puzzle GetPuzzleFromGrid(int x, int vertical)
    {
        if (!IsValidGridPosition(x, vertical))
            return null;
        return puzzles[x, vertical];
    }

    private bool IsValidGridPosition(int x, int vertical)
    {
        //return x < 0 || x >= gridSizeHorizontal || vertical < 0 || vertical >= gridSizeVertical;
        return x >= 0 && x < gridSizeHorizontal && vertical >= 0 && vertical < gridSizeVertical;
    }

    // Get the puzzle at the specified world position (useful for mouse cursor)
    public Puzzle GetPuzzleFromPosition(float horizontal, float vertical)
    {
        int i = (int)(horizontal / cellSize);
        if (i < 0 || i >= gridSizeHorizontal) return null;

        int j = (int)(vertical / cellSize);
        if (j < 0 || j >= gridSizeHorizontal) return null;
        return puzzles[i, j];
    }

    public Vector3 GridToWorldCoords(float horizontal, float vertical, GridOrientation grid)
    {
        //return gridOrientation == GridOrientation.Wall
        //    ? new Vector3(x * cellSize, vertical * cellSize, 0f)
        //    : new Vector3(x * cellSize, originalY, vertical * cellSize);

        // if (grid == GridOrientation.Wall)
        // {
            float x =  (horizontal + bottomLeftCornerPosition.x) * cellSize;
            float y =  vertical * cellSize;
            return new Vector3(x, y, 0f);
        // }
        // else 
        // {
        //     float x =  horizontal * cellSize;
        //     float z =  vertical * cellSize + 0.5f; 
        //     return new Vector3(x, 1.5f, z);
        // }
    }

    public void SetPuzzle(Puzzle puzzle, int horizontal, int vertical)
    {

        if (IsValidGridPosition(horizontal, vertical))
        {
            puzzles[horizontal, vertical] = puzzle;
        }
        else
        {
            Debug.LogWarning($"[SetPuzzle] Pr�ba ustawienia puzzla poza zakresem: ({horizontal}, {vertical})");
        }
    }

    public int GetGridSizeHorizontal()
    {
        return gridSizeHorizontal;
    }

    public int GetGridSizeVertical()
    {
        return gridSizeVertical;
    }
    
    // public float GetDefaultZPosition()
    // {
    //     return ;
    // }
}
