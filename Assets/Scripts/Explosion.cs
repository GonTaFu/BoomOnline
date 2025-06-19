using UnityEngine;

public class Explosion : MonoBehaviour
{
    public GameObject startGO;
    public GameObject middleGO;
    public GameObject endGO;

    public float explosionTime = 0.5f;

    public void SetActiveRenderer(GameObject active)
    {
        // Tắt tất cả các đối tượng con
        startGO.SetActive(false);
        middleGO.SetActive(false);

        // Bật đối tượng con mà bạn muốn hiển thị
        active.SetActive(true);
        DestroyAfter();
    }

    public void SetDirection(Vector2 direction)
    {
        // Quay hướng của đối tượng theo hướng nổ
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    // Gọi từ Animation Event để tự hủy đối tượng khi hoạt ảnh kết thúc
    public void DestroyAfter()
    {
        // Xóa gameObject cha sau 0.5 giây
        Destroy(gameObject, explosionTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu va chạm với một "Enemy", bỏ qua
        if (collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        // Nếu va chạm với các đối tượng khác, xử lý bình thường
    }
}
