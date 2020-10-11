using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameConstants;

public class GameManager : MonoBehaviour
{
    public List<GameObject> missileObjects;
    public List<Mesh> missileMeshes;
    public List<Material> materials;

    public GameObject ragDoll;

    List<GameObject> ragDollPool;

    // Start is called before the first frame update
    void Start()
    {
        CreateRagdollPool();
    }

    void CreateRagdollPool()
    {
        ragDollPool = new List<GameObject>();
        // Create RagDollPool
        for (int i = 0; i < GameConstants.RagDollCount; i++)
        {
            GameObject rgInstance = GameObject.Instantiate(ragDoll);
            rgInstance.transform.position = new Vector3(1000f, 1000f, 1000f);
            ragDollPool.Add(rgInstance);
            rgInstance.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetRagdollInstance()
    {
        GameObject rgInstance = ragDollPool[0];
        ragDollPool.Remove(rgInstance);
        rgInstance.SetActive(true);
        return rgInstance;
    }

    public void MissleDropdownValueChanged(int index, int changeIndex)
    {
        missileObjects[index].GetComponent<Projectile>().ChangeProjectileType((ProjectileType)changeIndex);
    }

    public void MaterialDropdownValueChanged(int index, int changeIndex)
    {
        missileObjects[index].GetComponent<Projectile>().ChangeProjectileMaterial((ProjectileColour)changeIndex);
    }

    public void FireAway(int index)
    {
        missileObjects[index].GetComponent<Projectile>().FireMyself();    
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
    }
}
