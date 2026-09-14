using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rB2d;
    private Vector2 Movement;
    private float Speed = 5.0f;
    private int jumpCount = 0;
    private int jumpMax = 1;
    private float jumpForce = 10.0f;
    private bool isGrounded = false;
    private float coyoteTime =0.5f;
    [SerializeField] private GameObject CamPos;
    private void Start()
    {
        rB2d = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            jumpCount = 0;
            isGrounded = true;
            coyoteTime = 0.25f;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    private void Move()
    {
        Movement.x = Input.GetAxis("Horizontal");
        rB2d.linearVelocity = new Vector2(Movement.x*Speed, rB2d.linearVelocity.y);
    }
    private void Jumping()
    {
        if ((Input.GetKeyDown(KeyCode.W)|| Input.GetKeyDown(KeyCode.Space))&&(coyoteTime>0||jumpCount<jumpMax))
        {
            rB2d.linearVelocity = new Vector2(rB2d.linearVelocity.x,jumpForce);
            if (coyoteTime<0)
            {
                jumpCount++;
            }
            else
            {
                coyoteTime =-1;
            }
        }
        if (!isGrounded)
        {
            coyoteTime -= Time.deltaTime;
        }
    }
    private void Cam()
    {
        Vector3 newPos = new Vector3(transform.position.x + Movement.x * Speed, transform.position.y, transform.position.z);
        float dis = Vector3.Distance(CamPos.transform.position, newPos);
        if (dis>1)
        {
            CamPos.transform.position = Vector3.MoveTowards(CamPos.transform.position, newPos, dis*Time.deltaTime);
        }
    }
    private void Update()
    {
        /*
         To do
        LedgeGrab
        Wallslide/Walljump 
        */
        Move();
        Jumping();
        Cam();
        /*
        Done
        Left/Right movement
        jump
        Double jump
         */
    }
}
