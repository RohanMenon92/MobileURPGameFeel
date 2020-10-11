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
    public float implodeTime = 2f;
    bool imploding = false;

    GameManager gameManager;
    Rigidbody _rigidbody;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    GameObject targetRagdoll;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        _rigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(imploding)
        {
            ImplosionUpdate();
        }
        //waitTime -= Time.deltaTime;
        //if (waitTime <= 0)
        //{
        //}
    }

    void ImplosionUpdate()
    {
        foreach (Rigidbody rb in targetRagdoll.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(-GameConstants.implosionForce, gameObject.transform.position, 10.0f, 0.0f, ForceMode.Force);
        }

        implodeTime -= Time.deltaTime;
        if (implodeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void CreatePull(Collider other)
    {
        Transform refTransform = other.transform;
        Destroy(other.gameObject);
        GameObject ragDoll = gameManager.GetRagdollInstance();
        ragDoll.transform.position = refTransform.position;
        ragDoll.transform.rotation = refTransform.rotation;
        ragDoll.transform.localScale = refTransform.localScale;

        foreach(Rigidbody rb in ragDoll.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce((transform.position - other.transform.position).normalized * GameConstants.pullForce, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

    void CreatePush(Collider other)
    {
        //Transform refTransform = other.transform;
        //Destroy(other.gameObject);
        //GameObject ragDoll = gameManager.GetRagdollInstance();
        //ragDoll.transform.position = refTransform.position;
        //ragDoll.transform.rotation = refTransform.rotation;
        //ragDoll.transform.localScale = refTransform.localScale;

        //foreach (Rigidbody rb in ragDoll.GetComponentsInChildren<Rigidbody>())
        //{
        //    rb.AddForce((other.transform.position - transform.position).normalized * GameConstants.pushForce, ForceMode.Impulse);
        //}

        other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * GameConstants.pushForce, ForceMode.Impulse);
        Destroy(gameObject);
    }

    void CreateExplode(Collider other)
    {
        Transform refTransform = other.transform;
        Destroy(other.gameObject);
        GameObject ragDoll = gameManager.GetRagdollInstance();
        ragDoll.transform.position = refTransform.position;
        ragDoll.transform.rotation = refTransform.rotation;
        ragDoll.transform.localScale = refTransform.localScale;

        foreach (Rigidbody rb in ragDoll.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(GameConstants.explosionForce, gameObject.transform.position, 10.0f, 0.0f, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

    void CreateImplode(Collider other)
    {
        Transform refTransform = other.transform;
        Destroy(other.gameObject);
        GameObject ragDoll = gameManager.GetRagdollInstance();
        ragDoll.transform.position = refTransform.position;
        ragDoll.transform.rotation = refTransform.rotation;
        ragDoll.transform.localScale = refTransform.localScale;
        targetRagdoll = ragDoll;

        // Disable Collider
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        this.GetComponent<Collider>().enabled = false;

        imploding = true;
    }



    void CreateMultiHit(Collider other)
    {
        StartCoroutine(MultiHitRoutine(Random.Range(4, 8), other.GetComponent<Rigidbody>()));
        // Disable Collider
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        this.GetComponent<Collider>().enabled = false;
    }
    IEnumerator MultiHitRoutine(int hits, Rigidbody rb)
    {
        int currentHits = 0;
        // Push multiple times
        while (hits > currentHits)
        {
            rb.AddForce((transform.position - rb.transform.position).normalized * GameConstants.multiHitForce, ForceMode.Impulse);
            yield return new WaitForSeconds(GameConstants.multiHitTime);
            currentHits++;
        }
        if(currentHits == hits)
        {
            // Final hit, convert to ragdoll
            Transform refTransform = rb.transform;
            GameObject ragDoll = gameManager.GetRagdollInstance();
            ragDoll.transform.position = refTransform.position;
            ragDoll.transform.rotation = refTransform.rotation;
            ragDoll.transform.localScale = refTransform.localScale;
            targetRagdoll = ragDoll;
            Destroy(refTransform.gameObject);
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        switch(projectileType)
        {
            case ProjectileType.Puller:
                CreatePull(other);
                break;
            case ProjectileType.Pusher:
                CreatePush(other);
                break;
            case ProjectileType.Exploder:
                CreateExplode(other);
                break;
            case ProjectileType.Imploder:
                CreateImplode(other);
                break;
            case ProjectileType.MultiHit:
                CreateMultiHit(other);
                break;
        }
    }

    public void ChangeProjectileType(ProjectileType newType)
    {
        projectileType = newType;
        // Do Projectile Type Things
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
        _rigidbody.velocity = new Vector3(0, 0, -speed);
    }
}
