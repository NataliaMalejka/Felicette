using UnityEngine;

public class Grid : MonoBehaviour
{
    public Puzzle[,] puzzles;

    [SerializeField, Tooltip("Size of the puzzle grid.")]
    private int gridSizeX;
    public int GridSizeX
    {
        get { return gridSizeX; }
    }

    [SerializeField, Tooltip("Size of the puzzle grid.")]
    private int gridSizeY;
    public int GridSizeY
    {
        get { return gridSizeY; }
    }

    [SerializeField, Tooltip("Size of individual cell.")]
    public float cellSize;

    [SerializeField, Tooltip("PuzzleMap gameobject. This is very temporary solution.")]
    GameObject puzzleMap;

    void Start()
    {
        // Create array and set all values to null
        puzzles = new Puzzle[gridSizeX, gridSizeY];
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                puzzles[i, j] = null;
            }
        }

        // Get all the pieces and assign them to array
        // This will change!!
        Puzzle[] pieces = puzzleMap.GetComponentsInChildren<Puzzle>();
        foreach (Puzzle piece in pieces)
        {
            Vector2 pos = piece.transform.position;
            puzzles[(int)pos.x, (int)pos.y] = piece;

            int x = Mathf.RoundToInt(pos.x / cellSize);
            int y = Mathf.RoundToInt(pos.y / cellSize);

            puzzles[x, y] = piece;
        }

    }

    // Get the puzzle at the specified grid cell
    public Puzzle GetPuzzleFromGrid(int x, int y)
    {
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return null;
        return puzzles[x, y];
    }

    // Get the puzzle at the specified world position (useful for mouse cursor)
    public Puzzle GetPuzzleFromPosition(float x, float y)
    {
        int i = (int)(x / cellSize);
        if (i < 0 || i >= gridSizeX) return null;

        int j = (int)(y / cellSize);
        if (j < 0 || j >= gridSizeX) return null;

        return puzzles[i, j];
    }

    public void SetPuzzle(Puzzle puzzle, int x, int y)
    {
        puzzles[x, y] = puzzle;
    }

    public int GetGridSizeX()
    {
        return gridSizeX;
    }

    public int GetGridSizeY()
    {
        return gridSizeY;
    }
    
    // public float GetDefaultZPosition()
    // {
    //     return ;
    // }
}
