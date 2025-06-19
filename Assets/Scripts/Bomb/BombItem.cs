// using System.Collections;
// using UnityEngine;

// public class BombItem : Item
// {
//     private int bombAmount = 1;
//     private float bombDuration = 5f; // Duration of the bomb effect
//     private BombController bomb;

//     public override IEnumerator Effect()
//     {
//         bomb = GameObject.Find("Player").GetComponent<BombController>();

//         if (bomb != null)
//         {
//             bomb.AddBombAmount(bombAmount);
//             yield return new WaitForSeconds(bombDuration);
//             // bomb.RemoveBombAmount(bombAmount);
//         }
//         else
//         {
//             Debug.Log("BombController not found");
//         }

//     }
    
// }
