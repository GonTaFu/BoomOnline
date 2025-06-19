using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D playerRb;
    private float HorizontalInput;
    private float VerticalInput;

    public GameObject bomberPrefab;

    public Animator playerAnim;
    [SerializeField] float moveSpeed = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestPlaceBombRpc(new Vector3Int(1, 1, 1), NetworkObjectId);
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestPlaceBombRpc(Vector3Int gridPosition, ulong sourcePlayerId)
    {
        // Validate tại server
        PlaceBombRpc(gridPosition, sourcePlayerId);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void PlaceBombRpc(Vector3Int pos, ulong playerId)
    {
        // Validate hợp lệ: ô trống? cooldown xong chưa? có quyền không?
        // Instantiate bomb prefab (NetworkObject)
        GameObject bomb = Instantiate(bomberPrefab, pos, Quaternion.identity);
        // bomb.GetComponent<NetworkObject>().Spawn(); // Netcode sẽ sync tới tất cả clients
    }
}
