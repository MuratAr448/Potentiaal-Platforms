using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool pickedUp = false;
    private Player player;
    [SerializeField] private int coins;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player")&& !pickedUp)
        {
            player.GainCoins(coins, gameObject);
            pickedUp = true;
            gameObject.SetActive(false);
            Destroy(gameObject, 1);
        }
    }
}
