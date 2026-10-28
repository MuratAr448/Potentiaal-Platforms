using UnityEngine;

public class Heart : MonoBehaviour
{
    private bool pickedUp = false;
    private Player player;
    [SerializeField] private int heal;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && !pickedUp)
        {
            player.GainHealth(heal,gameObject);
            gameObject.SetActive(false);
            Destroy(gameObject, 1);
            pickedUp = true;
        }
    }
}
