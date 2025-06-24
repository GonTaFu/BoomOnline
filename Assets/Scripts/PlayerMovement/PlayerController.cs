using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D playerRb;
    private float HorizontalInput;
    private float VerticalInput;

    public bool inBubble = false;

    public GameObject bomberPrefab;

    public Animator playerAnim;
    [SerializeField] float moveSpeed = 5f;
}
