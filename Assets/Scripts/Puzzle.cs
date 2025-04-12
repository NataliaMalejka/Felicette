using UnityEngine;

enum Connection {EMPTY, HOLE, KNOB};

// TO DO Abstract with GridElement class?
public class Puzzle : MonoBehaviour
{
    const int LEFT = 0;
    const int RIGHT = 1;
    const int UP = 2;
    const int DOWN = 3;

    Connection[] connections;

    [SerializeField] private Connection left;
    [SerializeField] private Connection right;
    [SerializeField] private Connection up;
    [SerializeField] private Connection down;

    [SerializeField] public bool portable;
    [SerializeField] private bool alwaysVisible;

    private Grid grid;
    private int gridSizeX;
    private int gridSizeY;
    private GameObject player;
    private Vector3 playerPosition;
    private Vector3 puzzlePosition;
    private GameObject[] children;

    public bool isPlaced = true;

    void Start()
    {
        // TO DO - This code assumes that there is one GRID object in scene, worth remembering that, added log as reminder
        Debug.LogWarning("Puzzle: Start() - Only one Grid object should be present in the scene."); 
        grid = GameObject.FindWithTag("Grid").GetComponent<Grid>();
        player = GameObject.FindWithTag("Player");

        gridSizeX = grid.GetGridSizeX();
        gridSizeY = grid.GetGridSizeY();

        children = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            children[i] = this.transform.GetChild(i).gameObject;
        }

        connections = new Connection[] { left, right, up, down };
    }

    // Might be worth to replace this with events or something similar instead of checking every update
    void FixedUpdate()
    {
        puzzlePosition = this.transform.position;
        playerPosition = player.transform.position;

        bool isNearby = Mathf.Abs(playerPosition.y - puzzlePosition.y) < 0.1f && Mathf.Abs(playerPosition.x - puzzlePosition.x) <= 1.0f;

        if ((isNearby || alwaysVisible) && isPlaced)
        {
            foreach (GameObject child in children)
                child.SetActive(true);
        }
        else
        {
            foreach (GameObject child in children)
                child.SetActive(false);
        }
            
    }

    public bool FitsAt(int x, int y)
    {
        Puzzle neighbor;
        //left
        if(x >= 1)
        {
            neighbor = grid.GetPuzzleFromGrid(x - 1, y);
            if (neighbor != null)
            {
                if (!CanConnect(connections[LEFT], neighbor.connections[RIGHT]))
                    return false;
            }
        }

        //right
        if (x <= gridSizeX - 2)
        {
            neighbor = grid.GetPuzzleFromGrid(x + 1, y);
            if (neighbor != null)
            {
                if (!CanConnect(connections[RIGHT], neighbor.connections[LEFT]))
                    return false;
            }
        }

        //up
        if (y <= gridSizeX - 2)
        {
            neighbor = grid.GetPuzzleFromGrid(x, y + 1);
            if (neighbor != null)
            {
                if (!CanConnect(connections[UP], neighbor.connections[DOWN]))
                    return false;
            }
        }

        //down
        if (y >= 1)
        {
            neighbor = grid.GetPuzzleFromGrid(x, y - 1);
            if (neighbor != null)
            {
                if (!CanConnect(connections[DOWN], neighbor.connections[UP]))
                    return false;
            }
        }

        return true;
    }

    static bool CanConnect(Connection con1, Connection con2)
    {
        if (con1 == Connection.KNOB && con2 == Connection.HOLE)
            return true;
        if (con1 == Connection.HOLE && con2 == Connection.KNOB)
            return true;
        if (con1 == Connection.EMPTY && con2 == Connection.EMPTY)
            return false;

        return false;
    }
}