using System.Collections;
using UnityEngine;

public class TempEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Jump());
    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(5);
        transform.position = transform.position + Vector3.up * 5;
        StartCoroutine(Jump());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
