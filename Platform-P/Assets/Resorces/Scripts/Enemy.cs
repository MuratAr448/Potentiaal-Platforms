using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 10;
    public int healthMax = 10;
    public int def = 1;

    public int damage = 1;
    private Player player;

    private bool turned = false;
    public Rigidbody2D rB2d;
    public float speed;
    private bool isGrounded = true;
    private bool isJumping = false;
    private void Start()
    {
        rB2d = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.transform.CompareTag("Player"))
        {
            player.TakeDamage(damage);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    public void TakeDamage(int Pdamage)
    {
        Pdamage -= def;
        if (Pdamage >= 0)
        {
            health -= Pdamage;
        }
    }
    private void FixedUpdate()
    {
        Movement();
    }
    private void Movement()
    {
        int direction = 0;
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        if (!turned)
        {
            sprite.flipX = true;
            direction = 1;
        }
        else
        {
            sprite.flipX = false;
            direction = -1;
        }
        RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y) + Vector2.right * direction, Vector2.down,1);
        if (hit2D.collider != null)
        {
            if (hit2D.transform.CompareTag("Ground"))
            {
                rB2d.linearVelocity = new Vector2(speed * direction, rB2d.linearVelocity.y);
            }
            else if (hit2D.transform.CompareTag("Player"))
            {
                if (!isJumping)
                {
                    StartCoroutine(JumpAttack(direction));
                }
            }
        }
        else if (isGrounded)
        {
            turned = !turned;
        }
    }
    private IEnumerator JumpAttack(int direction)
    {
        isJumping = true;
        rB2d.linearVelocity = new Vector2(speed * direction, 5);
        yield return new WaitForSeconds(Time.deltaTime);
        while (!isGrounded)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isJumping = false;
    }
}
