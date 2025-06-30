using System.Collections;
using UnityEngine;

public class PlayerAnimate : PlayerMovement
{
    public PlayerMovement player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatePlayer();
    }

    void AnimatePlayer()
    {
        if (player != null)
        {
            // Example animation logic based on player movement
            if (IsMoving())
            {
                // Play walking animation
                playerAnim.SetBool("isMoving", true);
                playerAnim.SetFloat("MoveX", HorizontalInput);
                playerAnim.SetFloat("MoveY", VerticalInput);
            }
            else
            {
                // Stop Running animation
                playerAnim.SetBool("isMoving", false);
            }
        }

        if (inBubble)
        {
            // Play bubble animation
            playerAnim.SetBool("inBubble", true);
            playerAnim.SetBool("isMoving", false);
        }
        else if (!inBubble)
        {
            // Stop bubble animation
            playerAnim.SetBool("inBubble", false);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //If Enemy collides with player in bubble, play animation
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (inBubble)
            {
                playerAnim.SetBool("EnemyTouch", true);
                playerAnim.SetBool("isMoving", false);
            }
            //if player in bubble after 5 seconds, play animation
            else
            {
                StartCoroutine(PlayBubblePopAnimation());
            }

            //If enemy touches player, play animation
            if (!inBubble)
            {
                playerAnim.SetBool("EnemyTouch", true);
                playerAnim.SetBool("isMoving", false);
            }
        }
    }

    private IEnumerator PlayBubblePopAnimation()
    {
        yield return new WaitForSeconds(5f);
        playerAnim.SetBool("EnemyTouch", true);
        playerAnim.SetBool("isMoving", false);
    }

}
