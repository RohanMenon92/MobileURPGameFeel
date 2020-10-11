using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameConstants;
using DG.Tweening;
using System;
using UnityEngine.UI;

public enum GameState
{
    Select,
    WindUp,
    PerformAttack,
    AttackComplete
}
public class GameManager : MonoBehaviour
{
    public List<GameObject> missileObjects;
    public List<CanvasGroup> misslePanels;

    public List<Mesh> missileMeshes;
    public List<Material> materials;

    public GameObject ragDoll;
    public CanvasGroup scrollGroup;
    List<GameObject> ragDollPool;

    public GameObject ignitionPrefab;
    public GameObject explosionPrefab;
    public GameObject implosionPrefab;

    public Transform ignitionEffectsPool;
    public Transform explosionEffectsPool;
    public Transform implosionEffectsPool;
    public Transform worldEffects;

    GameState currentState = GameState.Select;
    int currentFire;
    int firedMissles = 0;
    Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= GameConstants.EffectPoolSize; i++)
        {
            GameObject newShieldhit = Instantiate(ignitionPrefab, ignitionEffectsPool);
            newShieldhit.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.EffectPoolSize; i++)
        {
            GameObject newBulletHit = Instantiate(explosionPrefab, explosionEffectsPool);
            newBulletHit.SetActive(false);
        }
        for (int i = 0; i <= GameConstants.EffectPoolSize; i++)
        {
            GameObject smokeEffect = Instantiate(implosionPrefab, implosionEffectsPool);
            smokeEffect.SetActive(false);
        }

        cameraTransform = FindObjectOfType<Camera>().transform.parent;
        CreateRagdollPool();
        cameraTransform.transform.position = GameConstants.selectPos;
        cameraTransform.transform.rotation =  Quaternion.Euler(GameConstants.selectRot);
    }

    void Update()
    {
        OnProcessState(currentState);
    }

    // FSM region

    public void WaitAndSwitchState(GameState newState, float delay)
    {
        StartCoroutine(WaitStateSwitch(newState, delay));
    }

    IEnumerator WaitStateSwitch(GameState newState, float delay)
    {
        yield return new WaitForSeconds(delay);

        SwitchState(newState);
    }

    public void SwitchState(GameState newState)
    {
        bool switchAllowed = false;

        // Do check for if switch is possible
        switch (currentState)
        {
            case GameState.Select:
                {
                    switchAllowed = newState == GameState.WindUp;
                }
                break;
            case GameState.WindUp:
                {
                    switchAllowed = newState == GameState.PerformAttack;
                }
                break;
            case GameState.PerformAttack:
                {
                    switchAllowed = newState == GameState.AttackComplete;
                }
                break;
            case GameState.AttackComplete:
                {
                    switchAllowed = newState == GameState.Select;
                }
                break;
        }

        if (switchAllowed)
        {
            OnExitState(currentState);
            currentState = newState;
            OnEnterState(currentState);
        }
    }

    // Check entry to stateEnter
    void OnEnterState(GameState stateEnter)
    {
        switch (stateEnter)
        {
            case GameState.Select:
                {
                    scrollGroup.interactable = true;
                    scrollGroup.DOFade(1f, GameConstants.scrollFade);
                    cameraTransform.DOMove(GameConstants.selectPos, GameConstants.scrollFade);
                    cameraTransform.DORotate(GameConstants.selectRot, GameConstants.scrollFade).OnComplete(() =>
                    {

                    });
                }
                break;
            case GameState.WindUp:
                {
                    missileObjects[currentFire].GetComponent<Projectile>().WarmUp();
                    cameraTransform.DOMove(GameConstants.windUpPos, GameConstants.windUpTransition);
                    cameraTransform.DORotate(GameConstants.windUpRot, GameConstants.windUpTransition).OnComplete(() => {
                        SwitchState(GameState.PerformAttack);
                    });
                }
                break;
            case GameState.PerformAttack:
                {
                    missileObjects[currentFire].GetComponent<Projectile>().FireMyself();
                }
                break;
            case GameState.AttackComplete:
                {
                    cameraTransform.DOMove(GameConstants.completeAPos, GameConstants.windUpTransition);
                    cameraTransform.DORotate(GameConstants.competeARot, GameConstants.windUpTransition).OnComplete(() => {
                        SwitchState(GameState.Select);
                    });

                }
                break;
        }
    }
    void OnExitState(GameState stateExit)
    {
        switch (stateExit)
        {
            case GameState.Select:
                {
                    scrollGroup.interactable = false;
                    misslePanels[currentFire].DOFade(0f, GameConstants.scrollFade / 2).OnComplete(() => {
                        // Fade main section on complete and resize horizontal layout
                        misslePanels[currentFire].gameObject.SetActive(false);
                        RectTransform scrollRect = scrollGroup.GetComponentInChildren<HorizontalLayoutGroup>().GetComponent<RectTransform>();
                        scrollRect.sizeDelta = new Vector2(400 * (misslePanels.Count - firedMissles), 0);
                        scrollGroup.DOFade(0f, GameConstants.scrollFade/2);
                    });
                }
                break;
            case GameState.WindUp:
                {

                }
                break;
            case GameState.PerformAttack:
                {

                }
                break;
            case GameState.AttackComplete:
                {
                }
                break;
        }
    }
    void OnProcessState(GameState stateProcess)
    {

        switch (stateProcess)
        {
            case GameState.Select:
                {

                }
                break;
            case GameState.WindUp:
                {

                }
                break;
            case GameState.PerformAttack:
                {
                    if (missileObjects[currentFire].transform != null)
                    {
                        cameraTransform.LookAt(missileObjects[currentFire].transform);
                    }
                }
                break;
            case GameState.AttackComplete:
                {
                }
                break;
        }
    }
    // FSM end region

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
        firedMissles++;
        currentFire = index;
        SwitchState(GameState.WindUp);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
    }

    internal Color GetColor(ProjectileColour newColor)
    {
        switch (newColor)
        {
            case ProjectileColour.Blue:
                return Color.blue;
            case ProjectileColour.Green:
                return Color.green;
            case ProjectileColour.Grey:
                return Color.gray;
            case ProjectileColour.Orange:
                return new Color(1f, 0.5f, 0f);
            case ProjectileColour.Pink:
                return new Color(1f, 0.08f, 0.58f);
            case ProjectileColour.Red:
                return Color.red;
            case ProjectileColour.Yellow:
                return Color.yellow;
        }
        return Color.gray;
    }

    public void CameraShake(float duration, float strength)
    {
        Camera.main.DOShakePosition(duration, strength * 3, 10, 90, true);
    }

    public void ReturnEffectToPool(GameObject effectToStore, GameConstants.EffectTypes effectType)
    {
        if (effectType == GameConstants.EffectTypes.Ignition)
        {
            // Return to normal bullet pool
            effectToStore.transform.SetParent(ignitionEffectsPool);
        }
        else if (effectType == GameConstants.EffectTypes.Explosion)
        {
            // Return to shotgun bullet pool
            effectToStore.transform.SetParent(explosionEffectsPool);
        }
        else if (effectType == GameConstants.EffectTypes.Implosion)
        {
            // Return to laser bullet pool
            effectToStore.transform.SetParent(implosionEffectsPool);
        }

        effectToStore.gameObject.SetActive(false);
        effectToStore.transform.localScale = Vector3.one;
        effectToStore.transform.position = Vector3.zero;
    }

    public GameObject BeginEffect(GameConstants.EffectTypes effectType, Vector3 position, Vector3 lookAt)
    {
        GameObject effectObject = null;

        switch (effectType)
        {
            // Get First Child, set parent to gunport (to remove from respective pool)
            case GameConstants.EffectTypes.Implosion:
                effectObject = implosionEffectsPool.GetComponentInChildren<ImplosionScript>(true).gameObject;
                break;
            case GameConstants.EffectTypes.Explosion:
                effectObject = explosionEffectsPool.GetComponentInChildren<ExplosionScript>(true).gameObject;
                break;
            case GameConstants.EffectTypes.Ignition:
                effectObject = ignitionEffectsPool.GetComponentInChildren<IgnitionScript>(true).gameObject;
                break;
        }

        //bulletObject.transform.SetParent(worldBullets);
        //bulletObject.transform.position = gunPort.transform.position;
        //// Return bullet and let GunPort handle how to fire and set initial velocities
        //return bulletObject;
        effectObject.transform.SetParent(worldEffects);
        effectObject.transform.position = new Vector3(position.x, position.y, position.z);
        effectObject.transform.up = new Vector3(lookAt.x, lookAt.y, lookAt.z);

        effectObject.SetActive(true);

        switch (effectType)
        {
            // Get First Child, set parent to gunport (to remove from respective pool)
            case GameConstants.EffectTypes.Ignition:
                effectObject.GetComponent<IgnitionScript>().FadeIn();
                break;
            case GameConstants.EffectTypes.Explosion:
                effectObject.GetComponent<ExplosionScript>().FadeIn();
                break;
            case GameConstants.EffectTypes.Implosion:
                effectObject.GetComponent<ImplosionScript>().FadeIn();
                break;
        }

        return effectObject;
    }
}
