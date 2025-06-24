    using System.Collections;
    using UnityEngine;
    using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    [SerializeField] private CircleCollider2D bombCollider;
    [SerializeField] private CircleCollider2D bomberCollider;

    public float timeDestory = 2f;

    [SerializeField] private Tilemap tileDestruction;
    [SerializeField] private Tilemap tileIndestructions;

    public GameObject flameStartPrefab;
    public GameObject flameMiddlePrefab;
    public GameObject flameEndPrefab;
    public GameObject brickDestroyPrefab;

    private bool playerInside;

    public int rangeDestruct = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileDestruction = GameObject.Find("Destruction").GetComponent<Tilemap>();
        tileIndestructions = GameObject.Find("Indestruction").GetComponent<Tilemap>();

        // bombCollider = GetComponent<CircleCollider2D>();
        // bomberCollider = GameObject.Find("Player").GetComponent<CircleCollider2D>();

        // Bỏ qua va chạm ban đầu
        // Physics2D.IgnoreCollision(bombCollider, bomberCollider, true);
        // playerInside = true;

        StartCoroutine(DestroyAfter());
    }

    // private void Update()
    // {

    //     // Giả sử khi đặt bomber đặt bomb thì vị trí bomber == bomb == 0
    //     // => Đi ra thì kích hoạt va chạm
    //     if (!playerInside) return;
    //     if (Vector2.Distance(transform.position, bomberCollider.transform.position) > 0.5f)
    //     {
    //         playerInside = false;
    //         Physics2D.IgnoreCollision(bombCollider, bomberCollider, false); // Kích hoạt lại va chạm
    //     }
    // }

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(timeDestory);

        // Debug.Log("DestroyAfter");

        Vector3Int center = tileDestruction.WorldToCell(transform.position);

        // Nổ phần Up - Down
        Explode(rangeDestruct, center, Vector3Int.up);
        Explode(rangeDestruct, center, Vector3Int.down);

        // Nổ phần Left - Right
        Explode(rangeDestruct, center, Vector3Int.left);
        Explode(rangeDestruct, center, Vector3Int.right);

        // Âm thanh nổ
        // SoundManager.PlaySound(SoundType.EXPLOSION);

        Destroy(gameObject);

        // Hiệu ứng nổ ở quả bom - start
        ExplodeEffAt(flameStartPrefab, transform.position, Vector3Int.zero);

        // gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    void ExplodeEffAt(GameObject flamePrefab, Vector2 position, Vector3Int path)
    {
        Quaternion rotation = flamePrefab.transform.rotation;
        if (path == Vector3Int.left)
        {
            rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (path == Vector3Int.right)
        {
            rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (path == Vector3Int.up)
        {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (path == Vector3Int.down)
        {
            rotation = Quaternion.Euler(0, 0, 180);
        }

        Instantiate(flamePrefab, position, rotation);
    }

    void Explode(int range, Vector3Int start, Vector3Int path)
    {
        for (int i = 1; i <= range; i++)
        {
            Vector3Int tilePos = start + (path * i);

            TileBase tileDestr = tileDestruction.GetTile(tilePos);
            TileBase tileInter = tileIndestructions.GetTile(tilePos);

            // Kiểm tra xem có vật thể nào không cho phá chặn ở phía trước không
            if (tileInter != null) return;

            // Phá các bức tường cho phép
            if (tileDestr != null)
            {
                // ExplodeEffAt(brickDestroyPrefab, tileDestruction.GetCellCenterWorld(tilePos), Vector3Int.zero);
                tileDestruction.SetTile(tilePos, null);
                return;
            }

            if (i == range)
            {
                ExplodeEffAt(flameEndPrefab, tileDestruction.GetCellCenterWorld(tilePos), path);
            }
            else
            {
                ExplodeEffAt(flameMiddlePrefab, tileDestruction.GetCellCenterWorld(tilePos), path);
            }

        }
    }
}
