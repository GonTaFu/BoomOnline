using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public Tilemap tilemap;
    public LayerMask obstacleMask;

    private HashSet<Vector3Int> bombPlanted = new HashSet<Vector3Int>();

    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    private float bombTimeRefuse = 3f;
    public int bombAmount = 1;            // Tổng số bom được đặt cùng lúc
    public int bombRemaining;             // Số bom hiện còn được đặt

    [Header("Explosion Settings")]
    public Explosion explosionPrefab;
    public float explosionTime = 1.0f;
    public int ExplosionRadius = 1;

    private void Start()
    {
        loadTitleMap();
    }

    private void OnEnable()
    {
        if (bombRemaining <= 0) // Chỉ set nếu lần đầu
            bombRemaining = bombAmount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bombRemaining > 0)
        {
            StartCoroutine(bombPlace());
        }
    }

    private IEnumerator bombPlace()
    {
        

        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        Vector3 placePos = tilemap.GetCellCenterWorld(cellPos);

        if (bombPlanted.Contains(cellPos))
            yield break;

        bombPlanted.Add(cellPos);
        GameObject bomb = Instantiate(bombPrefab, placePos, Quaternion.identity);
        bombRemaining--;

        yield return new WaitForSeconds(bombTimeRefuse);

        // Nổ chính giữa
        Explosion explosion = Instantiate(explosionPrefab, placePos, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.startGO);

        // Nổ 4 hướng
        Explode(cellPos, Vector2Int.up, ExplosionRadius);
        Explode(cellPos, Vector2Int.down, ExplosionRadius);
        Explode(cellPos, Vector2Int.left, ExplosionRadius);
        Explode(cellPos, Vector2Int.right, ExplosionRadius);

        Destroy(bomb);
        bombRemaining++; // Cho đặt lại bom sau khi nổ
        bombPlanted.Remove(cellPos);
    }

    private void Explode(Vector3Int cellPos, Vector2Int direction, int length)
    {
        if (length <= 0) return;

        Vector3Int nextCell = cellPos + new Vector3Int(direction.x, direction.y, 0);
        Vector3 worldPos = tilemap.GetCellCenterWorld(nextCell);

        var destructables = GameObject.FindGameObjectsWithTag("Destructable");
        // if (destructables.Length > 0)
        // {
        //     destructables[0].GetComponent<DestructableTilemap>().DestroyTileAtWorldPosition(worldPos);
        // }

        Collider2D hit = Physics2D.OverlapBox(worldPos, Vector2.one * 0.5f, 0f);
        if (hit != null)
        {
            if (hit.CompareTag("Bomb"))
            {
                return; // Chặn flame lan
            }

            // if (hit.CompareTag("Enemy"))
            // {
            //     Enemy enemy = hit.GetComponent<Enemy>();
            //     if (enemy != null)
            //     {
            //         enemy.Die(); // Gọi hàm chết của Enemy
            //     }
            //     Destroy(hit.gameObject, 0.5f); // Flame giết enemy
            // }
            if (hit.CompareTag("Destructable") || hit.CompareTag("Indestructable"))
            {
                return; // Chặn Flame bởi Destructable và Indestructable
            }
        }

        Explosion explosion = Instantiate(explosionPrefab, worldPos, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middleGO : explosion.endGO);
        explosion.SetDirection(direction);

        Explode(nextCell, direction, length - 1);
    }
    private void loadTitleMap()
    {
        tilemap = GameObject.Find("Destructable").GetComponent<Tilemap>();
        // brickTilemap = GameObject.Find("Destructable").GetComponent<Tilemap>();
    }
}
