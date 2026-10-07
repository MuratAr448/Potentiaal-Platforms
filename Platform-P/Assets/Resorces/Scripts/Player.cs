using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using UnityEngine.XR;
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
    private float speed = 5.0f;
    private int jumpCount = 0;
    private int jumpMax = 1;
    private float jumpForce = 12.5f;
    public bool isGrounded = false;
    private float coyoteTime = 0.5f;

    [SerializeField] private GameObject CamPos;
    private Walls tempDisableWall;
    private bool slide = false;
    [SerializeField] private Animator Animation;
    [SerializeField] private GameObject sword;
    private bool cooldown = true;

    private int attackStat = 3;
    private List<Enemy> enemys = new List<Enemy>();

    private int maxHealth = 6;
    public int health = 6;
    public List<Image> Hearts;

    public int coins = 0;
    public TextMeshProUGUI coinText;

    private float iFrames = 1f;
    private float iFrameTimer = 0;

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
            tempDisableWall.ledgeGrabed = false;
            tempDisableWall = null;
        }
    }
    private void Move()
    {
        Movement.x = Input.GetAxis("Horizontal");
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        if (PlayerState == StatePlayer.Normal)
        {
            if (isGrounded)
            {
                speed = sprint ? 10 : 7;
            }
            rB2d.linearVelocity = new Vector2(Movement.x * speed, rB2d.linearVelocity.y);
        }
    }
    private void Jumping()
    {
        bool jumpAction = Input.GetKeyDown(KeyCode.W);
        switch (PlayerState)
        {
            case StatePlayer.Normal:
                if (jumpAction && (coyoteTime > 0 || jumpCount < jumpMax))
                {
                    StartCoroutine(Jumped());
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
            case StatePlayer.Wallslide:
                if (jumpAction)
                {
                    StartCoroutine(Jumped());
                    rB2d.linearVelocity = new Vector2(rB2d.linearVelocity.x, jumpForce);
                    PlayerState = StatePlayer.Normal;
                    slide = false;
                }
                break;
            case StatePlayer.Ledgegrab:
                LedgeGrab();
                if (jumpAction)
                {
                    StartCoroutine(Jumped());
                    transform.position = transform.position + Vector3.right * (tempDisableWall.side * 0.35f);
                    rB2d.linearVelocity = new Vector2(rB2d.linearVelocity.x, jumpForce);
                    PlayerState = StatePlayer.Normal;
                }
                break;
            default: break;
        }

    }
    private IEnumerator Jumped()
    {
        rB2d.gravityScale = 1;

        while (isGrounded)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        while (!isGrounded)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            if (PlayerState != StatePlayer.Wallslide)
            {
                if (rB2d.gravityScale < 3)
                {
                    rB2d.gravityScale += 0.02f;
                }
                else
                {
                    StopCoroutine(Jumped());
                }
            }
            else
            {
                rB2d.gravityScale = 1;
            }
        }
    }
    private void Animate()
    {
        SpriteRenderer PlayerSprite = GetComponentInChildren<SpriteRenderer>();
        if (iFrameTimer<=iFrames)
        {
            PlayerSprite.enabled = !PlayerSprite.enabled;
        }
        else
        {
            PlayerSprite.enabled = true;
        }

        if (PlayerState == StatePlayer.Normal)
        {
            if (Movement.x > 0.1f)
            {
                PlayerSprite.flipX = false;
            }
            else if (Movement.x < -0.1f)
            {
                PlayerSprite.flipX = true;
            }
        }

        if (!isGrounded)
        {
            Animation.Play("PlayerJump");
            //jump animation
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))&&(rB2d.linearVelocityX>0.1f|| rB2d.linearVelocityX< -0.1f))
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

        if (hitLeft.transform != null && PlayerState == StatePlayer.Normal&&!isGrounded)
        {
            if (hitLeft.transform.parent.TryGetComponent(out Walls walls))
            {
                if (walls != tempDisableWall)
                {
                    jumpCount = 0;
                    if (tempDisableWall != null)
                    {
                        tempDisableWall.ledgeGrabed = false;
                    }

                    tempDisableWall = walls;
                    if (hitLeft.collider.name == "Ledge")
                    {
                        tempDisableWall.ledgeGrabed = true;
                        PlayerState = StatePlayer.Ledgegrab;
                        transform.position = walls.transform.position + Vector3.right * walls.side;
                    }
                    else
                    {
                        slide = true;
                        PlayerState = StatePlayer.Wallslide;
                        rB2d.linearVelocity = Vector2.zero;
                        StartCoroutine(SlideDown(walls.transform.position + Vector3.right * walls.side));
                    }
                } else if (!tempDisableWall.ledgeGrabed)
                {
                    if (hitLeft.collider.name == "Ledge")
                    {
                        tempDisableWall.ledgeGrabed = true;
                        PlayerState = StatePlayer.Ledgegrab;
                        transform.position = walls.transform.position + Vector3.right * walls.side;
                    }
                }
            }
        } 
        else if (hitRight.transform != null && PlayerState == StatePlayer.Normal && !isGrounded)
        {
            if (hitRight.transform.parent.TryGetComponent(out Walls walls))
            {
                if (walls != tempDisableWall)
                {
                    jumpCount = 0;
                    if (tempDisableWall != null)
                    {
                        tempDisableWall.ledgeGrabed = false;
                    }

                    tempDisableWall = walls;
                    if (hitRight.collider.name == "Ledge")
                    {
                        tempDisableWall.ledgeGrabed = true;
                        PlayerState = StatePlayer.Ledgegrab;
                        transform.position = walls.transform.position + Vector3.right * walls.side;
                    }
                    else
                    {
                        slide = true;
                        PlayerState = StatePlayer.Wallslide;
                        StartCoroutine(SlideDown(walls.transform.position + Vector3.right * walls.side));
                    }
                }
                else if (!tempDisableWall.ledgeGrabed)
                {
                    if (hitRight.collider.name == "Ledge")
                    {
                        tempDisableWall.ledgeGrabed = true;
                        PlayerState = StatePlayer.Ledgegrab;
                        transform.position = walls.transform.position + Vector3.right * walls.side;
                    }
                }
            }
        }
    }
    private IEnumerator SlideDown(Vector3 slidePos)
    {
        transform.position = new Vector3(slidePos.x, transform.position.y - Time.deltaTime, slidePos.z);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.8f, LayerMask.GetMask("Ground"));
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.8f, LayerMask.GetMask("Ground"));
        bool stillWallslide = hitLeft == true || hitRight == true;
        yield return new WaitForSeconds(Time.deltaTime);
        if (stillWallslide && slide)
        {
            StartCoroutine(SlideDown(new Vector3(slidePos.x, transform.position.y, slidePos.z)));
        } else
        {
            PlayerState = StatePlayer.Normal;
        }
    }
    private void LedgeGrab()
    {
        if (tempDisableWall.side == 1)
        {
            transform.position = tempDisableWall.transform.position + Vector3.right * tempDisableWall.side;
        }
        else
        {
            transform.position = tempDisableWall.transform.position + Vector3.right * tempDisableWall.side;
        }
    }
    private IEnumerator Attacking()
    {
        float attackTime = 0.6f;
        cooldown = false;
        if (!GetComponentInChildren<SpriteRenderer>().flipX)
        {
            sword.transform.position = transform.position+Vector3.right;
            sword.GetComponent<SpriteRenderer>().flipX = false;
        }else
        {
            sword.transform.position = transform.position + Vector3.left;
            sword.GetComponent<SpriteRenderer>().flipX = true;
        }
        sword.SetActive(true);
        
        yield return new WaitForSeconds(attackTime);

        sword.SetActive(false);
        if (enemys != null)
        {
            enemys.Clear();
        }
        cooldown = true;

    }
    public IEnumerator HitAttack(Enemy enemy)
    {
        if (!enemys.Contains(enemy))
        {
            int currentAttack = attackStat;
            float hitSchake = 0.05f;
            int rand = Random.Range(0, 5);
            bool crit = rand == 0 ? true : false;
            if (crit)
            {
                currentAttack *= 2;
                hitSchake *= 5;
            }
            Time.timeScale = 0;
            
            yield return new WaitForSecondsRealtime(hitSchake);

            enemy.TakeDamage(currentAttack);
            enemys.Add(enemy);
            if (Movement.x > 0.1f)
            {
                enemy.rB2d.AddForce(new Vector2(currentAttack * 100, hitSchake * 100));
            }
            else
            {
                enemy.rB2d.AddForce(new Vector2(-currentAttack*100, hitSchake * 100));
            }
            

            Time.timeScale = 1;
        }

    }
    private void UIShow()
    {
        for (int i = 0; i < Hearts.Count; i++)
        {
            Animator Heartbounce = Hearts[i].GetComponent<Animator>();
            if (health >= 2 + i + i)
            {
                //Heartbounce.Play("Full health");
                Heartbounce.SetInteger("Health", 2);
            }
            else if (health == 1 + i + i) 
            {
                //Heartbounce.Play("Damaged");
                Heartbounce.SetInteger("Health", 1);
            }
            else
            {
                //Heartbounce.Play("No Life");
                Heartbounce.SetInteger("Health", 0);
            }
        }
        coinText.text = coins.ToString();
    }
    public void TakeDamage(int damage)
    {
        if (iFrameTimer>=iFrames)
        {
            health -= damage;
            if (health <= 0)
            {
                Debug.Log("dead");
            }
            iFrameTimer = 0;
        }
    }
    public void GainHealth(int HP, GameObject gB)
    {
        health += HP;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        Destroy(gB);
    }
    public void GainCoins(int Coin, GameObject gB)
    {
        coins += Coin;
        Destroy(gB);
    }
    private void Update()
    {
        UIShow();
        CheckWalls();
        Jumping();
        Animate();
        if (Input.GetMouseButtonDown(0)&& cooldown)
        {
            StartCoroutine(Attacking());
        }
        iFrameTimer += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        Move();
    }
}
