using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Player player;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy")&&collision.transform.TryGetComponent(out Enemy enemy))
        {
            StartCoroutine(player.HitAttack(enemy));
        }
    }
}
