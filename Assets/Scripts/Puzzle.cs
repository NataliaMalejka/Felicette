using UnityEngine;
using UnityEngine.VFX;

enum Connection {EMPTY, HOLE, KNOB};

// TO DO Abstract with GridElement class?
public class Puzzle : MonoBehaviour
{
    const int LEFT = 0;
    const int UP = 1;
    const int RIGHT = 2;
    const int DOWN = 3;


    Connection[] connections;

    [SerializeField] private Connection left;
    [SerializeField] private Connection right;
    [SerializeField] private Connection up;
    [SerializeField] private Connection down;
    [SerializeField] private VisualEffect visualEffectFog;
    [SerializeField] private Material pickUpMaterial;
    private Material defaultMaterial;

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
        grid = GetComponentInParent<Grid>();
        player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().OnPlayerMoved.AddListener(OnPlayerMoved);
        defaultMaterial = GetComponent<Renderer>().material;

        gridSizeX = grid.GetGridSizeX();
        gridSizeY = grid.GetGridSizeY();

        // TO DO - Is it quicker to create and use an array instead of accessing transform.childCount and GetChild() directly every time?
        children = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            children[i] = this.transform.GetChild(i).gameObject;
        }

        //Run once to set the initial visibility of the puzzle
        OnPlayerMoved(player.transform.position);
        visualEffectFog.SetVector3("PlayerPosition", player.transform.position);

        connections = new Connection[] { left, up, right, down };
        FixConnectionsRotation();
    }

    private void OnPlayerMoved(Vector3 playerPosition)
    {
        visualEffectFog.SetVector3("PlayerPosition", playerPosition);
    }

    public void ActOnPuzzlePlaced()
    {
        if (visualEffectFog != null)
            visualEffectFog.gameObject.SetActive(true);
        GetComponent<Renderer>().material = defaultMaterial;
        SetOrderingLayer("Default");
    }

    public void ActOnPuzzlePicked()
    {
        if (visualEffectFog != null)
            visualEffectFog.gameObject.SetActive(false);
        if (pickUpMaterial != null)
        {
            pickUpMaterial.SetColor("_glow_color", new Color(0.745f, 0.0078f, 0f, 1f));
            GetComponent<Renderer>().material = pickUpMaterial;

            SetOrderingLayer("Top");
        }
    }

    private void SetOrderingLayer(string layer, GameObject obj = null)
    {
        if (obj == null)
            obj = gameObject;

        if (layer != "Top" && layer != "Default")
        {
            Debug.LogError("Puzzle.SetOrderingLayer: Invalid layer name. Use 'Top' or 'Default'");
            return;
        }

        var spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = layer;
        }
    }

    public bool FitsAt(int x, int y)
    {
        Puzzle neighbor;
        //left
        if (x >= 1)
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

    // Rotates the logical connections of a puzzle based on object's rotation along Z axis
    void FixConnectionsRotation()
    {
        int rotation = Mathf.RoundToInt(transform.rotation.eulerAngles.z);
        int amount = rotation / 90;

        Connection[] oldConnections = (Connection[])connections.Clone();
        connections[0] = oldConnections[(0 + amount) % 4];
        connections[1] = oldConnections[(1 + amount) % 4];
        connections[2] = oldConnections[(2 + amount) % 4];
        connections[3] = oldConnections[(3 + amount) % 4];

        left = connections[LEFT];
        right = connections[RIGHT];
        up = connections[UP];
        down = connections[DOWN];
    }

    public void Shrink()
    {
        float newScale = transform.localScale.x * 0.5f;
        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
    }
    public void ExpandToDefaultSize()
    {
        float newScale = transform.localScale.x / 0.5f;
        transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
    }

    public void ChangeShaderColor(bool isPlacable)
    {
        //can be optimized
        if (isPlacable)
        {
            pickUpMaterial.SetColor("_glow_color", new Color(0.015f, 0.604f, 0.195f, 1f));
        }
        else
        {
            pickUpMaterial.SetColor("_glow_color", new Color(0.745f, 0.0078f, 0f, 1f));
        }
    }
}