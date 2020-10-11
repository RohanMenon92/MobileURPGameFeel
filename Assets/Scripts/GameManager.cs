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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
