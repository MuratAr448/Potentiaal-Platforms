using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 10;
    public int healthMax = 10;
    public int def = 1;

    public int damage = 10;

    public void TakeDamage(int Pdamage)
    {
        Pdamage -= def;
        if (Pdamage >= 0)
        {
            health -= Pdamage;
        }
    }
}
