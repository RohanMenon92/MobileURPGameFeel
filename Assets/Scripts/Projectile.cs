using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class Projectile : MonoBehaviour
{
    public ProjectileType projectileType;
    public ProjectileColour projectileColour;

    Color baseColor;

    public float speed = 10;
    public float waitTime = 2f;

    GameManager gameManager;
    Rigidbody rigidbody;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        //waitTime -= Time.deltaTime;
        //if (waitTime <= 0)
        //{
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    public void ChangeProjectileType(ProjectileType newType)
    {
        projectileType = newType;
        // Do Projectile Type Things
        Debug.Log(newType);
        meshFilter.mesh = gameManager.missileMeshes[(int)newType];
    }

    public void ChangeProjectileMaterial(ProjectileColour newColour)
    {
        projectileColour = newColour;
        
        switch(newColour)
        {
            case ProjectileColour.Blue:
                baseColor = Color.blue;
                break;
            case ProjectileColour.Green:
                baseColor = Color.green;
                break;
            case ProjectileColour.Grey:
                baseColor = Color.gray;
                break;
            case ProjectileColour.Orange:
                baseColor = new Color(1f, 0.5f, 0f);
                break;
            case ProjectileColour.Pink:
                baseColor = new Color(1f, 0.08f, 0.58f);
                break;
            case ProjectileColour.Red:
                baseColor = Color.red;
                break;
            case ProjectileColour.Yellow:
                baseColor = Color.yellow;
                break;
        }

        meshRenderer.material = gameManager.materials[(int)newColour];
        // Do Projectile Type Things
    }

    public void FireMyself()
    {
        // Fire the missle
        rigidbody.velocity = new Vector3(0, 0, -speed);
    }
}
