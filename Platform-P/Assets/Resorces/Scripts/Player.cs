using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rB2d;
    private Vector2 Movement;
    private float Speed = 5.0f;
    private bool normalJump;
    private bool doubleJump;
    private bool coyoteTime;
    private void Start()
    {
        rB2d = GetComponent<Rigidbody2D>();
    }
    private void Move()
    {
        Movement.x = Input.GetAxis("Horizontal");
        Movement.y = Input.GetAxis("Vertical");
        
        Vector3 Direction = new Vector3(Movement.x, Movement.y, 0);
        transform.Translate(Direction * Speed * Time.deltaTime);
        /*
         Left/Right movement
         Jump
         Double Jump
         LedgeGrab
         Wallslide/Walljump 
         Coyote jump
         */
    }

    private void Update()
    {
        Move();
    }
}
