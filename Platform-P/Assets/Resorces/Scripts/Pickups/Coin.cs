using UnityEngine;

public class Coin : MonoBehaviour
{
    private Player player;
    [SerializeField] private int coins;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            player.GainCoins(coins, gameObject);
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }
}
