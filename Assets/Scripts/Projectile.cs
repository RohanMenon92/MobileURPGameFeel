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

    GameObject ignitionEffect;
    private void Awake()
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
            DestroyBullet();
        }
    }

    void CreatePull(Collider other)
    {
        ImploderHitEffect(other.transform.up, Vector3.one * 0.5f);

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
        DestroyBullet();
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

        ExploderHitEffect(other.transform.up, Vector3.one * 0.5f);

        other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * GameConstants.pushForce, ForceMode.Impulse);
        DestroyBullet();
    }

    void CreateExplode(Collider other)
    {
        ExploderHitEffect(other.transform.position, Vector3.one * 1.5f);

        Transform refTransform = other.transform;
        Destroy(other.gameObject);
        GameObject ragDoll = gameManager.GetRagdollInstance();
        ragDoll.transform.position = refTransform.position;
        ragDoll.transform.rotation = refTransform.rotation;
        ragDoll.transform.localScale = refTransform.localScale;

        // lower explosion point to make character bounce up
        Vector3 explosionPoint = transform.position - new Vector3(0f, 1f, 0f);
        foreach (Rigidbody rb in ragDoll.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce((other.transform.position - explosionPoint).normalized * GameConstants.explosionForce, ForceMode.Impulse); ;
        }
        DestroyBullet();
    }

    void CreateImplode(Collider other)
    {
        ImploderHitEffect(other.transform.position, Vector3.one * 1.5f);

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
        MultiHitEffect(transform.position, other.transform.position, Vector3.one * 0.75f);

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
            Vector3 hitDir = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            // Adding Effect
            MultiHitEffect(rb.transform.position, hitDir, Vector3.one * 0.25f);

            // Adding Force
            rb.AddForce((rb.transform.position - hitDir).normalized * GameConstants.multiHitForce, ForceMode.Impulse);
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
            DestroyBullet();
        }
    }

    void ImploderHitEffect(Vector3 lookAt, Vector3 scale)
    {
        GameObject effect = gameManager.BeginEffect(EffectTypes.Implosion, transform.position, lookAt);
        effect.transform.localScale = scale;
        effect.GetComponent<ImplosionScript>().SetColor(gameManager.GetColor(projectileColour));
    }

    void ExploderHitEffect(Vector3 lookAt, Vector3 scale)
    {
        GameObject effect = gameManager.BeginEffect(EffectTypes.Explosion, transform.position, lookAt);
        effect.transform.localScale = scale;
        effect.GetComponent<ExplosionScript>().SetColor(gameManager.GetColor(projectileColour));

    }

    void MultiHitEffect(Vector3 startPos, Vector3 lookAt, Vector3 scale)
    {
        GameObject effect = gameManager.BeginEffect(EffectTypes.Ignition, startPos, lookAt);
        effect.transform.localScale = scale;
        effect.GetComponent<IgnitionScript>().SetColor(gameManager.GetColor(projectileColour));
    }

    void DestroyBullet()
    {
        gameManager.WaitAndSwitchState(GameState.Select, 2f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // On hitting the target
        bool largeShake = projectileType == ProjectileType.Exploder || projectileType == ProjectileType.Imploder;
        gameManager.OnHit(other.transform, largeShake ? 1.0f : 0.25f, largeShake ? 0.75f : 0.25f);
        
        // Projectile Type Specific hit
        switch (projectileType)
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

        baseColor = gameManager.GetColor(newColour);

        meshRenderer.material = gameManager.materials[(int)newColour];
        // Do Projectile Type Things
    }

    public void WarmUp()
    {
        ignitionEffect = gameManager.BeginEffect(EffectTypes.Ignition, transform.position, -transform.forward);
        ignitionEffect.GetComponent<IgnitionScript>().SetColor(gameManager.GetColor(projectileColour));
        ignitionEffect.transform.SetParent(transform, true);
        ignitionEffect.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void FireMyself()
    {
        // Fire the missle
        _rigidbody.velocity = new Vector3(0, 0, -speed);
    }
}
