using UnityEngine;

public class Walls : MonoBehaviour
{
    private Player player;
    public float side = 0;
    public bool ledgeGrabed;
    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void CheckSide()
    {
        if (player != null)
        {
            if (player.transform.position.x > transform.position.x)
            {
                side = 1f;
            }
            else
            {
                side = -1f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckSide();
    }
}
