using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManagerV3 : MonoBehaviour
{
    public static GameManagerV3 Instance { get; private set; }

    [Header("Tile Assets")]
    [SerializeField] private Tile blockTile;      // Wall
    [SerializeField] private Tile brickTile;      // Brick
    [SerializeField] private Tile grassTile;      // Grass

    [Header("Map Settings")]
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;

    [Header("Tilemap Layers")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap destructionTilemap;
    [SerializeField] private Tilemap indestructionTilemap;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameManagerV3 detected. Destroying this instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: If you want it to persist across scenes
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyMap(char[,] map)
    {
        int rows = map.GetLength(0);
        int columns = map.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                char type = map[row, col];
                Vector3Int tilePos = new Vector3Int(col - 1, -row - 1, 0);

                switch (type)
                {
                    case '*':
                        SetTile(destructionTilemap, brickTile, tilePos);
                        break;
                    case '#':
                        SetTile(indestructionTilemap, blockTile, tilePos);
                        break;
                }
            }
        }
    }

    private void SetTile(Tilemap tilemap, Tile tile, Vector3Int pos)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is not assigned.");
            return;
        }
        tilemap.SetTile(pos, tile);
    }
}
