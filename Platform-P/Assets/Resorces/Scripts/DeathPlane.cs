using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private Player player;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.position = Vector3.zero+Vector3.down;
        if (collision.transform.CompareTag("Player"))
        {
            player.TakeDamage(1);
        }
    }
}
