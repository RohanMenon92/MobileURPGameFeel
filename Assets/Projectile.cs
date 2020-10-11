using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10;
    public Rigidbody _RB;
    public float waitTime = 2f;
    private void Awake()
    {
        _RB = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime <= 0)
        {
            _RB.velocity = new Vector3(0, 0, -speed);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
