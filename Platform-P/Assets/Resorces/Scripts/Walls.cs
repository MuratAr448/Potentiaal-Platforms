using UnityEngine;

public class Walls : MonoBehaviour
{
    private Player player;
    public int side = 0;
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
                side = 1;
            }
            else
            {
                side = -1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckSide();
    }
}
