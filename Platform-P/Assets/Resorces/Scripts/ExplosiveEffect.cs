using Unity.Cinemachine;
using UnityEngine;

public class ExplosiveEffect : MonoBehaviour
{
    
    void Start()
    {
        Destroy(gameObject,5);
    }

}
