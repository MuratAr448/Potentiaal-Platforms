using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.U2D.Animation;
public class Player : MonoBehaviour
{
    private enum StatePlayer
    {
        Normal,
        Wallslide,
        Ledgegrab
    }
    private StatePlayer PlayerState;
    private Rigidbody2D rB2d;
    private Vector2 Movement;
    private float Speed = 5.0f;
    private int jumpCount = 0;
    private int jumpMax = 1;
    private float jumpForce = 10.0f;
    public bool isGrounded = false;
    private float coyoteTime =0.5f;
    [SerializeField] private GameObject CamPos;
    private Walls tempDisableWall;
    private bool slide = false;
    [SerializeField] private Animator Animation;
    private void Start()
    {
        rB2d = GetComponent<Rigidbody2D>();
    }

    public void JumpAble()
    {
        jumpCount = 0;
        isGrounded = true;
        coyoteTime = 0.25f;
        if (PlayerState == StatePlayer.Normal && tempDisableWall != null)
        {
            tempDisableWall = null;
        }
    }
    private void Move()
    {
        Movement.x = Input.GetAxis("Horizontal");
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        if (PlayerState == StatePlayer.Normal)
        {
            Speed = sprint ? 7 : 5;
            rB2d.linearVelocity = new Vector2(Movement.x * Speed, rB2d.linearVelocity.y);
        }
    }
    private void Jumping()
    {
        bool jumpAction = Input.GetKeyDown(KeyCode.W);
        switch (PlayerState)
        {
            case  StatePlayer.Normal:
                if (jumpAction && (coyoteTime > 0 || jumpCount < jumpMax))
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
                if (jumpAction)
                {
                    jumpCount = 0;
                    rB2d.linearVelocity = new Vector2(rB2d.linearVelocity.x, jumpForce);
                    PlayerState = StatePlayer.Normal;
                    slide = false;
                }
                break;
            case  StatePlayer.Ledgegrab:
                LedgeGrab();
                if (jumpAction)
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
        Vector3 addPos = new Vector3(Movement.x * Speed, yCam, -10);
        Vector3 newPos = transform.position+ addPos;
        
        float dis = Vector3.Distance(CamPos.transform.position, newPos);
        if (dis>1)
        {
            CamPos.transform.position = Vector3.MoveTowards(CamPos.transform.position, newPos, dis*dis*Time.deltaTime);
        }
        else
        {
            CamPos.transform.position = Vector3.MoveTowards(CamPos.transform.position, newPos, dis * Time.deltaTime);
        }
    }
    private void animation()
    {
        if(PlayerState==StatePlayer.Normal)
        {
            if (rB2d.linearVelocity.x > 0.1f)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = false;
            }
            else if (rB2d.linearVelocity.x < -0.1f)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
        }

        if (!isGrounded)
        {
            Animation.Play("PlayerJump");
            //jump animation
        }
        else if (Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D))
        {
            Animation.Play("PlayerMove");
            //move animation
        }
        else
        {
            Animation.Play("PlayerIdle");
            //Idle
        }
    }
    private void CheckWalls()
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
        CheckWalls();
        Move();
        Jumping();
        //Cam();
        animation();
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
