using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour
{
    [Header("Tile Assets")]
    [SerializeField] private TileBase blockTile;      // Wall
    [SerializeField] private TileBase brickTile;      // Brick
    [SerializeField] private TileBase grassTile;      // Grass

    [Header("Prefabs")]
    // [SerializeField] private GameObject bomberPrefab;
    // [SerializeField] private GameObject balloonPrefab;
    // [SerializeField] private GameObject enemyPrefab2;
    // [SerializeField] private GameObject portalPrefab;
    // [SerializeField] private GameObject bombItemPrefab;
    // [SerializeField] private GameObject flameItemPrefab;
    // [SerializeField] private GameObject speedItemPrefab;

    [Header("Map Settings")]
    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;

    [Header("Tilemap Layers")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap destructionTilemap;
    [SerializeField] private Tilemap indestructionTilemap;

    private void Start()
    {
        LoadRandomLevel();
    }

    private void LoadRandomLevel()
    {
        bool bomberSpawned = false;
        // bool portalSpawned = false;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                float rand = Random.value;

                // Đặt nền (grass) vào floor luôn
                floorTilemap.SetTile(tilePos, grassTile);

                // Tường viền không phá được
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    indestructionTilemap.SetTile(tilePos, blockTile);
                    continue;
                }

                GameObject prefabToSpawn = null;

                if (rand < 0.1f)
                {
                    indestructionTilemap.SetTile(tilePos, blockTile);
                }
                else if (rand < 0.3f)
                {
                    destructionTilemap.SetTile(tilePos, brickTile);
                }
                else
                {
                    float entityRand = Random.value;

                    if (!bomberSpawned && entityRand < 0.05f)
                    {
                        // prefabToSpawn = bomberPrefab;
                        bomberSpawned = true;
                    }
                    // else if (!portalSpawned && entityRand < 0.1f)
                    // {
                    //     prefabToSpawn = portalPrefab;
                    //     portalSpawned = true;
                    // }
                    // else if (entityRand < 0.2f)
                    // {
                    //     prefabToSpawn = balloonPrefab;
                    // }
                    // else if (entityRand < 0.25f)
                    // {
                    //     prefabToSpawn = enemyPrefab2;
                    // }
                    // else if (entityRand < 0.3f)
                    // {
                    //     prefabToSpawn = bombItemPrefab;
                    // }
                    // else if (entityRand < 0.35f)
                    // {
                    //     prefabToSpawn = flameItemPrefab;
                    // }
                    // else if (entityRand < 0.4f)
                    // {
                    //     prefabToSpawn = speedItemPrefab;
                    // }
                }

                if (prefabToSpawn != null)
                {
                    Instantiate(prefabToSpawn, floorTilemap.GetCellCenterWorld(tilePos), Quaternion.identity);
                }
            }
        }

        // Đảm bảo có Bomber và Portal
        // if (!bomberSpawned)
        // {
        //     Vector3Int pos = new Vector3Int(1, 1, 0);
        //     Instantiate(bomberPrefab, floorTilemap.GetCellCenterWorld(pos), Quaternion.identity);
        // }

        // if (!portalSpawned)
        // {
        //     Vector3Int pos = new Vector3Int(width - 2, height - 2, 0);
        //     Instantiate(portalPrefab, floorTilemap.GetCellCenterWorld(pos), Quaternion.identity);
        // }
    }
}
