using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Player player;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        AnimationClip clip = GetComponent<Animation>().clip;
        Destroy(gameObject,clip.length);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy")&&collision.transform.TryGetComponent(out Enemy enemy))
        {
            StartCoroutine(player.HitAttack(enemy));
        }
    }
}
