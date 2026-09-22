using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.position = Vector3.zero;
    }
}
