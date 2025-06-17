using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private float HorizontalInput;
    private float VerticalInput;
    [SerializeField] float moveSpeed = 5f;
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(HorizontalInput, VerticalInput).normalized;

        playerRb.linearVelocity = move * moveSpeed;
        
    }

}
