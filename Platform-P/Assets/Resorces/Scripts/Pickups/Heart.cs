using UnityEngine;

public class Heart : MonoBehaviour
{
    private Player player;
    [SerializeField] private int heal;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.transform.CompareTag("Player"))
        {
            player.GainHealth(heal,gameObject);
        }
        gameObject.SetActive(false);
        Destroy(gameObject,1);
    }
}
