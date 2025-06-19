using System.Collections;
using UnityEngine;

public class Flame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfter());

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D col in hits)
        {
            if (col.CompareTag("Item"))
            {
                Destroy(col.gameObject);
            }
        }
    }

    IEnumerator DestroyAfter() {
        yield return new WaitForSeconds(0.75f);

        Destroy(gameObject);
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("Kill Player");

    //         collision.gameObject.GetComponent<PlayerController>().SetIsDead(true);
    //     }
    //     if (collision.gameObject.CompareTag("Enemy"))
    //     {
    //         Debug.Log("Kill Enemy");

    //         // collision.gameObject.GetComponent<Enemy>().SetIsDead(true);
    //         collision.gameObject.GetComponent<Enemy>().TakeDamage();
    //     }

    //     if (collision.gameObject.CompareTag("Bomb"))
    //     {
    //         Debug.Log("Hit Bomb");

    //         collision.gameObject.GetComponent<Bomb>().timeDestory = 0f;
    //     }
    // }
}
