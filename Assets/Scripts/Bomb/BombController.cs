using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BombController : MonoBehaviour
{
    public GameObject bombPrefab;
    private int bombRemaining;
    [SerializeField] private int bombAmount = 1;
    private float timeFuse = 2.0f;
    public int bombRange = 1;

    private bool canControl = true;

    public List<Vector2> bombsPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bombRemaining = bombAmount;

    }

    // Update is called once per frame
    void Update()
    {
        if (canControl == false) return;
        // Chỉ cho phép đặt đúng số lượng bomb nếu số lượng bomb (bombRemaining hết thì phải đợi hồi)
        if (bombRemaining > 0 && (Input.GetKeyDown(KeyCode.Space)))
        {
            // SoundManager.PlaySound(SoundType.PLACEBOMB);
            StartCoroutine(PlaceBomb());
        }
    }

    public void ButtonPlaceBomb()
    {
        if (canControl == false) return;
        // Chỉ cho phép đặt đúng số lượng bomb nếu số lượng bomb (bombRemaining hết thì phải đợi hồi)
        if (bombRemaining > 0)
        {
            // SoundManager.PlaySound(SoundType.PLACEBOMB);
            StartCoroutine(PlaceBomb());
        }
    }

    IEnumerator PlaceBomb()
    {
        Vector2 placePos = transform.position;
        placePos.x = MathF.Round(placePos.x);
        placePos.y = MathF.Round(placePos.y);

        // Kiểm tra xem ở vị trí hiện tại có một trái bomb chưa
        if (bombsPos.Contains(placePos))
        {
            yield break;
        }

        // Lấy 1 trái bomb ra
        bombRemaining--;

        // Set thời gian nổ của các trái bomb về sau giống nhau
        // bombPrefab.GetComponent<Bomb>().timeDestory = timeFuse;

        GameObject bombObj = Instantiate(bombPrefab, placePos, bombPrefab.transform.rotation);
        Bomb bomb = bombObj.GetComponent<Bomb>();
        bomb.timeDestory = timeFuse;
        bomb.rangeDestruct = Mathf.Max(bombRange, bomb.rangeDestruct);

        // Lưu lại vị trí bomb thứ n vừa mới được đặt
        bombsPos.Add(placePos);
        // Debug.Log("Before: " + bombsPos);

        // Đợi thời gian nổ của các trái bomb về sau (lấy giá trị thời gian từ chính nó thay vì từ chính controller)
        yield return new WaitForSeconds(bomb.timeDestory);

        // Hồi lại một trái sau khi trái lấy ra đã nổ
        bombRemaining++;

        // Hủy vị trí sau khi nổ xong.
        bombsPos.Remove(placePos);
        // Debug.Log("After: " + bombsPos);

    }

    public void AddBombAmount(int amount)
    {
        bombAmount += amount;
        bombRemaining += amount;
    }

    public void RemoveBombAmount(int amount)
    {
        bombAmount -= amount;
        bombRemaining -= amount;
    }
}
