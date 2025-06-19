using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.Netcode;
public class LevelLoader : NetworkBehaviour
{
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

    // Dictionary<Vector2Int, char> dataLoadMap = new Dictionary<Vector2Int, char>(); // tạm ẩn
    char[,] mapData;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            LoadRandomLevel();
        }
        else
        {
            LoadMapFromData(mapData);
        }
    }

    private void LoadRandomLevel()
    {
        int rows = 13;
        int columns = 31;
        char[,] map = GenerateRandomMap(rows, columns);
        mapData = map;

        ApplyMap(map); // Host tự hiển thị
        string[] stringMap = CharArrayToStringArray(map);
        SendMapToClientsClientRpc(stringMap);
    }

    [ClientRpc] // cái này 
    private void SendMapToClientsClientRpc(string[] stringMap)
    {
        if (IsServer) return; // Host đã có rồi
        char[,] receivedMap = StringArrayToCharArray(stringMap);
        ApplyMap(receivedMap);
    }

    private void ApplyMap(char[,] map)
    {
        int rows = map.GetLength(0);
        int columns = map.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                char type = map[row, col];
                Vector3Int tilePos = new Vector3Int(col - 1, -row - 1, 0);
                Vector3 worldPos = new Vector3(col, -row, 0);

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


    private void LoadMapFromData(char[,] map)
    {
        int rows = 13;
        int columns = 31;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                char type = map[row, col];
                Vector3Int tilePos = new Vector3Int(col - 1, -row - 1, 0);
                Vector3 worldPos = new Vector3(col, -row, 0);
                switch (type)
                {
                    case '*':
                        Debug.Log("*");
                        SetTile(destructionTilemap, brickTile, tilePos);
                        // SpawnItemWithBrick(tilePos, worldPos);
                        break;
                    case '#':
                        Debug.Log("#");
                        SetTile(indestructionTilemap, blockTile, tilePos);
                        break;
                }

            }
        }
    }

    private char[,] GenerateRandomMap(int rows, int columns)
    {
        char[,] map = new char[rows, columns];

        // Khởi tạo toàn bộ bản đồ là cỏ (' ')
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                map[row, col] = ' ';
            }
        }

        // Đặt tường không thể phá hủy (#) ở biên
        for (int row = 0; row < rows; row++)
        {
            map[row, 0] = '#';
            map[row, columns - 1] = '#';
        }
        for (int col = 0; col < columns; col++)
        {
            map[0, col] = '#';
            map[rows - 1, col] = '#';
        }

        // Đặt tường không thể phá hủy (#) ở lưới 2x2
        for (int row = 2; row < rows - 1; row += 2)
        {
            for (int col = 2; col < columns - 1; col += 2)
            {
                map[row, col] = '#';
            }
        }

        // Đặt người chơi (p) ở góc trên bên trái
        map[1, 1] = 'p';
        map[1, 2] = ' ';
        map[2, 1] = ' ';
        map[2, 2] = ' ';

        map[1, 3] = '*';
        map[2, 3] = '*';
        map[3, 3] = '*';
        map[3, 1] = '*';
        map[2, 1] = '*';
        map[3, 1] = '*';

        // Đặt gạch có thể phá hủy (*) ở các vị trí ngẫu nhiên, trừ vùng an toàn quanh người chơi
        float brickChance = 0.5f;
        for (int row = 1; row < rows - 1; row++)
        {
            for (int col = 1; col < columns - 1; col++)
            {
                // Chỉ đặt gạch ở các ô trống và không nằm trong vùng an toàn (1,2), (2,1), (2,2), (1,3), (2,3), (3,1), (3,2)
                if (map[row, col] == ' ' &&
                    !(row == 1 && col == 2) &&
                    !(row == 2 && col == 1) &&
                    !(row == 2 && col == 2) &&
                    !(row == 1 && col == 3) &&
                    !(row == 2 && col == 3) &&
                    !(row == 3 && col == 1) &&
                    !(row == 3 && col == 2))
                {
                    if (Random.value < brickChance)
                    {
                        map[row, col] = '*';
                    }
                }
            }
        }

        return map;
    }

    private Vector2Int GetRandomEmptyPosition(char[,] map, int rows, int columns)
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();
        for (int row = 1; row < rows - 1; row++)
        {
            for (int col = 1; col < columns - 1; col++)
            {
                // Chỉ thêm các ô trống và không nằm trong vùng an toàn (1,2), (2,1), (2,2), (1,3), (2,3), (3,1), (3,2)
                if (map[row, col] == ' ' &&
                    !(row == 1 && col == 2) &&
                    !(row == 2 && col == 1) &&
                    !(row == 2 && col == 2) &&
                    !(row == 1 && col == 3) &&
                    !(row == 2 && col == 3) &&
                    !(row == 3 && col == 1) &&
                    !(row == 3 && col == 2))
                {
                    emptyPositions.Add(new Vector2Int(col, row));
                }
            }
        }

        if (emptyPositions.Count == 0)
        {
            Debug.LogError("No empty positions available for spawning!");
            return new Vector2Int(1, 1);
        }

        int index = Random.Range(0, emptyPositions.Count);
        return emptyPositions[index];
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

    private string[] CharArrayToStringArray(char[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        string[] result = new string[rows];
        for (int row = 0; row < rows; row++)
        {
            char[] rowChars = new char[cols];
            for (int col = 0; col < cols; col++)
            {
                rowChars[col] = map[row, col];
            }
            result[row] = new string(rowChars);
        }
        return result;
    }

    private char[,] StringArrayToCharArray(string[] lines)
    {
        int rows = lines.Length;
        int cols = lines[0].Length;
        char[,] map = new char[rows, cols];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                map[row, col] = lines[row][col];
            }
        }
        return map;
    }

}
