using UnityEngine;

public class Heart : MonoBehaviour
{
    public enum healthStatus
    {
        full,
        damaged,
        dead
    }
    public healthStatus hS;
}
