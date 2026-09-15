using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
public enum StatePlayer
{
    Normal,
    Wallslide,
    Ledgegrab
}
public class Player : MonoBehaviour
{
    public StatePlayer PlayerState;
    private Rigidbody2D rB2d;
    private Vector2 Movement;
    private float Speed = 5.0f;
    private int jumpCount = 0;
    private int jumpMax = 1;
    private float jumpForce = 10.0f;
    private bool isGrounded = false;
    private float coyoteTime =0.5f;
    [SerializeField] private GameObject CamPos;
    [SerializeField] private Walls tempDisableWall;
    private bool slide = false;
    private Camera cam;
    private void Start()
    {
        rB2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            jumpCount = 0;
            isGrounded = true;
            coyoteTime = 0.25f;
            if (PlayerState == StatePlayer.Normal&& tempDisableWall!=null)
            {
                tempDisableWall = null;
            }
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
        if (PlayerState == StatePlayer.Normal)
        {
            Movement.x = Input.GetAxis("Horizontal");
            rB2d.linearVelocity = new Vector2(Movement.x * Speed, rB2d.linearVelocity.y);
        }
    }
    private void Jumping()
    {
        switch (PlayerState)
        {
            case  StatePlayer.Normal:
                if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && (coyoteTime > 0 || jumpCount < jumpMax))
                {
                    rB2d.linearVelocity = new Vector2(rB2d.linearVelocity.x, jumpForce);
                    if (coyoteTime < 0)
                    {
                        jumpCount++;
                    }
                    else
                    {
                        coyoteTime = -1;
                    }
                }
                if (!isGrounded)
                {
                    coyoteTime -= Time.deltaTime;
                }
                break;
            case  StatePlayer.Wallslide:
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
                {
                    jumpCount = 0;
                    rB2d.linearVelocity = new Vector2(rB2d.linearVelocity.x, jumpForce);
                    PlayerState = StatePlayer.Normal;
                    slide = false;
                }
                break;
            case  StatePlayer.Ledgegrab:
                LedgeGrab();
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
                {
                    transform.position = transform.position + Vector3.right * (tempDisableWall.side*0.25f);
                    jumpCount = 0;
                    rB2d.linearVelocity = new Vector2(rB2d.linearVelocity.x, jumpForce);
                    PlayerState = StatePlayer.Normal;
                }
                break;
            default: break;
        }

    }
    private void Cam()
    {
        float yCam = 0f;
        if (rB2d.linearVelocity.y<-5)
        {
            yCam = -5f;
        }else if (rB2d.linearVelocity.y>5)
        {
            yCam = 5;
        }else
        {
            yCam = rB2d.linearVelocity.y;
        }
        Vector3 newPos = new Vector3(transform.position.x + Movement.x*Speed, transform.position.y+ yCam, transform.position.z);
        float dis = Vector3.Distance(CamPos.transform.position, newPos);
        if (dis>1)
        {
            CamPos.transform.position = Vector3.MoveTowards(CamPos.transform.position, newPos, dis*Time.deltaTime);
        }
    }
    private void ScreenShake()
    {

    }
    private void CheckWallLeft()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.8f, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.8f, LayerMask.GetMask("Ground"));

        if (hitLeft.transform!=null&&PlayerState == StatePlayer.Normal)
        {
            if (hitLeft.transform.parent.TryGetComponent(out Walls walls))
            {
                if (walls!=tempDisableWall)
                {
                    tempDisableWall = walls;
                    if (hitLeft.collider.name == "Ledge")
                    {
                        PlayerState = StatePlayer.Ledgegrab;
                        transform.position = walls.RightSide.transform.position;
                    }
                    else
                    {
                        slide = true;
                        PlayerState = StatePlayer.Wallslide;
                        StartCoroutine(SlideDown(walls.RightSide.transform.position));
                    }
                }
            }
        }else if (hitRight.transform != null && PlayerState == StatePlayer.Normal)
        {
            if (hitRight.transform.parent.TryGetComponent(out Walls walls))
            {
                if (walls != tempDisableWall)
                {
                    tempDisableWall = walls;
                    if (hitRight.collider.name == "Ledge")
                    {
                        PlayerState = StatePlayer.Ledgegrab;
                        transform.position = walls.LeftSide.transform.position;
                    }
                    else
                    {
                        slide = true;
                        PlayerState = StatePlayer.Wallslide;
                        StartCoroutine(SlideDown(walls.LeftSide.transform.position));
                    }
                }
            }
        }
    }
    private IEnumerator SlideDown(Vector3 slidePos)
    {
        transform.position = new Vector3(slidePos.x, transform.position.y-Time.deltaTime, slidePos.z);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.8f, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.8f, LayerMask.GetMask("Ground"));
        bool stillWallslide = hitLeft ==true || hitRight ==true;
        yield return new WaitForSeconds(Time.deltaTime);
        if (stillWallslide&& slide)
        {
            StartCoroutine(SlideDown(new Vector3(slidePos.x, transform.position.y, slidePos.z)));
        }else
        {
            PlayerState = StatePlayer.Normal;
        }
    }
    private void LedgeGrab()
    {
        if (tempDisableWall.side==1)
        {
            transform.position = tempDisableWall.RightSide.transform.position;
        }
        else
        {
            transform.position = tempDisableWall.LeftSide.transform.position;
        }
    }
    private void Update()
    {
        /*
         To do
        */
        CheckWallLeft();
        Move();
        Jumping();
        Cam();
        /*
        Done
        Left/Right movement
        jump
        Double jump
        LedgeGrab
        Wallslide/Walljump 
         */
    }
}
